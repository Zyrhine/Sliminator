using UnityEngine;

public class ToxicBlob : MonoBehaviour
{
    private Rigidbody rb;
    private Transform trail;

    void Awake()
    {
        Destroy(gameObject, 1);
    }

    private void Start()
    {
        trail = transform.Find("Trail");
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * 25);
        rb.AddForce(transform.forward * 100);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("AddDamage", 25f);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Disconnect the trail from the gameObject and delay its destruction
        if (trail){
            trail.parent = null;
            Destroy(trail.gameObject, 0.5f);
        }
    }
}
