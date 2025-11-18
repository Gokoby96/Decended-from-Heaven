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
    public float dashSpeed;
    public float swingSpeed;
    public float dashSpeedChangeFactor;
    public float maxYSpeed;
    
    
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;


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
    
    [Header("Crouch Ceiling Check")]
    public float ceilingCheckDistance = 1.0f;
    public LayerMask whatIsCeiling;
    private bool blockedAbove;
    private bool isCrouching = false;
    
   
    
    
    public Transform oriantation;
    //bu ise playerın movement stateini tutucak 
    public MovementState state;
   
  // Movement stateleri enum listesinde tutuyoruz 
    public enum MovementState
    {
        freeze,
        walking,
        swinging,
        sprinting,
        crouching,
        wallRunning,
        dashing,
        sliding,
        air
    }

    public bool activeGrapple;
    public bool freeze;
    public bool swinging;
    public bool dashing;
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
        CheckCeiling();
        
        if (isCrouching && !blockedAbove && !UnityEngine.Input.GetKey(crouchKey))
        {
            StopCrouch();
        }
        
       
        
       
        // sürükleme
        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching && !activeGrapple)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void CheckCeiling()
    {
        // Karakterin üstünde engel var mı kontrol et
        blockedAbove = Physics.Raycast(transform.position, Vector3.up, ceilingCheckDistance, whatIsCeiling);
        
        Color rayColor = blockedAbove ? Color.red : Color.green;
        Debug.DrawRay(transform.position, Vector3.up * ceilingCheckDistance, rayColor);
    }

    private void Input()
    {
        // ınput alınan yer
        horizontalınput = UnityEngine.Input.GetAxis("Horizontal");
        verticalınput = UnityEngine.Input.GetAxis("Vertical");
        if (UnityEngine.Input.GetKeyDown(jump)&& ReadyToJump&& isGrounded && !wallrunning)
        {
            ReadyToJump = false;

          
            {
                Jump();
            }

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        // crouching işlemi
        if (UnityEngine.Input.GetKeyDown(crouchKey))
        {
            StartCrouch();
        }

        if (UnityEngine.Input.GetKeyUp(crouchKey))
        {
            if (!blockedAbove && state == MovementState.walking)
            {
               
                // Üstü açık, normal yüksekliğe dön
                StopCrouch();
            }
            else
            {
                StartCoroutine(WaitUntilClear());
            }
        }
    }

  
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocitySet= CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
    Invoke(nameof(SetVelocity), 0.1f);
      
    }

    private Vector3 velocitySet;
    
    private void SetVelocity()
    {
       
        rb.linearVelocity = velocitySet;
        
    }

  
   

    // bu fonksiyon player statelerini tutup hızını ayarlamamıza yarayacak
    private void StateHandler()
    {
        if (freeze)
        {
           state = MovementState.freeze;
           desiredMoveSpeed = 0;
           rb.linearVelocity = Vector3.zero;
        }
       else if  (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        
        // wallrun state inde 
       else  if (wallrunning)
        {
            state = MovementState.wallRunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        // slide state inde 
       else if (sliding)
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
        else if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
        }
        //crouching state inde ise
        else if (UnityEngine.Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        // sprint te ise
       else  if (isGrounded&& UnityEngine.Input.GetKey(sprintKey)&& verticalınput >=0 )
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintspeed;
        }
        // Walking state de ise
        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // air stateinde ise
        else
        {
            state = MovementState.air;
            if (desiredMoveSpeed < sprintspeed)
            {
                desiredMoveSpeed = walkSpeed;
                
            }
            else
            {
                desiredMoveSpeed = sprintspeed;
            }
        }
        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing)
        {
            keepMomentum = true;
        }

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                speed = desiredMoveSpeed;
            }
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

        
    }

    private float speedChangeFactor;
    private void StartCrouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 80f, ForceMode.Impulse);
    }
    private void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
      
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - speed);
        float startValue = speed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            speed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

       speed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }
    
    private void MovePlayer()
    {
        if (activeGrapple)
        {
            return;
        }
       
        if (state == MovementState.dashing)
        {
            return;
        }
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
        if (activeGrapple)
        {
            return;
        }
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

        if (maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed,rb.linearVelocity.z );
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
   

    private IEnumerator WaitUntilClear()
    {
        Debug.Log("WaitUntilClear başlatıldı. Engel kalkması bekleniyor...");
        // Engel kalkana kadar bekle
        while (blockedAbove)
        {
            CheckCeiling();
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Engel kalktı! Scale düzeltiliyor.");
      
        StopCrouch();
        
    }
   

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt( -1*gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ/ (Mathf.Sqrt(-1*trajectoryHeight / gravity));
        
        return velocityXZ + velocityY;
    }

}
