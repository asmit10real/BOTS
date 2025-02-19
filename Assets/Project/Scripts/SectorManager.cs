using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class SectorManager : NetworkBehaviour {
    public List<EnemySpawner> enemySpawners; // List of all spawners in this sector

    public override void Spawned() {
        if (Object.HasStateAuthority) {
            Debug.Log("[SectorManager] Game started, activating first enemy spawner.");
            enemySpawners[0]?.ActivateSpawner(); // ✅ Activate first area
        }
    }
}

