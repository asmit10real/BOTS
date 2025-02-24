using UnityEngine;
using Fusion;

public class PlayerCombat : NetworkBehaviour {
    public GameObject AttackHitbox; // Assigned in Inspector
    private Animator _animator;
    private bool _isAttacking = false;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    void Update() {
        if (!Object.HasInputAuthority) return;

        if (Input.GetKeyDown(KeyCode.V) && !_isAttacking) {
            Debug.Log("[PlayerCombat] V key pressed! Attempting to attack...");
            _isAttacking = true;
            RPC_PerformAttack();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_PerformAttack() {
        Debug.Log("[PlayerCombat] Attack triggered on all clients!");
        _isAttacking = true;
        _animator.SetTrigger("Attack");
    }

    // 🎯 **Called by Animation Event when the attack should hit**
    public void EnableHitbox() {
        if (!Runner.IsServer) return;

        Debug.Log("[PlayerCombat] Hitbox activated!");
        AttackHitbox.SetActive(true);
    }

    // 🎯 **Called by Animation Event at the end of the attack**
    public void DisableHitbox() {
        Debug.Log("[PlayerCombat] Hitbox disabled!");
        AttackHitbox.SetActive(false);
        _isAttacking = false; // Reset attack state
    }
}
