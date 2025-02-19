using UnityEngine;
using Fusion;

public class PlayerHealth : NetworkBehaviour {
    [Networked] public int Health { get; set; } = 100;

    public void TakeDamage(int damage) {
        if (!Object.HasStateAuthority) return; // Only the owner modifies health

        Health -= damage;
        Debug.Log($"Player {Object.InputAuthority} took {damage} damage! Remaining HP: {Health}");

        if (Health <= 0) {
            Die();
        }
    }

    private void Die() {
        Debug.Log($"Player {Object.InputAuthority} has died.");
        RPC_HandleDeath();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HandleDeath() {
        gameObject.SetActive(false);
        
        // TEMPORARY RESPAWN SYSTEM (Teleport Player Back)
        Runner.Invoke(nameof(Respawn), 3f);
    }

    private void Respawn() {
        Health = 100;
        gameObject.SetActive(true);
        transform.position = new Vector3(0, 1, 0); // TEMP: Respawn at (0,1,0)
    }
}
