using UnityEngine;

public class HealthPickup : Pickup {
    public int healAmount = 25;

    protected override void ApplyEffect(GameObject player) {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null) {
            //playerHealth.Heal(healAmount);
            Debug.Log($"[Pickup] {player.name} restored {healAmount} HP!");
        }
    }
}
