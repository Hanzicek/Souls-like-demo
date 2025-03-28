using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponTip; // Assign to sword tip transform
    public float hitRadius = 0.3f;
    public LayerMask enemyLayer;
    public int damage = 250;

    [Header("Detection Settings")]
    public float activeHitTime = 0.2f; // How long hitbox stays active
    private float _lastHitTime;
    private bool _isHitActive;

    [Header("Effects")]
    public GameObject slashEffect;
    public TrailRenderer weaponTrail;

    void Update()
    {
        // Visual feedback when hit detection is active
        if(_isHitActive && weaponTrail != null && !weaponTrail.emitting)
        {
            weaponTrail.emitting = true;
        }
        else if(!_isHitActive && weaponTrail != null && weaponTrail.emitting)
        {
            weaponTrail.emitting = false;
        }

        // Disable hit detection after duration
        if(Time.time > _lastHitTime + activeHitTime)
        {
            _isHitActive = false;
        }
    }

    // Called via Animation Event at the exact frame of impact
    public void ActivateHitDetection()
    {
        _isHitActive = true;
        _lastHitTime = Time.time;
        
        if(slashEffect != null)
            Instantiate(slashEffect, weaponTip.position, weaponTip.rotation);
        
        CheckForHits();
    }

    void CheckForHits()
    {
        Collider[] hits = Physics.OverlapSphere(weaponTip.position, hitRadius, enemyLayer);
        
        foreach(Collider hit in hits)
        {
            if(hit.TryGetComponent(out BossHealth boss))
            {
                boss.TakeDamage(damage);
                Debug.Log($"Hit at {weaponTip.position} at time {Time.time}");
            }
        }
    }

    // Visualize hit radius in editor
    void OnDrawGizmosSelected()
    {
        if(weaponTip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(weaponTip.position, hitRadius);
        }
    }
}