using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class PlayerSpawner : SimulationBehaviour
{
    public GameObject PlayerPrefab;
    public Transform[] SectorSpawnPoints; // Assign these in the Inspector
/*
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[PlayerSpawner] OnPlayerJoined called for {player}");

        if (player == Runner.LocalPlayer)
        {
            Debug.Log($"[PlayerSpawner] Spawning player for {player} with input authority.");
            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
        }
    }
*/
    private void Start()
    {
        StartCoroutine(RepositionPlayersAfterSceneLoad());
    }

    private IEnumerator RepositionPlayersAfterSceneLoad()
    {
        yield return new WaitForSeconds(1.5f);

        foreach (var player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerObject))
            {
                Debug.Log($"[PlayerSpawner] Moving {player} to sector start position.");

                // Move the player to the correct spawn point
                int spawnIndex = player.RawEncoded % SectorSpawnPoints.Length;
                playerObject.transform.position = SectorSpawnPoints[spawnIndex].position;

                // ✅ Re-enable movement, combat, and health
                EnablePlayerScripts(playerObject);

                // Re-enable physics/movement
                var rigidbody = playerObject.GetComponent<Rigidbody>();
                if (rigidbody) rigidbody.isKinematic = false; 
            }
        }
    }

    private void EnablePlayerScripts(NetworkObject playerObject)
    {
        var playerController = playerObject.GetComponent<PlayerController>();
        var playerCombat = playerObject.GetComponent<PlayerCombat>();
        var playerHealth = playerObject.GetComponent<PlayerHealth>();

        if (playerController) playerController.enabled = true;
        if (playerCombat) playerCombat.enabled = true;
        if (playerHealth) playerHealth.enabled = true;

        Debug.Log($"[PlayerSpawner] Re-enabled movement, combat, and health scripts for {playerObject.name} in game scene.");
    }
}
