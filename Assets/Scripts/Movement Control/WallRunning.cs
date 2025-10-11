using System;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    public bool wallrunning;
    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;
    

[Header("Detection")]
public float wallCheckDistance;
public float minJumpHeight;
public RaycastHit leftWallhit;
public RaycastHit rightWallhit;
public bool wallLeft;
public bool wallRight;

[Header("References")]
public Transform orientation;
private PlayerMovement pm;
private Rigidbody rb;
[Header("Input")]
public KeyCode upwardsRunKey = KeyCode.LeftShift;
public KeyCode downwardsRunKey = KeyCode.LeftControl;
private bool upwardsRunning;
private bool downwardsRunning;
private float horizontalInput;
private float verticalInput;
public float wallClimbSpeed;

[Header("Wall Jump Settings")]
public KeyCode wallJumpKey = KeyCode.Space;
public float wallJumpUpForce ;
public float wallJumpSideForce ;

private void Start()
{
    rb = GetComponent<Rigidbody>();
    pm = GetComponent<PlayerMovement>();
    
}

private void Update()
{
    CheckForWall();
    StateMachine();
    
    Wallruncheck();
}

private void Wallruncheck()
{
    if (pm.wallrunning && Input.GetKeyDown(wallJumpKey))
    {
        WallJump();
    }
}

private void FixedUpdate()
{
    if (pm.wallrunning)
    {
        WallRunningMovement();
    }
}

private void CheckForWall()
{
    wallRight = Physics.Raycast(transform.position, orientation.right,out rightWallhit,wallCheckDistance, whatIsWall);
    wallLeft =  Physics.Raycast(transform.position, -orientation.right,out leftWallhit,wallCheckDistance, whatIsWall);

}

private bool AboveGround()
{
    return !Physics.Raycast(transform.position,Vector3.down,minJumpHeight, whatIsGround);
}



private void StateMachine()
{
    horizontalInput = Input.GetAxis("Horizontal");
    verticalInput = Input.GetAxis("Vertical");
    
    upwardsRunning = Input.GetKey(upwardsRunKey);
    downwardsRunning = Input.GetKey(downwardsRunKey);

    if ((wallLeft || wallRight) && verticalInput>0 && AboveGround())
    {
        if (!pm.wallrunning)
        {
            StartWallRun();
        }
    }
    else
    {
        if (pm.wallrunning)
        {
            StopWallRun();
        }
    }
}

private void StartWallRun()
{
   pm.wallrunning = true; 
}

private void WallRunningMovement()
{
    rb.useGravity = false;
    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
    Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

    if ((orientation.forward-wallForward).magnitude> (orientation.forward - - wallForward).magnitude)
    {
        wallForward = -wallForward;
    }
    rb.AddForce(wallForward*wallRunForce, ForceMode.Force);
    
    if (upwardsRunning)
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);
    if (downwardsRunning)
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);

    
    if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
    {
        rb.AddForce(-wallNormal*100, ForceMode.Force);
    }
   
}

private void StopWallRun()
{
    pm.wallrunning = false;
}
private void WallJump()
{
    Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

    
    Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);
    if ((orientation.forward - wallForward).magnitude > (orientation.forward + wallForward).magnitude)
        wallForward = -wallForward;

    
    Vector3 jumpDirection =
        (orientation.forward * 0.8f) +     
        (wallNormal * 0.4f) +              
        (Vector3.up * 0.6f);               

    jumpDirection.Normalize();

   
    Vector3 preservedVelocity = rb.linearVelocity * 0.5f;
    rb.linearVelocity = Vector3.zero;

    
    float jumpForce = wallJumpSideForce; 
    rb.AddForce((jumpDirection * jumpForce) + preservedVelocity, ForceMode.Impulse);

    StopWallRun();

   
    Invoke(nameof(ResetWallRunCooldown), 0.2f);
}

private void ResetWallRunCooldown()
{
    wallrunning = false;
}

}
