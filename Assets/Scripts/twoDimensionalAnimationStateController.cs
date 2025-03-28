using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twoDimensionalAnimationStateController : MonoBehaviour
{
    // Reference to the Animator component
    Animator animator;
    
    // Movement velocities
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    
    // Acceleration and deceleration values
    public float acceleration = 4.0f;
    public float deceleration = 4.0f;
    
    // Maximum velocities for walking and running
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    // Animator parameter hashes for performance optimization
    int VelocityZHash;
    int VelocityXHash;
    int attackHash;
    int dodgeHash;



    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Convert parameter names into hashes for efficiency
        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");
        attackHash = Animator.StringToHash("Attack"); 
        dodgeHash = Animator.StringToHash("DodgeRoll");

    }

    void changeVelocity(bool forwardPressed, bool backwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // Increase forward velocity up to max
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        // Increase backward velocity up to max
        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        
        // Increase left velocity up to max
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        // Increase right velocity up to max
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        
        // Gradually decrease velocity when no movement input is detected
        if (!forwardPressed && !backwardPressed && velocityZ != 0.0f)
        {
            velocityZ = Mathf.MoveTowards(velocityZ, 0, Time.deltaTime * deceleration);
        }
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    void lockOrResetVelocity(bool forwardPressed, bool backwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // Reset Z velocity if it's close to 0 and no movement is occurring
        if (!forwardPressed && !backwardPressed && Mathf.Abs(velocityZ) < 0.05f)
        {
            velocityZ = 0.0f;
        }

        // Reset X velocity if it's close to 0 and no movement is occurring
        if (!leftPressed && !rightPressed && Mathf.Abs(velocityX) < 0.05f)
        {
            velocityX = 0.0f;
        }
        
        // Limit forward velocity
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }
        
        // Limit backward velocity
        if (backwardPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        else if (backwardPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        else if (backwardPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }

        // Limit leftward velocity
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        // Limit rightward velocity
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    void attackAnimation()
    {
        // This activates the "Attack" trigger parameter in the Animator
        animator.SetTrigger(attackHash);
    }
    void dodgeRollAnimation()
    {
        animator.SetTrigger(dodgeHash);
    }
    void Update()
    {
        // Check for player input
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool isAttacking = Input.GetMouseButtonDown(0);
        bool isDodging = Input.GetKeyDown(KeyCode.LeftControl);
        // Determine current max velocity (walk or run)
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        
        // Update velocities based on player input

        
        
        if(isAttacking != true && isDodging != true)
        {
            changeVelocity(forwardPressed, backwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            lockOrResetVelocity(forwardPressed, backwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            

        }
        else if (isDodging != true)
        {
        attackAnimation();
        }
        else
        {
            dodgeRollAnimation();
            

        }
        isAttacking = false;

        // Update animator parameters
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
    }
}
