using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for rotating the windmill rotor.
public class RotorRotator : MonoBehaviour
{
    public float rotationSpeed; // Speed for it to rotate

    // Update is called once per frame
    void Update()
    {   
        // Rotates
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
