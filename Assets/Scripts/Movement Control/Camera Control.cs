using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Bu script kameranın mouse ile kordinatlarıyla ilgilidir. -96
    
// fare hassasiyeti için olan yer
    public float Sensx;
    public float Sensy;
// player için kısım
    public Transform orientaion;
//Kamere için kısım
    float xRotation;
    float yRotation;
    [Header("Camera Tilt Settings")] // Kameranın smooth bir şekilde sağa yada sola yatmasını sağlamak için değerler
    public float tiltAmount ;      
    public float tiltSpeed ;
    public float tiltBoostAmount;
    
    [Header("FOV Settings")]
    public Camera playerCam;
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovLerpSpeed = 8f;

    private PlayerMovement playerMovement;
    
    float currentTilt;                 
    float targetTilt;   
    
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        // İlk satır imleci ortaya kilitlemek için ikincisi ise görünmez yapmak için
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerCam == null)
            playerCam = GetComponent<Camera>();

        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        // mouse input aldığımız yer burası
        float mouseX = Input.GetAxis("Mouse X")*Time.deltaTime * Sensx;
        float mouseY = Input.GetAxis("Mouse Y")*Time.deltaTime * Sensy;
        
        
        
        
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        float tiltBoost = 1f;

// Eğer sprint yapıyorsak tilt biraz daha güçlü olsun
        if (playerMovement.state == PlayerMovement.MovementState.sprinting && verticalInput == 0)
        {
            tiltBoost = tiltBoostAmount; // sağa sola sprintte eğim artışı
        }

        if (horizontalInput > 0) targetTilt = -tiltAmount * tiltBoost;
        else if (horizontalInput < 0) targetTilt = tiltAmount * tiltBoost;
        else targetTilt = 0;
       

        // Smooth geçişin yapıldığı yer
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Kamera ve playerın döndürüldüğü yer burası
        
        transform.rotation = Quaternion.Euler(xRotation, yRotation, currentTilt);
        orientaion.rotation = Quaternion.Euler(0, yRotation, 0);
        
        float targetFOV;

// Eğer sprint atıyorsa ve ileri gidiyorsa FOV büyür
        if (playerMovement.state == PlayerMovement.MovementState.sprinting && verticalInput > 0)
        {
            targetFOV = sprintFOV;
        }
        else
        {
            targetFOV = normalFOV;
        }

        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);
    }
}
