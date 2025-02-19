using UnityEngine;
using Fusion;

public class EnemyHealth : NetworkBehaviour {
    public int maxHealth = 100;
    [Networked] private int currentHealth { get; set; }
    private EnemySpawner spawner;

    public GameObject[] pickupPrefabs; // ✅ Assign power-up prefabs in Inspector
    public float dropChance = 0.25f; // ✅ 25% chance to drop

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
        TryDropPickup();
        Runner.Despawn(Object);
    }
    
    private void TryDropPickup() {
        if (pickupPrefabs.Length == 0) return;
        if (Random.value > dropChance) return; // ✅ 25% chance

        GameObject pickupPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f; // Slightly above enemy
        Runner.Spawn(pickupPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"[EnemyHealth] Dropped {pickupPrefab.name}!");
    }
}


