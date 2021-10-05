using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Player player;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.25f;
    public float distanceOffset = 25;

    void Start()
    {
        transform.parent = null;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        // Get position of player with influence from aim position and add distance offset
        Vector3 offset = transform.forward * -distanceOffset;
        Vector3 position = player.transform.position + (player.aimPos - player.transform.position) / 4;
        Vector3 targetPosition = position + offset;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Rotate camera
        float rotate = Input.GetAxis("Rotate");
        if (rotate != 0)
        {
            //transform.Rotate(Vector3.up, rotate, Space.World);
            transform.RotateAround(position, Vector3.up, rotate);
        }
    }
}
