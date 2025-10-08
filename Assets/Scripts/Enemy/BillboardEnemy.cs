using UnityEngine;

public class BillboardEnemy : MonoBehaviour
{
    [Header("Camera to Face")]
    public Transform targetCamera; // Düşmanın bakacağı kamera

    void Start()
    {
        // Playerın kamerasını seçiyoruz
        if (targetCamera == null)
            targetCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Sadece Y ekseninde dön
        Vector3 lookDir = targetCamera.position - transform.position;
        lookDir.y = 0; // Y eksenini durdurduk
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}