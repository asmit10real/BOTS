using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class RoomLobbyManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerSlots;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private TMP_Text chatContent;
    [SerializeField] private TMP_InputField chatInput;

    private LobbySessionManager sessionManager;

    private void Start()
    {
        sessionManager = FindFirstObjectByType<LobbySessionManager>();

        if (sessionManager == null)
        {
            Debug.LogError("[RoomLobbyManager] ERROR: LobbySessionManager not found!");
            return;
        }

        readyButton.onClick.AddListener(OnReadyClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        mapDropdown.onValueChanged.AddListener(OnMapChanged);
        chatInput.onSubmit.AddListener(OnChatMessageSent);

        sessionManager.OnPlayerListUpdated += UpdatePlayerSlots; // 🔹 UI updates when players change

        //UpdatePlayerSlots();
    }

    public void UpdatePlayerSlots()
    {
        var players = sessionManager.Runner.ActivePlayers.ToList();
        Debug.Log($"[RoomLobbyManager] Updating UI. Active Players: {players.Count}");

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < players.Count)
            {
                var player = players[i];
                playerSlots[i].SetActive(true);
                var textComponent = playerSlots[i].GetComponentInChildren<TMP_Text>();

                bool isReady = sessionManager.AllPlayersReady();
                textComponent.text = player.ToString() + (isReady ? " (READY)" : "");
            }
            else
            {
                playerSlots[i].SetActive(false);
            }
        }
    }

    private void OnReadyClicked()
    {
        Debug.Log("[RoomLobbyManager] Ready button clicked.");
        
        if (sessionManager == null) return;
        sessionManager.ToggleReady(sessionManager.Runner.LocalPlayer); // 🔹 Calls `ToggleReady()` in `LobbySessionManager`
        
        UpdatePlayerSlots();

        if (sessionManager.Runner.IsServer && sessionManager.AllPlayersReady())
        {
            Debug.Log("[RoomLobbyManager] All players ready! Starting game...");
            StartGame();
        }
    }

    private void OnExitClicked()
    {
        sessionManager.Runner.Shutdown();
    }

    private void OnMapChanged(int value)
    {
        Debug.Log($"[RoomLobbyManager] Map changed to {value}");
    }

    private void OnChatMessageSent(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            RPC_SendChatMessage(message, sessionManager.Runner.LocalPlayer);
            chatInput.text = "";
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendChatMessage(string message, PlayerRef sender)
    {
        chatContent.text += $"\n{sender}: {message}";
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }

    private void StartGame()
    {
        if (sessionManager.Runner.IsSceneAuthority)
        {
            int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Project/Scenes/Sector1.unity");

            if (sceneBuildIndex >= 0)
            {
                SceneRef sceneRef = SceneRef.FromIndex(sceneBuildIndex);
                sessionManager.Runner.LoadScene(sceneRef, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("[RoomLobbyManager] ERROR: Scene 'GameScene' not found in Build Settings!");
            }
        }
    }
}
