using UnityEngine;
using System.Collections.Generic;
using Fusion;
using System.Linq;
public class EnemySpawner : NetworkBehaviour {
    public List<Transform> spawnPoints; // Spawn locations
    public List<NetworkPrefabRef> enemyPrefabs; // Enemy types
    public List<int> keyEnemyIndexes; // ✅ List of key enemy indexes
    public InvisibleGate linkedGate; // ✅ The gate this spawner controls
    private Dictionary<NetworkObject, bool> spawnedEnemies = new Dictionary<NetworkObject, bool>();

    private bool shouldSpawn = false;
    private bool hasSpawned = false; // ✅ New safeguard variable


    public void ActivateSpawner() {
        if (hasSpawned) return; // ✅ Prevents duplicate activation
        Debug.Log($"[EnemySpawner] Activating spawner for section...");
        hasSpawned = true; // ✅ Mark as activated
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

            NetworkObject enemy = Runner.Spawn(enemyPrefabs[i], spawnPosition, spawnRotation, null, (runner, obj) => {
                EnemyHealth enemyHealth = obj.GetComponent<EnemyHealth>();
                if (enemyHealth != null) {
                    enemyHealth.AssignSpawner(this); // ✅ Assign this spawner to the enemy
                }
            });

            bool isKeyEnemy = keyEnemyIndexes.Contains(i);
            spawnedEnemies[enemy] = isKeyEnemy;

            if (isKeyEnemy) {
                Debug.Log($"[EnemySpawner] Enemy {i} is a KEY enemy.");
            }
        }
    }


    public void NotifyEnemyDeath(NetworkObject enemy) {
        if (!spawnedEnemies.ContainsKey(enemy)) return;

        Debug.Log($"[EnemySpawner] Enemy {enemy.Id} has died.");
        bool wasKeyEnemy = spawnedEnemies[enemy]; // ✅ Capture if it was a key enemy
        spawnedEnemies.Remove(enemy); // ✅ Remove enemy from tracking

        // ✅ Ensure we're only counting remaining key enemies correctly
        if (wasKeyEnemy) {
            int remainingKeyEnemies = spawnedEnemies.Values.Count(v => v);
            Debug.Log($"CCHECKING IF KEY NUMBERS OF ENEMIES IS RIGHT {remainingKeyEnemies}");

            if (remainingKeyEnemies == 0) {
                Debug.Log($"[EnemySpawner] All key enemies defeated. Opening gate.");
                linkedGate?.OpenGate();
            }
        }
    }


}
