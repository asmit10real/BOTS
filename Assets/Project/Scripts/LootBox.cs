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

    public override void Spawned() {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();

        if (rb == null || boxCollider == null) {
            Debug.LogError($"[LootBox] Missing Rigidbody or Collider on {gameObject.name}!");
            return;
        }

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
        Debug.Log($"[LootBox] Collision detected with {collision.gameObject.name}, Layer: {collision.gameObject.layer}");
        if (!hasLanded && collision.gameObject.CompareTag("Ground")) { // ✅ Only detect ground impact
            Debug.Log("Collision detected");
            hasLanded = true; // ✅ Mark as landed
            EnableCollection();
        }
    }

    private void EnableCollection() {
        rb.isKinematic = true; // Stops movement
        GetComponent<Collider>().isTrigger = true; // Allows collection
        isCollectible = true;
        Debug.Log("[LootBox] Now collectible!");
    }

    private void OnTriggerEnter(Collider other) {
        if (!isCollectible || !Object.HasStateAuthority) return;

        if (other.CompareTag("Player")) {
            Debug.Log($"[LootBox] {other.name} collected loot!");
            Runner.Despawn(Object);
        }
    }
}
