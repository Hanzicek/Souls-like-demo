using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 500; // Set boss max HP
    private int currentHealth;

    public Slider healthBar; // Drag the UI Slider here in the Inspector

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth; // Normalize health (0-1)
        }
    }

    void Die()
    {
        Debug.Log("Boss Defeated!");
        // Add death animation or boss destruction here
        Destroy(gameObject, 2f); // Remove boss after 2 seconds
    }
}
