using UnityEngine;

public class LightCookieAnimator : MonoBehaviour
{
       public float speed = 1.5f;
       public float angle = 5f;
   
       private Vector3 startRotation;
   
       void Start()
       {
           startRotation = transform.eulerAngles;
       }
   
       void Update()
       {
           float x = Mathf.Sin(Time.time * speed) * angle;
           float y = Mathf.Cos(Time.time * speed * 0.8f) * angle;
   
           transform.rotation = Quaternion.Euler(
               startRotation.x + x,
               startRotation.y + y,
               startRotation.z
           );
       }

}
