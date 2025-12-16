using UnityEngine;

public class FollowCameraSoft : MonoBehaviour
{
    public Transform cam;

    void Update()
    {
        transform.position = cam.position;
    }
}
