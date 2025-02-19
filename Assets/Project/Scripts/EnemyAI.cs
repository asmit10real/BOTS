using System.Linq;
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

    [Networked] private Vector3 NetworkedPosition { get; set; } = Vector3.zero;
    [Networked] private int UniqueEnemyId { get; set; } // ✅ Networked Unique ID

    public override void Spawned() {
        if (!Object.HasStateAuthority) return;

        controller = GetComponent<CharacterController>();

        UniqueEnemyId = (int)Object.Id.Raw;

        NetworkObject netObj = GetComponent<NetworkObject>();

        // ✅ Ensure NetworkObject is properly referenced before using it
        if (netObj != null) {
            transform.position = netObj.transform.position;
        } else {
            Debug.LogError($"[EnemyAI] ERROR: NetworkObject is NULL for Enemy {Object.Id}");
        }

        Debug.Log($"[EnemyAI] Enemy {UniqueEnemyId} final position at Spawned: {transform.position}");
    }






    void FixedUpdate() {
        if (!Object.HasStateAuthority) return;

        ApplyGravity();

        if (target == null) {
            Debug.Log($"[EnemyAI] Enemy {Object.Id} has no target, searching...");
            FindClosestPlayer(); // ✅ Ensure the enemy keeps searching for a player
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
            Invoke(nameof(FindClosestPlayer), 1f); // Retry every second
            return;
        }

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var player in players) {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < closestDistance) {
                closest = player.transform;
                closestDistance = dist;
            }
        }

        if (closest != null) {
            target = closest;
            Debug.Log($"[EnemyAI] Enemy {Object.Id} found target: {target.position}");
        } else {
            Invoke(nameof(FindClosestPlayer), 1f);
        }
    }


    private void MoveTowardsTarget() {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Runner.DeltaTime;

        // Keep enemy facing the player
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        controller.Move(move);
    }

    private void ApplyGravity() {
        if (controller.isGrounded) {
            velocity.y = -2f;
        } else {
            velocity.y += gravity * Runner.DeltaTime;
        }

        controller.Move(velocity * Runner.DeltaTime);
    }
}
