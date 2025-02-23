using Fusion;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour
{
    public GameObject PlayerPrefab;
    private Transform[] SectorSpawnPoints; // Dynamically assigned in each scene

    public void Start()
    {
        StartCoroutine(SpawnLocalPlayerIfInSector());
    }

    private IEnumerator SpawnLocalPlayerIfInSector()
    {

        // 🚨 Skip spawning if we are still in the lobby
        if (SceneManager.GetActiveScene().name == "RoomLobby")
        {
            Debug.Log("[PlayerSpawner] In RoomLobby, skipping player spawn.");
            yield break;
        }

        Debug.Log($"[PlayerSpawner] Spawning local player in {SceneManager.GetActiveScene().name}");

        // 🔹 Find all valid spawn points
        GameObject spawnPointParent = GameObject.Find("PlayerSpawnPoints");
        if (spawnPointParent == null)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No 'PlayerSpawnPoints' object found in the scene!");
            yield break;
        }

        SectorSpawnPoints = spawnPointParent.GetComponentsInChildren<Transform>()
            .Where(t => t != spawnPointParent.transform) // Exclude the parent itself
            .ToArray();

        if (SectorSpawnPoints.Length == 0)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No valid spawn points found!");
            yield break;
        }

        Debug.Log($"✅ [PlayerSpawner] Found {SectorSpawnPoints.Length} spawn points.");

        // 🚨 Ensure we have a valid NetworkRunner
        if (Runner == null)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No active NetworkRunner found! Cannot spawn player.");
            yield break;
        }

        // 🚨 Only spawn the local player (each client will handle their own spawn)
        int spawnIndex = Runner.LocalPlayer.RawEncoded % SectorSpawnPoints.Length;

        Debug.Log($"[PlayerSpawner] Spawning LOCAL player {Runner.LocalPlayer} at spawn point {spawnIndex}...");

        Runner.Spawn(PlayerPrefab, SectorSpawnPoints[spawnIndex].position, Quaternion.identity, Runner.LocalPlayer);

        Debug.Log($"✅ [PlayerSpawner] Spawned LOCAL player {Runner.LocalPlayer}");
    }
}
