using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public Transform target;        // Oyuncunun Transform'u
    public float moveSpeed ;    // Düşmanın hareket hızı
    public float stopDistance ; // Oyuncuya yaklaştığında duracağı mesafe

    [Header("Dönme Ayarları")]
    public bool faceCamera = true;  // Kameraya dönsün mü?
    public float rotationSpeed ;

    void Update()
    {
        if (target == null)
            return;

        // Hedefe doğru mesafeyi hesaplar
        float distance = Vector3.Distance(transform.position, target.position);

        // Hedefe yaklaşma
        if (distance > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // Kameraya bakma 
        if (faceCamera && Camera.main != null)
        {
            Vector3 lookDir = Camera.main.transform.position - transform.position;
            lookDir.y = 0; // Sadece yatay eksende dön
            Quaternion rot = Quaternion.LookRotation(-lookDir); // -lookDir = sprite'ın önü
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
        }
    }
}

