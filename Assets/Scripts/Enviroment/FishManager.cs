using UnityEngine;

public class FishManager : MonoBehaviour
{
   public static FishManager instance;
   
      public GameObject fishPrefab;
         public int fishCount = 20;
         public float spawnRadius = 10f;
     
         [Header("Movement Settings")]
         public float swimRadius = 15f;
         public float centerForce = 1.5f;
     
         void Start()
         {
             for (int i = 0; i < fishCount; i++)
             {
                 Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
                 GameObject fish = Instantiate(fishPrefab, spawnPos, Quaternion.identity);
     
                 Fish fishScript = fish.GetComponent<Fish>();
                 fishScript.manager = this;
             }
         }
}
