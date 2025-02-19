using UnityEngine;
using Fusion;

public class KeyEnemyTracker : NetworkBehaviour {
    public InvisibleGate linkedGate;

    public void NotifyDeath() {
        if (linkedGate != null) {
            linkedGate.RPC_EnemyKilled();
        }
    }
}
