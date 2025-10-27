using System;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class Grapling : MonoBehaviour
{
   [Header("References")] 
   public PlayerMovement pm;
   public Transform cam;
   public Transform gunTip;
   public LayerMask whatIsGrappableable;
   public LineRenderer lr;
   
   [Header("Grappling")]
   public float maxGrappleDistance;
   public float grappleDelayTime;
   private Vector3 grapplePoint;
   public float overshootYAxis;

   [Header("Cooldown")] 
   public float grapplingCD;
   private float grapplingCDTimer;

   [Header("Input")] 
   public KeyCode grappleKey = KeyCode.Mouse1;

   private bool grappling;

   private void Start()
   {
      
   }

   private void Update()
   {
      if (Input.GetKeyDown(grappleKey))
      {
         StartGrapple();
      }

      if (grapplingCDTimer > 0)
      {
         grapplingCDTimer -= Time.deltaTime;
      }
   }

   private void StartGrapple()
   {
      if (grapplingCDTimer > 0) 
      {
         return;
      }
      grappling = true;
      pm.freeze = true;
      RaycastHit hit;
      if (Physics.Raycast(cam.position,cam.forward,out hit, maxGrappleDistance, whatIsGrappableable))
      {
         grapplePoint = hit.point;
         Invoke(nameof(ExecuteGrapple),grappleDelayTime);
      }
      else
      {
         grapplePoint = cam.position + cam.forward * maxGrappleDistance;
         Invoke(nameof(StopGrapple),grappleDelayTime);
      }
      lr.enabled = true;
      lr.SetPosition(1, grapplePoint);
   }

   private void LateUpdate()
   {
      if (grappling)
      {
         lr.SetPosition(0,gunTip.position);
      }
   }

   private void ExecuteGrapple()
   {
      pm.freeze = false;
      Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

      float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
      float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

      if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

      pm.JumpToPosition(grapplePoint, highestPointOnArc);

      Invoke(nameof(StopGrapple), 1f);
   }
   public bool IsGrappling()
   {
      return grappling;
   }

   public Vector3 GetGrapplePoint()
   {
      return grapplePoint;
   }

   public void StopGrapple()
   {
      pm.freeze = false;
      grappling = false;
      pm.activeGrapple = false; 
      grapplingCDTimer = grapplingCD;
      
      lr.enabled = false;
   }








}
