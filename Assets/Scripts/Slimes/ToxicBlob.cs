using UnityEngine;

public class ToxicBlob : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject Trail;

    void Awake()
    {
        Destroy(gameObject, 1);
    }

    private void Start()
    {
        Trail = GameObject.Find("Trail");
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * 25);
        rb.AddForce(transform.forward * 100);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("AddDamage", 25f);
            Destroy(gameObject);
            
        }
        else
        {
            Destroy(gameObject, 0.025f);
            
        }
    }

    private void OnDestroy()
    {
        if(Trail != null){
            Destroy(Trail, 0.5f);
        }
        
    }
}
