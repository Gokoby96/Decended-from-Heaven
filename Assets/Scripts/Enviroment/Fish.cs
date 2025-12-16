using UnityEngine;

public class Fish : MonoBehaviour
{
   [HideInInspector] public FishManager manager;
  
      public float speed = 2f;
      public float turnSpeed = 2f;
      public float wanderRadius = 5f;
  
      Vector3 targetPosition;
  
      void Start()
      {
          PickNewTarget();
      }
  
      void Update()
      {
           if (Time.frameCount % 3 != 0) return;
           if (Vector3.Distance(Camera.main.transform.position, transform.position) > 30f)
               return;
          Move();
          CheckDistance();
      }
  
      void Move()
      {
          Vector3 direction = targetPosition - transform.position;
  
          if (direction != Vector3.zero)
          {
              Quaternion targetRotation = Quaternion.LookRotation(direction);
              transform.rotation = Quaternion.Slerp(
                  transform.rotation,
                  targetRotation,
                  turnSpeed * Time.deltaTime
              );
          }
  
          transform.Translate(Vector3.forward * speed * Time.deltaTime);
      }
  
      void CheckDistance()
      {
          float dist = Vector3.Distance(transform.position, targetPosition);
          if (dist < 1.5f)
          {
              PickNewTarget();
          }
      }
  
      void PickNewTarget()
      {
          Vector3 randomOffset = Random.insideUnitSphere * wanderRadius;
          Vector3 centerOffset = (manager.transform.position - transform.position).normalized * manager.centerForce;
  
          targetPosition = transform.position + randomOffset + centerOffset;
  
          float distanceFromCenter = Vector3.Distance(targetPosition, manager.transform.position);
  
          if (distanceFromCenter > manager.swimRadius)
          {
              targetPosition = manager.transform.position +
                               (targetPosition - manager.transform.position).normalized * manager.swimRadius;
          }
      }
}
