using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Linq;

public class LobbySessionManager : NetworkBehaviour, IPlayerJoined
{
    [Networked, Capacity(8)] 
    private NetworkDictionary<PlayerRef, NetworkBool> playerReadyStatus => default;

    public override void Spawned()
    {
        Debug.Log("[LobbySessionManager] Spawned and tracking players.");
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        //Debug.Log($"[New debug: LobbySessionManager] Active Players: {Runner.ActivePlayers.Count()}");
        
        foreach (var player in Runner.ActivePlayers)
        {
            //Debug.Log($"[New debug: LobbySessionManager] Checking Player: {player}");

            if (Runner.TryGetPlayerObject(player, out NetworkObject playerObject))
            {
                //Debug.Log($"[New debug: LobbySessionManager] Found player object for {player}: {playerObject.name}, HasInputAuthority: {playerObject.HasInputAuthority}");
            }
            else
            {
                //Debug.Log($"[New debug: LobbySessionManager No PlayerObject found for {player}");
            }
        }
    }


    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[LobbySessionManager] Player joined: {player}");

        if (!playerReadyStatus.ContainsKey(player))
        {
            playerReadyStatus.Set(player, false); // Default to not ready
        }

        Debug.Log($"[LobbySessionManager] Current Active Players: {Runner.ActivePlayers.Count()}");
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
