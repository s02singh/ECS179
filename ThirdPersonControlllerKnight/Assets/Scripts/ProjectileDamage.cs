using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Hit player!");
            Destroy(this.gameObject);
            other.GetComponent<PlayerController>()?.TakeDamage(20); // NEED DAMAGE ENGINE
        }
    }
}
