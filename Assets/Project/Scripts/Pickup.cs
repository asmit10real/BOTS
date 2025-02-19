using UnityEngine;
using Fusion;

public abstract class Pickup : NetworkBehaviour {
    public float lifeTime = 10f; // Despawns after time
    protected bool isCollected = false;

    public override void Spawned() {
        Invoke(nameof(DestroyPickup), lifeTime); // Auto-destroy after X seconds
    }

    public virtual void OnTriggerEnter(Collider other) {
        if (isCollected) return;

        if (other.CompareTag("Player")) {
            ApplyEffect(other.gameObject);
            isCollected = true;
            Runner.Despawn(Object); // ✅ Destroy the pickup across network
        }
    }

    protected abstract void ApplyEffect(GameObject player);

    private void DestroyPickup() {
        if (!isCollected) {
            Runner.Despawn(Object);
        }
    }
}
