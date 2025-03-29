using UnityEngine;
using UnityEngine.UI; // Required for UI

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider; // Assign this in Inspector
    public bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died!");
        // Add death logic here
    }
    /*if (isDead == true)
    {

    }*/
}