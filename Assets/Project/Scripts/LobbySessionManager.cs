using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class LobbySessionManager : NetworkBehaviour, IPlayerJoined
{
    [Networked, Capacity(8)] 
    private NetworkDictionary<PlayerRef, NetworkBool> playerReadyStatus => default;

    public delegate void PlayerListUpdated();
    public event PlayerListUpdated OnPlayerListUpdated; // 🔹 UI can listen for updates

    public override void Spawned()
    {
        Debug.Log("[LobbySessionManager] Spawned and tracking players.");
        UpdatePlayerList();
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[LobbySessionManager] Player joined: {player}");

        if (!playerReadyStatus.ContainsKey(player))
        {
            playerReadyStatus.Set(player, false); // Default to not ready
        }

        // 🔹 Ensure RoomLobbyManager updates UI when a player joins
        var roomLobbyManager = FindFirstObjectByType<RoomLobbyManager>();
        if (roomLobbyManager != null)
        {
            roomLobbyManager.UpdatePlayerSlots();
        }

        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        Debug.Log($"[LobbySessionManager] Updating Player List. Active Players: {Runner.ActivePlayers.Count()}");
        OnPlayerListUpdated?.Invoke(); // 🔹 Notify `RoomLobbyManager`
    }

    public void ToggleReady(PlayerRef player)
    {
        if (playerReadyStatus.TryGet(player, out NetworkBool isReady))
        {
            playerReadyStatus.Set(player, !isReady);
        }
        else
        {
            playerReadyStatus.Set(player, true);
        }

        Debug.Log($"[LobbySessionManager] Player {player} ready state is now {playerReadyStatus[player]}");
        OnPlayerListUpdated?.Invoke(); // 🔹 Notify `RoomLobbyManager`
    }

    public bool AllPlayersReady()
    {
        foreach (var player in Runner.ActivePlayers)
        {
            if (!playerReadyStatus.TryGet(player, out NetworkBool isReady) || !isReady)
            {
                return false;
            }
        }
        return true;
    }
}
