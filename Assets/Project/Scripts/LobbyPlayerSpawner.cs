using Fusion;
using UnityEngine;

public class LobbyPlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public RoomLobbyManager roomLobbyManager;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[LobbyPlayerSpawner] OnPlayerJoined called for {player}");

        if (Runner.IsServer || Runner.IsSharedModeMasterClient)
        {
            int index = player.RawEncoded % 8;
            // Spawn player at room-specific position
            Vector3 lobbyPosition = GetLobbySpawnPosition(player);
            Transform uiSlot = roomLobbyManager.playerSlots[index].transform; // Get the assigned UI panel
            Vector3 adjustedPosition = uiSlot.position + new Vector3(0, 0, -2); // Offset slightly to make it visible
            
            var playerObject = Runner.Spawn(PlayerPrefab, adjustedPosition, Quaternion.identity, player);
            DisablePlayerScripts(playerObject);
            Debug.Log($"[LobbyPlayerSpawner] Checking if player exists in hierarchy: {playerObject}");
            //playerObject.transform.SetParent(uiSlot); // Attach to UI panel
            playerObject.transform.localScale = Vector3.one * 25f; // Scale down to fit in the UI


            // Disable movement (freeze in place)
            var rigidbody = playerObject.GetComponent<Rigidbody>();
            if (rigidbody) rigidbody.isKinematic = true; 

            Runner.SetPlayerObject(player, playerObject);

            // 🔹 Confirm that the player is now recognized
            if (Runner.TryGetPlayerObject(player, out var obj))
            {
                Debug.Log($"✅ [LobbyPlayerSpawner] Successfully set PlayerObject for {player} -> {obj.name}");
            }
            else
            {
                Debug.LogError($"❌ [LobbyPlayerSpawner] FAILED to set PlayerObject for {player}");
            }
        }
    }

    private Vector3 GetLobbySpawnPosition(PlayerRef player)
    {
        // Example: Each player gets a unique slot in the room
        int index = player.RawEncoded % 8;
        return new Vector3(index * 2, 1, 0); 
    }

    public void OnReturnToRoom()
    {
        foreach (var player in Runner.ActivePlayers)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerObject))
            {
                Vector3 lobbyPosition = GetLobbySpawnPosition(player);
                playerObject.transform.position = lobbyPosition;

                // Freeze movement
                var rigidbody = playerObject.GetComponent<Rigidbody>();
                if (rigidbody) rigidbody.isKinematic = true; 
            }
        }
    }

    private void DisablePlayerScripts(NetworkObject playerObject)
    {
        var playerController = playerObject.GetComponent<PlayerController>();
        var playerCombat = playerObject.GetComponent<PlayerCombat>();
        var playerHealth = playerObject.GetComponent<PlayerHealth>();

        if (playerController) playerController.enabled = false;
        if (playerCombat) playerCombat.enabled = false;
        if (playerHealth) playerHealth.enabled = false;

        Debug.Log($"[LobbyPlayerSpawner] Disabled movement, combat, and health scripts for {playerObject.name} in lobby.");
    }

}

