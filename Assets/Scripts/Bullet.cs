using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Awake()
    {
        Destroy(gameObject, 1);
    }

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward);
    }
}
