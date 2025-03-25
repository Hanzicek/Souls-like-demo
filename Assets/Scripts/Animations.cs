using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    
    private Animator animator;
    public float attackCooldown = 1.0f; // Cooldown in seconds
    private float lastAttackTime = 0f;  // Tracks the time of the last attack
    

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // Dodge Roll on LeftControl
        {
            // Call the function to determine which dodge animation to play
            PlayDodgeRollAnimation();
        }
         if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            PlayAttackAnimation();
        }
    }

    void PlayAttackAnimation()
    {
        // Play attack animation
        animator.CrossFade("Attack", 0.1f);
        
        // Update last attack time
        lastAttackTime = Time.time;
    }
    

    void PlayDodgeRollAnimation()
    {
        // Check the movement input to determine which dodge direction to play

        // If no movement keys are pressed, default to forward dodge
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            animator.CrossFade("DodgeRoll_FrontLeft", 0.1f);
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            animator.CrossFade("DodgeRoll_FrontRight", 0.1f);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            animator.CrossFade("DodgeRoll_BackLeft", 0.1f);
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            animator.CrossFade("DodgeRoll_BackRight", 0.1f);
        }
        else if (Input.GetKey(KeyCode.W)) // Forward movement
        {
            animator.CrossFade("DodgeRoll", 0.1f);
        }
        /*
        else if (Input.GetKey(KeyCode.LeftControl)) // Forward movement
        {
            animator.CrossFade("DodgeRoll", 0.1f);
        }
        */
        else if (Input.GetKey(KeyCode.S)) // Backward movement
        {
            animator.CrossFade("DodgeRoll_Back", 0.1f);
        }
        else if (Input.GetKey(KeyCode.A)) // Left movement
        {
            animator.CrossFade("DodgeRoll_Left", 0.1f);
        }
        else if (Input.GetKey(KeyCode.D)) // Right movement
        {
            animator.CrossFade("DodgeRoll_Right", 0.1f);
        }
    }
}
