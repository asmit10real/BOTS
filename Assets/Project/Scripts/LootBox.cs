using UnityEngine;
using Fusion;
using System.Collections;

public class LootBox : NetworkBehaviour {
    public float launchForce = 5f;
    public float forwardForce = 3f;
    private bool isCollectible = false;
    private Rigidbody rb;
    private bool hasLanded = false; // ✅ Delay velocity checks
    private Collider boxCollider;
    
    public ItemData[] possibleDrops; // ✅ Assigned in Inspector
    public float[] dropChances; // ✅ Same length as possibleDrops

    public override void Spawned() {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();

        if (rb == null || boxCollider == null) {
            Debug.LogError($"[LootBox] Missing Rigidbody or Collider on {gameObject.name}!");
            return;
        }

        // ❌ Ignore all collisions initially except Ground (so it can land properly)
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LootBox"), LayerMask.NameToLayer("Player"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LootBox"), LayerMask.NameToLayer("Enemy"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LootBox"), LayerMask.NameToLayer("LootBox"), true);
        boxCollider.isTrigger = false; // ✅ Ensure it's solid initially

        StartCoroutine(LaunchLootBox());
    }

    private IEnumerator LaunchLootBox() {
        transform.Rotate(0, Random.Range(0f, 360f), 0);
        Vector3 launchDirection = transform.forward * forwardForce + Vector3.up * launchForce;
        rb.AddForce(launchDirection, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f); // ✅ Delay before checking velocity
    }

    private void OnCollisionEnter(Collision collision) {
        if (!hasLanded && collision.gameObject.CompareTag("Ground")) { // ✅ Only detect ground impact
            hasLanded = true; // ✅ Mark as landed
            EnableCollection();
        }
    }

    private void EnableCollection() {
        rb.isKinematic = true; // Stops movement
        GetComponent<Collider>().isTrigger = true; // Allows collection
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LootBox"), LayerMask.NameToLayer("Player"), false);
        isCollectible = true;
        Debug.Log("[LootBox] Now collectible!");
    }

    private void OnTriggerEnter(Collider other) {
        if (!isCollectible || !Object.HasStateAuthority) return;

        if (other.CompareTag("Player")) {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) {
                GiveItemToPlayer(player);
            }
            Runner.Despawn(Object);
        }
    }

    private void GiveItemToPlayer(PlayerController player) {
        int chosenIndex = GetRandomDropIndex();
        if (chosenIndex != -1) {
            ItemData droppedItem = possibleDrops[chosenIndex];

            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null) {
                bool success = inventory.AddItem(droppedItem);
                if (success) {
                    Debug.Log($"[LootBox] {droppedItem.itemName} added to {player.name}'s inventory.");
                } else {
                    Debug.Log("[LootBox] Inventory full! Item was not added.");
                }
            }
        }
    }

    private int GetRandomDropIndex() {
        float totalChance = 0;
        foreach (float chance in dropChances) totalChance += chance;
        float randomValue = Random.Range(0, totalChance);
        
        float currentChance = 0;
        for (int i = 0; i < dropChances.Length; i++) {
            currentChance += dropChances[i];
            if (randomValue <= currentChance) return i;
        }
        Debug.LogError("[LootBox] ERROR: Loot roll failed!");
        return -1; // Should never happen
    }
}
