using UnityEngine;

public class StaticFieldPickup : Pickup {
    protected override void ApplyEffect(GameObject player) {
        Debug.Log($"[Pickup] {player.name} picked up Static Field (stun effect not implemented yet)!");
    }
}
