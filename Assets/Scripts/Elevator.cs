using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float MoveHeight = 50f;
    public float MoveSpeed = 0.25f;
    private Vector3 goalPos;
    private bool move = false;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        goalPos = new Vector3(transform.position.x, transform.position.y + MoveHeight, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            // Move to goal position
            rb.MovePosition(transform.position + new Vector3(0, MoveSpeed, 0));

            if (transform.position.y >= goalPos.y)
            {
                move = false;
            }
        }
    }

    void OnInteract()
    {
        move = true;
    }
}
