using Fusion;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour
{
    public GameObject PlayerPrefab;
    private Transform[] SectorSpawnPoints; // Dynamically assigned in each scene
    //private bool hasSpawned = false; // Prevent duplicate spawns

    public void SpawnLocalPlayerIfInSector()
    {
        /*// 🚨 Prevent duplicate execution
        if (hasSpawned)
        {
            Debug.Log("[PlayerSpawner] Skipping duplicate spawn.");
            return;
        }
        hasSpawned = true;
        */
        // 🚨 Skip spawning if we are still in the lobby
        if (SceneManager.GetActiveScene().name == "RoomLobby")
        {
            Debug.Log("[PlayerSpawner] In RoomLobby, skipping player spawn.");
            return;
        }

        Debug.Log($"[PlayerSpawner] Spawning local player in {SceneManager.GetActiveScene().name}");

        // 🔹 Find all valid spawn points
        GameObject spawnPointParent = GameObject.Find("PlayerSpawnPoints");
        if (spawnPointParent == null)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No 'PlayerSpawnPoints' object found in the scene!");
            return;
        }

        SectorSpawnPoints = spawnPointParent.GetComponentsInChildren<Transform>()
            .Where(t => t != spawnPointParent.transform) // Exclude the parent itself
            .ToArray();

        if (SectorSpawnPoints.Length == 0)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No valid spawn points found!");
            return;
        }

        Debug.Log($"✅ [PlayerSpawner] Found {SectorSpawnPoints.Length} spawn points.");

        // 🚨 Ensure we have a valid NetworkRunner
        if (Runner == null)
        {
            Debug.LogError("[PlayerSpawner] ERROR: No active NetworkRunner found! Cannot spawn player.");
            return;
        }

        // 🚀 Assign a unique spawn point
        int spawnIndex = Runner.LocalPlayer.RawEncoded % SectorSpawnPoints.Length;

        Debug.Log($"[PlayerSpawner] Spawning LOCAL player {Runner.LocalPlayer} at spawn point {spawnIndex}...");

        Runner.Spawn(PlayerPrefab, SectorSpawnPoints[spawnIndex].position, Quaternion.identity, Runner.LocalPlayer);

        Debug.Log($"✅ [PlayerSpawner] Spawned LOCAL player {Runner.LocalPlayer}");
    }
}
