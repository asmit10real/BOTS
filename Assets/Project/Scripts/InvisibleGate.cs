using UnityEngine;
using Fusion;

public class InvisibleGate : NetworkBehaviour {
    public Collider gateCollider; // Regular BoxCollider (Not Trigger)
    public SectorManager sectorManager;

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
        if (isOpen) return; // Double-check to prevent repeated calls

        isOpen = true; // Mark as opened
        if (gateCollider != null) {
            gateCollider.enabled = false; // Disable collider to allow movement
        }
        sectorManager.OpenNextArea(); // Tell the SectorManager to spawn new enemies
    }
}
