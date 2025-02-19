using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {
    public bool IsKeyEnemy = false; // Determines if this enemy is needed to open the next gate

    private void OnDrawGizmos() {
        Gizmos.color = IsKeyEnemy ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
