using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public HealFlash healFlash; 
    public GameObject[] HealthPlayer;
    public int HealAmount = 30;
    public GameObject healEffectPrefab;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool HealAdded = false;

            foreach (GameObject player in HealthPlayer)
            {
                if (player != null)
                {
                    PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth!= null)
                    {
                        if (playerHealth.Heal(HealAmount))
                        {
                            HealAdded = true;
                        }
                            
                    }
                }
            }


            if (HealAdded)
            {
                if (healFlash != null)
                    healFlash.Flash();

                Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}