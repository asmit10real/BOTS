using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] OnPlayerJoined called for {player}");

        if (player == Runner.LocalPlayer)
        {
            Debug.Log($"[PlayerSpawner] Spawning player for {player} with input authority.");
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }

    // ✅ NEW: Force player spawn after scene transition
    private void Start()
    {
        StartCoroutine(EnsureRunnerBeforeSpawning());
    }

    private void SpawnExistingPlayers()
    {
        Debug.Log($"[PlayerSpawner] Attempting to spawn players...");

        if (RoomLobbyManager.PlayersBeforeTransition == null || RoomLobbyManager.PlayersBeforeTransition.Count == 0)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No players found to spawn!");
            return;
        }

        foreach (var player in RoomLobbyManager.PlayersBeforeTransition)
        {
            Debug.Log($"[PlayerSpawner] Spawning player: {player}");

            if (Runner != null)
            {
                Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
            }
            else
            {
                Debug.LogError("[PlayerSpawner] ERROR: Runner is null when trying to spawn players!");
            }
        }
    }

    private IEnumerator EnsureRunnerBeforeSpawning()
    {
        Debug.Log("[PlayerSpawner] Waiting for NetworkRunner to initialize...");

        // ✅ Wait until Runner is assigned
        while (Runner == null)
        {
            yield return null;
        }

        Debug.Log("[PlayerSpawner] NetworkRunner detected! Waiting for players...");

        // ✅ Ensure Fusion has stabilized after scene load
        yield return new WaitForSeconds(1.5f);

        SpawnExistingPlayers();
    }
}
