using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class RoomLobbyManager : NetworkBehaviour
{
    [SerializeField] private List<GameObject> playerSlots; // Assign 8 UI slots in Inspector
    [SerializeField] private Button readyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private TMP_Text chatContent;
    [SerializeField] private TMP_InputField chatInput;

    [Networked] private int selectedMap { get; set; } // Syncs map selection
    [Networked, Capacity(8)] private NetworkDictionary<PlayerRef, bool> playerReadyStatus => default;



    private NetworkRunner runner;

    private void Start()
    {
        runner = FindFirstObjectByType<NetworkRunner>();

        readyButton.onClick.AddListener(OnReadyClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        mapDropdown.onValueChanged.AddListener(OnMapChanged);
        chatInput.onSubmit.AddListener(OnChatMessageSent);

        if (runner.IsServer)
        {
            mapDropdown.interactable = true; // Only the host can change the map
        }
        else
        {
            mapDropdown.interactable = false;
        }

    }

    

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        UpdatePlayerSlots();
    }

    private void UpdatePlayerSlots(){
        Debug.Log($"Updating player slots {runner.ActivePlayers}");
        var players = runner.ActivePlayers.ToList(); // Convert to List for indexing
        Debug.Log(players);
        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < players.Count)
            {
                Debug.Log("Checking if player is ready");
                var player = players[i];
                playerSlots[i].SetActive(true);
                var textComponent = playerSlots[i].GetComponentInChildren<TMP_Text>();

                bool isReady = playerReadyStatus.ContainsKey(player) ? playerReadyStatus.Get(player) : false;
                textComponent.text = player.ToString() + (isReady ? " (READY)" : "");
                Debug.Log("Attempted to change ready status");
            }
            else
            {
                Debug.Log($"Setting {playerSlots[i]} to false");
                playerSlots[i].SetActive(false);
            }
        }
    }


    public void OnReadyClicked()
    {
        bool isReady = false;
        if (playerReadyStatus.TryGet(runner.LocalPlayer, out bool currentStatus)) {
            isReady = !currentStatus; // Toggle ready state
        }
        playerReadyStatus.Set(runner.LocalPlayer, isReady);
        UpdatePlayerSlots();

        // If all players are ready, start the game
        if (runner.IsServer && AllPlayersReady())
        {
            Debug.Log("Starting game");
            StartGame();
        }
        
    }

    private void OnExitClicked()
    {
        runner.Shutdown();
        // Load main menu scene or return to Fusion lobby
    }

    private void OnMapChanged(int value)
    {
        if (runner.IsServer)
        {
            selectedMap = value;
        }
    }

    private void OnChatMessageSent(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            RPC_SendChatMessage(message, runner.LocalPlayer);
            chatInput.text = "";
        }
    }

    private bool AllPlayersReady()
    {
        Debug.Log("Checking if all players ready");
        foreach (var player in runner.ActivePlayers)
        {
            if (!playerReadyStatus.TryGet(player, out bool isReady) || !isReady)
            {
                Debug.Log("Players not ready");
                return false;
            }
        }
        Debug.Log("Players ready");
        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendChatMessage(string message, PlayerRef sender)
    {
        chatContent.text += $"\n{sender}: {message}";
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f; // Auto-scroll to bottom
    }

    private void StartGame()
    {
        if (runner.IsSceneAuthority) // Only the server should load the scene
        {
            int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Project/Scenes/Sector1.unity");

            if (sceneBuildIndex >= 0)
            {
                SceneRef sceneRef = SceneRef.FromIndex(sceneBuildIndex);
                runner.LoadScene(sceneRef, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("[RoomLobbyManager] ERROR: Scene 'GameScene' not found in Build Settings!");
            }
        }
    }

}
