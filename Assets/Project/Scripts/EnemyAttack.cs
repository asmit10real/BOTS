using UnityEngine;
using Fusion;

public class EnemyAttack : NetworkBehaviour {
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;
    private EnemyAI enemyAI;

    public Animator animator;

    private void Start() {
        enemyAI = GetComponent<EnemyAI>();
    }

    void FixedUpdate() {
        if (!Object.HasStateAuthority || enemyAI.target == null) return;

        float distance = Vector3.Distance(transform.position, enemyAI.target.position);
        if (distance <= enemyAI.attackRange && Time.time - lastAttackTime >= attackCooldown) {
            lastAttackTime = Time.time;
            RPC_PerformAttack();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PerformAttack() {
        // ✅ Prevents enemies from attacking other enemies
        if (enemyAI.target.CompareTag("Player") && enemyAI.target.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)) {
            playerHealth.TakeDamage(attackDamage);
        }
        
        if (animator != null) {
            animator.SetTrigger("Attack");
        }
    }
}
