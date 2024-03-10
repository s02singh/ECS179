using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{   
    public GameObject bow;
    public GameObject player;
    public float horizontalSpeed = 25f;
    public float verticalSpeed = 3f;

    public void Launch(GameObject projectile)
    {
        player = GameObject.Find("PlayerArmature");
        // Get the position of the bow
        Vector3 startingPosition = bow.transform.position;

        // Get the direction from the starting position to the player
        Vector3 direction = (player.transform.position - startingPosition).normalized;

        // Set rotation of projectile
        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

        // Instantiate the projectile at the starting position with proper rotation
        Rigidbody rb = Instantiate(projectile, startingPosition, rotation).GetComponent<Rigidbody>();

        // Add forces to the projectile
        rb.AddForce(direction * horizontalSpeed, ForceMode.Impulse);
        rb.AddForce(transform.up * verticalSpeed, ForceMode.Impulse);

        // Destroy the projectile after a delay
        Destroy(rb.gameObject, 2f);
    }
}
