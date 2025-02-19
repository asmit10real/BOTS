using UnityEngine;
using Fusion;

public class PlayerCombat : NetworkBehaviour {
    public float attackRange = 2f;
    public int attackDamage = 25;
    public LayerMask enemyLayer;
    private bool attackPressed = false;

    public override void Spawned() {
        Debug.Log($"PlayerCombat Spawned. HasInputAuthority: {Object.HasInputAuthority}");
    }

    void Update() {
        //Debug.Log("PlayerCombat Update Running...");
        if (Input.GetMouseButtonDown(0)) { // Left-click to attack
            Debug.Log("Mouse Click Detected!");
            attackPressed = true;
        }
    }

    public override void FixedUpdateNetwork() {
        Debug.Log("FixedUpdateNetwork is Running...");
        
        if (!Object.HasInputAuthority) {
            Debug.Log("No Input Authority. Skipping attack logic.");
            return; // Only local player should trigger attacks
        }

        if (attackPressed) {
            attackPressed = false;
            Debug.Log("Attack Performed!");
            PerformAttack();
        }
    }


    private void PerformAttack() {
        if (Runner.IsServer) {
            Debug.Log("Server executing attack...");
            DoDamage();
        } else {
            Debug.Log("Client requesting attack from server...");
            RPC_PerformAttack(); // Request attack execution from the server
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_PerformAttack() {
        Debug.Log("Server received attack request from client.");
        DoDamage();
    }

    private void DoDamage() {
        Vector3 attackPosition = transform.position + transform.forward * attackRange * 0.5f;
        Debug.Log($"Checking for enemies in attack range at position: {attackPosition}");

        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, attackRange * 0.5f, enemyLayer);

        if (hitEnemies.Length > 0) {
            Debug.Log($"Hit {hitEnemies.Length} enemies!");
        } else {
            Debug.Log("No enemies detected.");
        }

        foreach (Collider enemy in hitEnemies) {
            if (enemy.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth)) {
                Debug.Log($"Applying {attackDamage} damage to {enemy.name}");
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 attackPosition = transform.position + transform.forward * attackRange * 0.5f;
        Gizmos.DrawWireSphere(attackPosition, attackRange * 0.5f);
    }
}
