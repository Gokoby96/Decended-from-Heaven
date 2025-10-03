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
    public float slideForce;
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
    horizontalInput = Input.GetAxis("Horizontal");
    verticalInput = Input.GetAxis("Vertical");

    if (Input.GetKeyDown(slideKey)&& (horizontalInput != 0 || verticalInput != 0))
    {
        StartSlide();
    }

    if (Input.GetKeyUp(slideKey)&& pm.sliding)
    {
        StopSlide();
    }
}

private void StartSlide()
{
    pm.sliding = true;
    playerobj.localScale = new Vector3(playerobj.localScale.x, slideYscale, playerobj.localScale.z);
    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    slideTimer = maxSlideTime;
}

private void SlideMovement()
{
    Vector3 inputDirection = orientation.forward* verticalInput + orientation.right * horizontalInput;
 
  

    if (!pm.OnSlope()|| rb.linearVelocity.y> -0.1f)
    {
        rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
        slideTimer -= Time.deltaTime;
    }
    else
    {
        rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
    }

    if (slideTimer<=0)
    {
        StopSlide();
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
