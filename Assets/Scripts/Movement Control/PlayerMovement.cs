using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.UI;

public class PlayerMovement : MonoBehaviour
{
    // Bu script karakter kontrolü için yazılmıştır -96
    [Header("Movement")] 
    private float speed;
    public float walkSpeed;
    public float sprintspeed;
    public float slideSpeed;
    public float wallRunSpeed;
    
    
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;


    public float speedIncreaseMultıplier;
    public float slopeIncreaseMultiplier;
    
    
    [Header("Slope Handling")]
    public float maxslopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    public float groundDrag;
// Hava kontrolü için lazım olan değerler
    [Header("Ground Check")]
    public float playerheight;
    public LayerMask WhatIsGround;
    bool isGrounded;
    
    [Header("Keybinds")]
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    
    public Transform oriantation;
    //bu ise playerın movement stateini tutucak 
    public MovementState state;
  // Movement stateleri enum listesinde tutuyoruz 
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        wallRunning,
        sliding,
        air
    }

    public bool sliding;
    public bool wallrunning;
    
[Header("Crouching")]
public float crouchYScale;
public float crouchSpeed;
private float startYscale;


    float horizontalınput;
    
    private float verticalınput;
    
    Vector3 movement;
    
    private Rigidbody rb;
    
public float jumpForce;
public float jumpCooldown;
public float Airmultiplier;

bool ReadyToJump;
    private void Start()
    {
        ReadyToJump = true;
        // rigidbody eşleyip rotation durdurduk
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYscale = transform.localScale.y;
    }

    private void Update()
    {
        // player yerde mi?
        isGrounded =Physics.Raycast(transform.position, Vector3.down, playerheight* 0.5f + 0.2f, WhatIsGround);
        Input();
        StateHandler();
        SpeedControl();
        // sürükleme
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Input()
    {
        // ınput alınan yer
        horizontalınput = UnityEngine.Input.GetAxis("Horizontal");
        verticalınput = UnityEngine.Input.GetAxis("Vertical");
        if (UnityEngine.Input.GetKeyDown(jump)&& ReadyToJump&& (isGrounded|| wallrunning))
        {
            ReadyToJump = false;
            if (wallrunning)
            {
                // WallRunning scriptinden wall normalini al
                WallRunning wr = GetComponent<WallRunning>();
                Vector3 wallNormal = wr.wallRight ? wr.rightWallhit.normal : wr.leftWallhit.normal;

                // WallJump fonksiyonunu çağır
                WallJump(wallNormal);

                // Wallrun durdur
                wallrunning = false;
            }
            else
            {
                Jump();
            }

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        // crouching işlemi
        if (UnityEngine.Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x ,crouchYScale , transform.localScale.z);
            rb.AddForce(Vector3.down* 80f, ForceMode.Impulse);
        }

        if (UnityEngine.Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x ,startYscale , transform.localScale.z);
        }
    }
// bu fonksiyon player statelerini tutup hızını ayarlamamıza yarayacak
    private void StateHandler()
    {
        // wallrun state inde 
        if (wallrunning)
        {
            state = MovementState.wallRunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        // slide state inde 
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                
            }
            else
            {
                desiredMoveSpeed = sprintspeed;
            }
        }
        //crouching state inde ise
        else if (UnityEngine.Input.GetKeyDown(crouchKey))
        {
            state = MovementState.crouching;
            speed = crouchSpeed;
        }
        // sprint te ise
        if (isGrounded&& UnityEngine.Input.GetKey(sprintKey)&& verticalınput >=0 )
        {
            state = MovementState.sprinting;
            speed = sprintspeed;
        }
        // Walking state de ise
        else if (isGrounded)
        {
            state = MovementState.walking;
            speed = walkSpeed;
        }
        // air stateinde ise
        else
        {
            state = MovementState.air;
        }

        if (Mathf.Abs(desiredMoveSpeed- lastDesiredMoveSpeed) > 4f && speed !=0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyMoveSpeed());
        }
       
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - speed);
        float startValue = speed;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);
                time += Time.deltaTime* speedIncreaseMultıplier*slopeIncreaseMultiplier*slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultıplier;
            }
            
            yield return null;
        }

        speed = desiredMoveSpeed;
    }
    private void MovePlayer()
    {
       
        movement = oriantation.forward * verticalınput + oriantation.right * horizontalınput;
        if (OnSlope()&& !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(movement)*speed*20f, ForceMode.Force);
            rb.AddForce(Vector3.down*80f ,ForceMode.Force);
            
        }
        if (isGrounded)
        {
            rb.AddForce(movement.normalized*speed* 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(movement.normalized*speed* 10f* Airmultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();


    }

    private void SpeedControl()
    {
        if (OnSlope()&& !exitingSlope)
        {
            if (rb.linearVelocity.magnitude> speed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }
        Vector3 flatvel=  new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatvel.magnitude > speed)
        {
            Vector3 limitedvel = flatvel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedvel.x, rb.linearVelocity.y, limitedvel.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true; 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        ReadyToJump = true;
        exitingSlope = false;
    }

public  bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerheight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxslopeAngle;
        }
        return false;
    }

  public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    public void WallJump(Vector3 wallNormal)
    {
        exitingSlope = true; 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Yukarı ve duvardan uzaklaşma yönü
        Vector3 jumpDirection = transform.up + wallNormal;
        rb.AddForce(jumpDirection.normalized * jumpForce, ForceMode.Impulse);
    }

}
