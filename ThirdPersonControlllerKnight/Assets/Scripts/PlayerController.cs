using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Third Person Controller References
    [SerializeField]
    private Animator playerAnim;

   
    //Equip-Unequip parameters
    [SerializeField]
    private GameObject sword;
    [SerializeField]
    private GameObject swordOnShoulder;
    public bool isEquipping;
    public bool isEquipped;

    private Vector3 dodgeDirection;


    //Blocking Parameters
    public bool isBlocking;

    //Kick Parameters
    public bool isKicking;

    //Attack Parameters
    public bool isAttacking;
    private float timeSinceAttack;
    public int currentAttack = 0;
    public float shieldDelay = 1f;
    public float timeToBlock = 0f;

    [SerializeField]
    LineRenderer lineRenderer;




    private void Update()
    {
        timeSinceAttack += Time.deltaTime;

        // Call all mechanic functions
        Attack();
        HeavyAttack();
        Dodge();
        Ultimate();
        Equip();
        Block();
        Kick();
    }

    // PRESS R TO EQUIP
    private void Equip()
    {
        if (Input.GetKeyDown(KeyCode.R) && playerAnim.GetBool("Grounded"))
        {
            isEquipping = true;
            playerAnim.SetTrigger("Equip");
        }
    }

    // PRESS U TO ULTIMATE ABILITY
    private void Ultimate()
    {
        if (Input.GetKeyDown(KeyCode.U) && playerAnim.GetBool("Grounded"))
        {
            if (!isEquipped)
                return;
            isAttacking = true;
            playerAnim.SetTrigger("Ultimate");
            
        }
    }

    // CALLED BY ULTIMATE ANIMATION
    public void LightningStorm()
    {
        // Made a triangle
        Vector3 swordPosition = sword.transform.position;
        Vector3 attackDirection = sword.transform.forward;

        Vector3 leftPoint = swordPosition + attackDirection * 5f - sword.transform.right * 5f;  
        Vector3 rightPoint = swordPosition + attackDirection * 5f + sword.transform.right * 5f;
        Vector3 center = swordPosition;

        

        // Check for collisions within the box. need to add triangle logic later
        Collider[] hitColliders = Physics.OverlapBox(center, new Vector3(5f, 5f, 5f)); 
        
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, swordPosition);
        lineRenderer.SetPosition(1, leftPoint);
        lineRenderer.SetPosition(2, rightPoint);
        lineRenderer.SetPosition(3, center);

        lineRenderer.startWidth = .1f;
        lineRenderer.endWidth = .05f;
        lineRenderer.enabled = true;

        foreach (Collider collider in hitColliders)
        {
            Debug.Log("Hit something: " + collider.gameObject.name);
            if (collider.gameObject.CompareTag("Enemy"))
            {
              
                Enemy enemy = collider.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // WE NEED TO CHANGE THIS ONCE WE FIGURE OUT DAMAGE ENGINE
                    enemy.TakeDamage(100);

                }
            }
        }
    }
    

    // Called by Equip animation
    public void ActiveWeapon()
    {
        if (!isEquipped)
        {
            Debug.Log("here");
            sword.SetActive(true);
            swordOnShoulder.SetActive(false);
            isEquipped = !isEquipped;
        }
        else
        {
            sword.SetActive(false);
            swordOnShoulder.SetActive(true);
            isEquipped = !isEquipped;
        }
    }

    // HELPER FUNCTION
    public void Equipped()
    {
        isEquipping = false;
    }

    // HOLD DOUBLE CLICK/RIGHT CLICK TO BLOCK
    private void Block()
    {
        if (Time.time < timeToBlock)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Mouse1) && playerAnim.GetBool("Grounded"))
        {
            playerAnim.SetBool("Block", true);
            isBlocking = true;
        }
        else
        {
            playerAnim.SetBool("Block", false);
            isBlocking = false;
        }
    }

    // IGNORE NOT FINISHED
    private void Dodge()
    {
        // Check for dodge input
        if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
        {
            // Dodge left
            if (Input.GetKeyDown(KeyCode.A))
            {

            }
            // Dodge right
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //DodgeRight();
            }
            // Dodge back
            else if (Input.GetKeyDown(KeyCode.S))
            {
                playerAnim.SetTrigger("DodgeBack");

            }
        }
    }

  

    // HOLD CTRL TO KICK
    public void Kick()
    {
        if (Input.GetKey(KeyCode.LeftControl) && playerAnim.GetBool("Grounded"))
        {
            playerAnim.SetBool("Kick", true);
            isKicking = true;
        }
        else
        {
            playerAnim.SetBool("Kick", false);
            isKicking = false;
        }
    }

    // PRESS K FOR LIGHT ATTACKS. 3 PIECE COMBO
    private void Attack()
    {

        if (Input.GetKey(KeyCode.K) && playerAnim.GetBool("Grounded") && timeSinceAttack > 0.8f)
        {
            if (!isEquipped)
                return;

            currentAttack++;
            isAttacking = true;

            if (currentAttack > 3)
                currentAttack = 1;

            //Reset
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            //Call Attack Triggers
            playerAnim.SetTrigger("Attack" + currentAttack);
        

            //Reset Timer
            timeSinceAttack = 0;
        }


    }

    // HOLD J FOR HEAVY ATTACK. PRESS J AT THE END OF LIGHT COMBO FOR HEAVY COMBO
    private void HeavyAttack()
    {
        if (!isEquipped)
            return;
        
        if (Input.GetKey(KeyCode.J) && playerAnim.GetBool("Grounded"))
        {
            if (currentAttack == 3)
            {
                currentAttack = 1;
                playerAnim.SetTrigger("HeavyCombo");
                timeSinceAttack = 0;
            }
            else
            {
                Debug.Log("im pressed");
                playerAnim.SetBool("HeavyAttack1", true);
            }
        }
        else
        {
            Debug.Log("not pressed");
            playerAnim.SetBool("HeavyAttack1", false);
        }
    }


    // CALLED BY EACH ATTACK ANIMATION
    public void RaycastAttack()
    {
        
        if (!isEquipped)
        {
            return;
        }

        // Get sword position and attack direction
        Vector3 swordPosition = sword.transform.position;
        Vector3 attackDirection = sword.transform.forward;

        // Perform raycast
        RaycastHit hit;
        if (Physics.Raycast(swordPosition, attackDirection, out hit, 15))
        {
            Debug.DrawRay(swordPosition, attackDirection * 50, Color.red, 3f);

            Debug.Log("Hit something: " + hit.collider.gameObject.name);
            lineRenderer.SetPosition(0, swordPosition);
            lineRenderer.SetPosition(1, hit.point);

      
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.05f;
            

            // I added this line renderer to visualize
            lineRenderer.enabled = true;


            // Check if enemy hit
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                // Damage logic
                Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20); // WE NEED A DAMAGE ENGINE
                }
                else
                {
                    
                    Debug.LogWarning("WENT WRONG");
                }
            }
        }
    }

    // NO CHARACTER HEALTH YET. JUST ANIMATION
    public void TakeDamage(int damage)
    {
        if (!isBlocking)
        {
            playerAnim.SetTrigger("Hit");
        }
        else
        {
            Debug.Log("blocked");
            playerAnim.SetBool("Block", false);
            isBlocking = false;
            playerAnim.SetTrigger("BlockImpact");
            timeToBlock = Time.time + shieldDelay;
        }
      
    }


    // AT THE END OF ATTACKS. IGNORE THIS FOR NOW
    public void ResetAttack()
    {
        isAttacking = false;
    } 
}   
