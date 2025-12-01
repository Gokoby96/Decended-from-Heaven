using UnityEngine;

public class Key : MonoBehaviour
{
    public Door door; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.UnlockDoor();   
            Destroy(gameObject); 
        }
    }
}
