using UnityEngine;
using Fusion;

public class EnemyHealth : NetworkBehaviour {
    public int maxHealth = 100;
    [Networked] private int currentHealth { get; set; }
    private EnemySpawner spawner;

    public GameObject[] pickupPrefabs; // ✅ Assign power-up prefabs in Inspector
    public float dropChance = 0.25f; // ✅ 25% chance to drop

    public DropTable dropTable; // ✅ Reference the drop table
    public float baseDropChance = 1f; // ✅ Base chance for any loot to drop
    public float luckMultiplier = 0.2f; // ✅ Luck increases drop rate

    public override void Spawned() {
        currentHealth = maxHealth;
        // ✅ Find the Drop Table in the scene and assign it dynamically
        if (dropTable == null) {
            dropTable = FindFirstObjectByType<DropTable>();
            if (dropTable == null) {
                Debug.LogError("[EnemyHealth] No DropTable found in the scene!");
            }
        }
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
        TryDropLoot();
        Debug.Log("Attemping to despawn..");
        Runner.Despawn(Object);
        Debug.Log("Enemy should be gone");
    }

    private void TryDropPickup() {
        if (pickupPrefabs.Length == 0) return;
        if (Random.value > dropChance) return; // ✅ 25% chance

        GameObject pickupPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f; // Slightly above enemy
        Runner.Spawn(pickupPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"[EnemyHealth] Dropped {pickupPrefab.name}!");
    }

    private void TryDropLoot() {
        if (dropTable == null) return;

        float totalDropChance = baseDropChance + luckMultiplier;
        int dropCount = (Random.value <= totalDropChance) ? 1 : 0;

        // Additional roll for extra loot (higher luck increases chance)
        if (Random.value <= luckMultiplier) {
            dropCount++;
        }

        for (int i = 0; i < dropCount; i++) {
            GameObject lootPrefab = dropTable.GetRandomLoot();
            if (lootPrefab == null) continue;

            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Runner.Spawn(lootPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[EnemyHealth] Dropped {lootPrefab.name}!");
        }
    }
}


