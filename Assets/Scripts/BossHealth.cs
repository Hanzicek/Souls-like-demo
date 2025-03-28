using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Starting health points")] 
    public int maxHealth = 1000;
    [Tooltip("Current health points")] 
    public int currentHealth;

    [Header("UI Settings")]
    [Tooltip("Reference to the screen-space health bar")] 
    public Slider healthSlider;
    [Tooltip("Optional health text display")] 
    public Text healthText;
    [Tooltip("How fast health bar updates (smoothness)")] 
    public float healthLerpSpeed = 5f;

    [Header("Effects")]
    public AudioClip hitSound;
    public ParticleSystem hitParticles;

    private float _targetHealth;
    private bool _isDead;

    void Start()
    {
        currentHealth = maxHealth;
        _targetHealth = maxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
        
        UpdateHealthText();
    }

    void Update()
    {
        // Smooth health bar animation
        if (healthSlider != null && Mathf.Abs(healthSlider.value - _targetHealth) > 0.1f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, _targetHealth, Time.deltaTime * healthLerpSpeed);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        _targetHealth = currentHealth;
        
        // Effects
        if (hitParticles != null)
            Instantiate(hitParticles, transform.position, Quaternion.identity);
        
        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

        // UI Update
        UpdateHealthText();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }

    void Die()
    {
        _isDead = true;
        Debug.Log("BOSS DEFEATED!");
        
        // Disable health bar
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);
        
        // Add death effects/animations here
        Destroy(gameObject, 3f);
    }

    // For testing in editor
    [ContextMenu("Test Damage")]
    public void TestTakeDamage()
    {
        TakeDamage(250);
    }
}