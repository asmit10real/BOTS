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

    [Networked, Capacity(8)] 
    private NetworkDictionary<PlayerRef, NetworkBool> playerReadyStatus => default;
    [SerializeField] private NetworkObject lobbyPlayerPrefab; // ✅ Assign this in Inspector!


    private NetworkRunner runner;
    private LobbySessionManager sessionManager;

    private void Start()
    {
        runner = FindFirstObjectByType<NetworkRunner>();
        sessionManager = FindFirstObjectByType<LobbySessionManager>();

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

        UpdatePlayerSlots();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        UpdatePlayerSlots();
    }

    private void UpdatePlayerSlots()
    {
        var players = runner.ActivePlayers.ToList();

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < players.Count)
            {
                var player = players[i];
                playerSlots[i].SetActive(true);
                var textComponent = playerSlots[i].GetComponentInChildren<TMP_Text>();

                bool isReady = sessionManager != null && sessionManager.AllPlayersReady();
                textComponent.text = player.ToString() + (isReady ? " (READY)" : "");
            }
            else
            {
                playerSlots[i].SetActive(false);
            }
        }
    }

    public void OnReadyClicked()
    {
        Debug.Log("[RoomLobbyManager] Ready button clicked.");

        PlayerRef localPlayer = PlayerRef.None;

        foreach (var player in runner.ActivePlayers)
        {
            if (runner.TryGetPlayerObject(player, out NetworkObject playerObject) && playerObject.HasInputAuthority)
            {
                localPlayer = player;
                break;
            }
        }

        if (localPlayer == PlayerRef.None)
        {
            Debug.LogError("[RoomLobbyManager] ERROR: Unable to determine local player! No valid PlayerRef found.");
            return;
        }

        Debug.Log($"[RoomLobbyManager] Ready clicked by {localPlayer}");

        if (playerReadyStatus.TryGet(localPlayer, out NetworkBool isReady))
        {
            playerReadyStatus.Set(localPlayer, !isReady);
        }
        else
        {
            playerReadyStatus.Set(localPlayer, true);
        }

        UpdatePlayerSlots();

        if (runner.IsServer && sessionManager.AllPlayersReady())
        {
            Debug.Log("[RoomLobbyManager] All players ready! Starting game...");
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendChatMessage(string message, PlayerRef sender)
    {
        chatContent.text += $"\n{sender}: {message}";
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f; // Auto-scroll to bottom
    }

    private void StartGame()
    {
        if (runner.IsSceneAuthority)
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
