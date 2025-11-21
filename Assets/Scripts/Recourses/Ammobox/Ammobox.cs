using UnityEngine;

public class Ammobox : MonoBehaviour
{
    public AmmoTextEffect ammoEffect;
    public GameObject[] weaponsToRefill;
    public int ammoAmount = 30;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool ammoAdded = false;

            foreach (GameObject weapon in weaponsToRefill)
            {
                if (weapon != null)
                {
                    Pistol pistol = weapon.GetComponent<Pistol>();
                    if (pistol != null)
                    {
                        if (pistol.AddAmmo(ammoAmount))
                            ammoAdded = true;
                    }
                }
            }


            if (ammoAdded)
            {
                if (ammoEffect != null)
                    ammoEffect.PlayEffect();
                Destroy(gameObject);
            }
        }
    }
}
