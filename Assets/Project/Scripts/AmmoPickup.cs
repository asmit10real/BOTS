using UnityEngine;

public class AmmoPickup : Pickup {
    public int ammoAmount = 5;

    protected override void ApplyEffect(GameObject player) {
        // Placeholder for when guns are added
        Debug.Log($"[Pickup] {player.name} picked up {ammoAmount} ammo!");
    }
}
