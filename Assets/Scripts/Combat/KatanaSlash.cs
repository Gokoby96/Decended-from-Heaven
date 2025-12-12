using UnityEngine;

public class KatanaSlash : MonoBehaviour
{
    [Header("Slash Settings")]
    public float slashSpeed = 20f;         
    public float returnSpeed = 12f;      
    public float slashAmount = 0.25f;      
    public float rotationAmount = 25f; 

    private Vector3 defaultPos;
    private Quaternion defaultRot;

    private Vector3 slashPos;
    private Quaternion slashRot;

    private bool isSlashing = false;

    void Start()
    {
       
        defaultPos = transform.localPosition;
        defaultRot = transform.localRotation;

        
        slashPos = defaultPos + new Vector3(0f, -slashAmount, 0f);
        slashRot = Quaternion.Euler(defaultRot.eulerAngles + new Vector3(rotationAmount, 0, 0));
    }
    public void PlaySlash()
    {
        isSlashing = true;
    }

    void Update()
    {
        if (isSlashing)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, slashPos, Time.deltaTime * slashSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, slashRot, Time.deltaTime * slashSpeed);

            if (Vector3.Distance(transform.localPosition, slashPos) < 0.01f)
            {
                isSlashing = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPos, Time.deltaTime * returnSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultRot, Time.deltaTime * returnSpeed);
        }
    }
    }


