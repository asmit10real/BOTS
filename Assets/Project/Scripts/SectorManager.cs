using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Fusion;

public class SectorManager : NetworkBehaviour {
    public List<EnemySpawner> enemySpawners; // List of all spawners in this sector
    public SectorCompleteUI sectorCompleteUI;
    public EnemySpawner bossSpawner;
    

    public override void Spawned() {
        if (Object.HasStateAuthority) {
            Debug.Log("[SectorManager] Game started, activating first enemy spawner.");
            enemySpawners[0]?.ActivateSpawner(); // ✅ Activate first area
        }
    }

    public void NotifyBossDeath() {
        if (!Object.HasStateAuthority) return;

        Debug.Log("[SectorManager] Boss defeated! Triggering Sector Clear UI.");
        
        // Activate UI on all clients
        RPC_ShowSectorClearUI();

        // Start countdown to return to menu
        StartCoroutine(ExitToMainMenu());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowSectorClearUI() {
        if (sectorCompleteUI != null) {
            sectorCompleteUI.ShowSectorClear();
        }
    }

    private IEnumerator ExitToMainMenu() {
        yield return new WaitForSeconds(7.5f);

        Debug.Log("[SectorManager] Returning to RoomLobby");

        if (Runner.IsSceneAuthority) {
            string lobbySceneName = "RoomLobby"; // Ensure this matches your scene name exactly

            // 🚀 Load the RoomLobby scene
            SceneRef sceneRef = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath($"Assets/Project/Scenes/{lobbySceneName}.unity"));
            Runner.LoadScene(sceneRef, LoadSceneMode.Single);
        }
    }

}

