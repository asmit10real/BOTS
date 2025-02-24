using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int basicAttackDamage = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Make sure enemies have the "Enemy" tag!
        {
            Debug.Log($"[AttackHitbox] Hit enemy: {other.gameObject.name}");
            
            // Apply damage (if you have an EnemyHealth script)
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(basicAttackDamage); // Adjust damage value as needed
            }
        }
    }
}
