using UnityEngine;
using Fusion;

public class EnemyHealth : NetworkBehaviour {
    [Networked] public int Health { get; set; } = 100;

    public KeyEnemyTracker keyEnemyTracker; // If the enemy is a key enemy

    public void TakeDamage(int damage) {
        if (!Object.HasStateAuthority) return; // Only the host applies damage

        Health -= damage;
        if (Health <= 0) {
            Die();
        }
    }

    private void Die() {
        if (keyEnemyTracker != null) {
            keyEnemyTracker.NotifyDeath(); // Inform the gate if this is a key enemy
        }
        Runner.Despawn(Object); // Remove from the game
    }

    void Update() {
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.K)) { // Press 'K' to kill all enemies on the server
            TakeDamage(9999);
        }
    }
}
