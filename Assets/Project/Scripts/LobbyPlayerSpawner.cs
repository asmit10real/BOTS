using Fusion;
using UnityEngine;

public class LobbyPlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"[LobbyPlayerSpawner] OnPlayerJoined called for {player}");

        if (Runner.IsServer || Runner.IsSharedModeMasterClient)
        {
            var playerObject = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player);
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
}

