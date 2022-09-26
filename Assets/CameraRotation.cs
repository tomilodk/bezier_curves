using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float RotationSpeed = 5f;

    void FixedUpdate()
    {
        gameObject.transform.Rotate(Vector3.up * (Time.deltaTime * RotationSpeed));
    }
}