using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 1.5f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public int attackDamage = 25;
    private float lastAttackTime;
    public bool isAttacking = false;
    private bool hasHit = false;

    // Animation hashes
    private int walkAnimHash;
    private int attackAnimHash;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Set up animation hashes
        walkAnimHash = Animator.StringToHash("Walk");
        attackAnimHash = Animator.StringToHash("Attack");

        // Configure NavMeshAgent (EXACTLY AS YOU HAD IT)
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.angularSpeed = 0; // Manual rotation as per your original
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check attack animation state (UNCHANGED)
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        isAttacking = stateInfo.IsName("Attack");

        if (isAttacking)
        {
            // Freeze movement and rotation during attack (UNCHANGED)
            agent.isStopped = true;
            return;
        }

        // Attack if in range and cooldown is ready (UNCHANGED LOGIC)
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else if (distanceToPlayer > stoppingDistance)
        {
            // Move toward player (EXACTLY AS YOU HAD IT)
            MoveToPlayer();
        }
        else
        {
            // Close enough but not attacking (UNCHANGED)
            FacePlayer();
            StopMoving();
        }
    }

    // MOVEMENT METHODS (ALL UNCHANGED FROM YOUR ORIGINAL)
    void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        FacePlayer();
        animator.SetBool(walkAnimHash, true);
    }

    void StopMoving()
    {
        agent.isStopped = true;
        animator.SetBool(walkAnimHash, false);
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void Attack()
    {
        // Face player before attacking (UNCHANGED)
        FacePlayer();

        // Trigger attack (UNCHANGED)
        lastAttackTime = Time.time;
        animator.SetTrigger(attackAnimHash);
        hasHit = false; // Reset hit flag

        // Immediately stop movement (UNCHANGED)
        StopMoving();
    }

    // NEW: Damage implementation only
    public void DealDamage()
    {
        if (hasHit || player == null) return;

        // Check if player is still in range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange * 1.2f) // Slightly forgiving range
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                hasHit = true;
                Debug.Log($"Dealt {attackDamage} damage to player!");
            }
        }
    }
}