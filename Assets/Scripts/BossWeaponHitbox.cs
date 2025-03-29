using UnityEngine;

public class BossWeaponHitbox : MonoBehaviour
{
    // Reference to the main enemy script (assign in Inspector)
    public EnemyAI enemyAI; 
    
    void OnTriggerEnter(Collider other)
    {
        // Null checks for safety
        if (enemyAI == null) return;
        
        // Only damage player during attack frames
        if (enemyAI.isAttacking && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyAI.attackDamage);
                Debug.Log($"Hit player for {enemyAI.attackDamage} damage!");
            }
        }
    }
}