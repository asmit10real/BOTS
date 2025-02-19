using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour {
    public List<Transform> spawnPoints;
    public List<NetworkPrefabRef> enemyPrefabs;
    private bool shouldSpawn = false;

    public void ActivateSpawner() {
        shouldSpawn = true;
    }

    public override void FixedUpdateNetwork() {
        if (!Object.HasStateAuthority || !shouldSpawn) return;

        shouldSpawn = false;
        SpawnEnemies();
    }

    private void SpawnEnemies() {
        if (spawnPoints.Count != enemyPrefabs.Count) {
            Debug.LogError("[EnemySpawner] Mismatch in spawnPoints and enemyPrefabs count! Fix this in the inspector.");
            return;
        }

        for (int i = 0; i < spawnPoints.Count; i++) {
            Vector3 spawnPosition = spawnPoints[i].position;
            Quaternion spawnRotation = spawnPoints[i].rotation;

            Debug.Log($"[EnemySpawner] Attempting to spawn enemy {i} at {spawnPosition}");

            // ✅ Fusion will now sync position automatically via NetworkTransform
            NetworkObject enemy = Runner.Spawn(enemyPrefabs[i], spawnPosition, spawnRotation);

            Debug.Log($"[EnemySpawner] Spawned enemy {i} at {spawnPosition}, Network ID: {enemy.Id}");
        }
    }
}
