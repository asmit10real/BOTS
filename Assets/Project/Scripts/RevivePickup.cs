using UnityEngine;

public class RevivePickup : Pickup {
    protected override void ApplyEffect(GameObject player) {
        // Revive all teammates logic here
        Debug.Log($"[Pickup] {player.name} triggered a team revive!");
    }
}
