using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChancer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Eğer çarpan nesnenin etiketi (Tag) "Player" ise
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
