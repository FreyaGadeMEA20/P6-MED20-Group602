using UnityEngine;

public class JumpingAnimal : MonoBehaviour
{
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 0.5f;
    public float rotationSpeed = 5f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float bounceOffset = 0f;
    private float rotationOffset = 0f;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        float bounce = Mathf.Sin(bounceOffset) * bounceHeight;
        if (bounce < 0)
        {
            bounce = 0;
        }
        float rotation = Mathf.Sin(rotationOffset) * 360f;


        transform.position = startPosition + Vector3.up * bounce;
        transform.rotation = startRotation * Quaternion.Euler(0f, rotation, 0f);

        bounceOffset += Time.deltaTime * bounceSpeed;
        rotationOffset += Time.deltaTime * rotationSpeed;
    }

}