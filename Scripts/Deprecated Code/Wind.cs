using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public float minForce = 0f;
    public float maxForce = 1f;
    public float maxVelocityChange = 0.1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 force = Random.onUnitSphere;
        force.y = 0f; // limit wind to horizontal plane
        force = force.normalized * Random.Range(minForce, maxForce);

        if (rb.velocity.magnitude < maxVelocityChange)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(force * (maxVelocityChange / rb.velocity.magnitude), ForceMode.Impulse);
        }
    }
}