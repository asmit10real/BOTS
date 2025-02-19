using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined {
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player) {
        if (player == Runner.LocalPlayer) {
            Debug.Log($"Spawning player for {player} with input authority.");
            
            // Spawn player and assign Input Authority
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }
}
