using System;
using UnityEngine;

public class Slider : MonoBehaviour
{
    [Header("Referances")]
    public Transform orientation;
    public Transform playerobj;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")] 
    public float maxSlideTime;
    //public float slideForce;
    public float slideBoostForce ; 
    public float frictionCompensation ;
    private float slideTimer;


    public float slideYscale;
    private float startYscale;
    

[Header("Input")]
public KeyCode slideKey = KeyCode.LeftControl;
private float horizontalInput;
private float verticalInput;




private void Start()
{
    rb = GetComponent<Rigidbody>();
    pm = GetComponent<PlayerMovement>();
    startYscale = playerobj.localScale.y;
}

private void Update()
{
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");
    
    
    
    bool isSprintKeyDown = UnityEngine.Input.GetKey(pm.sprintKey);
   
    bool isGrounded = (pm.state != PlayerMovement.MovementState.air);
    
    bool isMoving = (horizontalInput != 0 || verticalInput != 0);

    if (Input.GetKeyDown(slideKey) && isMoving && isGrounded && isSprintKeyDown)
    {
       
        StartSlide();
    }
    
   

}

private void StartSlide()
{
    pm.sliding = true;
    playerobj.localScale = new Vector3(playerobj.localScale.x, slideYscale, playerobj.localScale.z);
    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    slideTimer = maxSlideTime;
    //Mevcut hızı al ve üzerine anlık kuvvet (Boost) uygula
    Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    
    Vector3 slideDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    
    if (rb.linearVelocity.y > 0)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }
    rb.AddForce(slideDirection.normalized * slideBoostForce, ForceMode.Impulse); 


}

private void SlideMovement()
{
    Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        slideTimer -= Time.deltaTime;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        if (slideTimer <= 0 || flatVel.magnitude < pm.walkSpeed) 
        {
            StopSlide();
            return; 
        }
       // Bu kuvvet, slideForce'un eski rolünü üstlenir, ancak artık hız artırmaz, sadece yavaşlamayı yavaşlatır.
            Vector3 slideDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
 
  

    if (!pm.OnSlope()|| rb.linearVelocity.y> -0.1f)
    {
        rb.AddForce(pm.GetSlopeMoveDirection(slideDirection) * 40f, ForceMode.Force); // Hafif yamaç aşağı itme
    }
        
    
    else
    {
        rb.AddForce(slideDirection.normalized * frictionCompensation, ForceMode.Force); 
    }
    

   
}

private void FixedUpdate()
{
    if (pm.sliding)
    {
        SlideMovement();
    }
}

private void StopSlide()
{
    playerobj.localScale = new Vector3(playerobj.localScale.x, startYscale, playerobj.localScale.z);
    pm.sliding = false;
}
}
