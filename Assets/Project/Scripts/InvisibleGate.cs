using UnityEngine;
using Fusion;

public class InvisibleGate : NetworkBehaviour {
    public Collider gateCollider; // Regular BoxCollider (Not Trigger)
    public SectorManager sectorManager;
    public EnemySpawner nextSectionSpawner;

    private int enemiesKilled = 0;
    public int requiredEnemiesToKill;
    private bool isOpen = false; // Prevents multiple calls

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EnemyKilled() {
        if (isOpen) return; // Prevents stack overflow
        enemiesKilled++;
        if (enemiesKilled >= requiredEnemiesToKill) {
            OpenGate();
        }
    }

    public void OpenGate() {
        Debug.Log($"[InvisibleGate] Opening gate...");

        if (gateCollider != null) {
            gateCollider.enabled = false;
            Debug.Log($"[InvisibleGate] Gate collider disabled.");

            // ✅ Activate the next wave of enemies
            if (nextSectionSpawner != null) {
                Debug.Log($"[InvisibleGate] Activating next enemy spawner...");
                nextSectionSpawner.ActivateSpawner();
            }
        } else {
            Debug.LogError($"[InvisibleGate] Gate collider reference is missing!");
        }
    }
}
