using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public float recoilAmount = 0.1f;     
    public float recoilSpeed = 10f;        

    private Vector3 originalLocalPos;      
    private Vector3 recoilOffset;           

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    void Update()
    {
        
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilSpeed);

        
        transform.localPosition = originalLocalPos + recoilOffset;
    }

    public void Fire()
    {
        
        recoilOffset += new Vector3(0, 0, -recoilAmount);
    }
}
