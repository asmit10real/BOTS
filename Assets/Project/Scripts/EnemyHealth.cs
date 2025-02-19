using UnityEngine;
using Fusion;

public class EnemyHealth : NetworkBehaviour {
    public int maxHealth = 100;
    [Networked] private int currentHealth { get; set; }
    private EnemySpawner spawner;

    public override void Spawned() {
        currentHealth = maxHealth;
    }

    public void AssignSpawner(EnemySpawner enemySpawner) {
        spawner = enemySpawner;
        Debug.Log($"[EnemyHealth] Assigned {Object.Id} to spawner {spawner.gameObject.name}");
    }

    public void TakeDamage(int damage) {
        if (!Object.HasStateAuthority) return;

        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        Debug.Log($"[EnemyHealth] Enemy {Object.Id} has died.");
        spawner?.NotifyEnemyDeath(Object); // ✅ Notify the correct spawner
        Runner.Despawn(Object);
    }
}


