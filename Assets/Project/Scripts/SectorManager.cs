using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class SectorManager : NetworkBehaviour {
    public List<EnemySpawner> enemySpawners;
    public List<InvisibleGate> gates;

    private int currentWave = 0;
    private bool waveStarted = false;

    public override void FixedUpdateNetwork() {
        if (!Object.HasStateAuthority || waveStarted) return;

        waveStarted = true;
        StartNextWave();
    }

    private void StartNextWave() {
        if (currentWave < enemySpawners.Count) {
            Debug.Log($"[SectorManager] Activating wave {currentWave}");
            enemySpawners[currentWave].ActivateSpawner(); // ✅ Tell the spawner to start
        }
    }

    public void OpenNextArea() {
        if (currentWave < gates.Count) {
            gates[currentWave].OpenGate();
        }

        currentWave++;
        waveStarted = false; // ✅ Allows the next wave to trigger
    }
}
