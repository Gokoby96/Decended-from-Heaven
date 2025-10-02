using UnityEngine;

public class Cameramove : MonoBehaviour
{
    public Transform cameraTransform;

    private void Update()
    {
        transform.position = cameraTransform.position;
    }
}
