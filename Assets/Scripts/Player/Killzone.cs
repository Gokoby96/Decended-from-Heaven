using UnityEngine;

public class Killzone : MonoBehaviour
{
    public PlayerHealth playerHealth; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth.TakeDamage(9999); 
        }
    }
}
    

