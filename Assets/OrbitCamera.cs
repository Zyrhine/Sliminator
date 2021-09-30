using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Vector3 OrbitPoint;
    public Vector3 Axis = Vector3.up;
    public float RotateSpeed = 0.01f;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(OrbitPoint, Axis, RotateSpeed);
    }
}
