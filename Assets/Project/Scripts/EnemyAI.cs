using System.Collections;
using UnityEngine;
using Fusion;

public class EnemyAI : NetworkBehaviour {
    public float moveSpeed = 1.5f;
    public float attackRange = 1.5f;
    public float detectionRadius = 10f;
    public Transform target;
    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private NetworkTransform netTransform;
    private bool canMove = false;

    [Networked] private Vector3 NetworkedPosition { get; set; } = Vector3.zero;
    [Networked] private int UniqueEnemyId { get; set; } // ✅ Networked Unique ID

    public override void Spawned() {
        if (!Object.HasStateAuthority) return;

        controller = GetComponent<CharacterController>();
        netTransform = GetComponent<NetworkTransform>();

        if (netTransform == null) {
            Debug.LogError($"[EnemyAI] ERROR: NetworkTransform missing on {Object.Id}");
            return;
        }

        Debug.Log($"[EnemyAI] Enemy {Object.Id} spawned at {transform.position} (Synced via NetworkTransform)");

        StartCoroutine(StartMovementAfterDelay(3f));
    }

    private IEnumerator StartMovementAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        canMove = true;
        Debug.Log($"[EnemyAI] Enemy {Object.Id} is now moving move speed: {moveSpeed}");
    }

    // ✅ MOVE MOVEMENT TO `FixedUpdateNetwork()`
    public override void FixedUpdateNetwork() {
        if (Object.HasStateAuthority & !canMove) {
            ApplyGravity();
        }
        if (!Object.HasStateAuthority || !canMove) return;

        ApplyGravity();

        if (target == null) {
            Debug.Log($"[EnemyAI] Enemy {Object.Id} has no target, searching...");
            FindClosestPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange + 0.5f) {
            MoveTowardsTarget();
        }

        // ✅ Ensure position is updated in the network
        NetworkedPosition = transform.position;
        Debug.Log($"[EnemyAI] Enemy {Object.Id} moving towards {target.position}, New Position: {NetworkedPosition}");
    }

    public override void Render() {
        if (!Object.HasStateAuthority) {
            transform.position = Vector3.Lerp(transform.position, NetworkedPosition, 0.1f);
            Debug.Log($"[EnemyAI] Enemy {Object.Id} rendering at {transform.position}");
        }
    }

    private void FindClosestPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0) {
            Debug.Log($"[EnemyAI] No players found! Retrying in 1 second...");
            Invoke(nameof(FindClosestPlayer), 1f);
            return;
        }

        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 bestPosition = Vector3.zero; // ✅ Track individual best move position

        foreach (var player in players) {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closestDistance) {
                closest = player.transform;
                closestDistance = dist;

                // ✅ Assign unique movement goal slightly offset per enemy
                bestPosition = player.transform.position + new Vector3(UniqueEnemyId * 0.2f, 0, UniqueEnemyId * -0.2f);
            }
        }

        if (closest != null) {
            target = closest;
            Debug.Log($"[EnemyAI] Enemy {Object.Id} found target: {target.position}, moving to adjusted {bestPosition}");
        } else {
            Invoke(nameof(FindClosestPlayer), 1f);
        }
    }


    private void MoveTowardsTarget() {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;

        // ✅ Ensure each enemy moves slightly offset from others
        float offset = UniqueEnemyId * 0.1f;  // Tiny per-enemy offset
        direction += new Vector3(offset, 0, -offset); // Slight deviation to prevent stacking

        Vector3 move = direction * moveSpeed * Runner.DeltaTime;

        // ✅ Prevent enemies from fully overlapping by adding randomness to movement
        move.x += Random.Range(-0.05f, 0.05f);
        move.z += Random.Range(-0.05f, 0.05f);

        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        controller.Move(move);
    }


    private void ApplyGravity() {
        if (controller.isGrounded) {
            velocity.y = -0.1f; // ✅ Tiny downward force to ensure grounded state
        } else {
            velocity.y += gravity * Runner.DeltaTime;
        }

        // ✅ Preserve the X/Z position while only modifying Y movement
        Vector3 currentPos = transform.position;
        Vector3 gravityMove = new Vector3(0, velocity.y, 0) * Runner.DeltaTime;

        controller.Move(gravityMove);

        // ✅ Ensure gravity never affects X and Z
        transform.position = new Vector3(currentPos.x, transform.position.y, currentPos.z);
    }


}
