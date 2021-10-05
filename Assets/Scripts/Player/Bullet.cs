using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        Destroy(gameObject, 1);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 2500);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.SendMessage("AddDamage", 25f);
            Destroy(gameObject);
        } else
        {
            Destroy(gameObject, 0.025f);
        }
    }
}
