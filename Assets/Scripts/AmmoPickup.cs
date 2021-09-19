using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int addAmmo = 100;
    public float rotationSpeed = 0.0f;

    private float hoverSpeed = 0.5f;
    private float hoverHeight = 0.5f;
    private Vector3 tempPos;
    private Vector3 posOffset;

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed *= 100.0f;
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.rotation.x, rotationSpeed * Time.deltaTime, transform.rotation.z);
        
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * hoverSpeed) * hoverHeight;
        transform.position = tempPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.SendMessage("ApplyAmmoPickup", addAmmo);
            Destroy(gameObject);
        }
    }
}