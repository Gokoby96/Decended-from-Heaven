
using UnityEngine;

public class BoxFloat : MonoBehaviour
{
    public float amplitude = 0.2f; 
    public float frequency = 1f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}

