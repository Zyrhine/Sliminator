using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private new Camera camera;
    private Rigidbody rb;
    private Animator anim;

    // Transforms
    private Transform turretTransform;
    private Transform firePoint1;
    private Transform firePoint2;

    [HideInInspector] public Vector3 aimPos;
    private Plane aimPlane;
    private Quaternion goalRot;
    private Quaternion firePoint1GoalRot;
    private Quaternion firePoint2GoalRot;
    private float fireInterval = 0f;

    [Header("Player Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float MaxShield = 0f;
    public float Shield = 0f;

    [Header("Abilities")]
    public bool HasDash = false;
    public bool HasShield = false;
    public bool HasMortarMine = false;
    public bool HasResistance = false;

    [Header("Settings")]
    public float MoveSpeed = 5f;
    public float RotationSpeed = 5f;
    public float HeadRotSpeed = 12f;
    public float FireRate = 0.1f;

    [Header("Prefabs")]
    public GameObject bullet;

    void Start()
    {
        camera = Camera.main;
        rb = GetComponentInChildren<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        turretTransform = gameObject.transform.Find("Mech/Root/Torso/Neck/Head");
        firePoint1 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head/Arm.L/Gun.L");
        firePoint2 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head/Arm.R/Gun.R");

        aimPlane = new Plane(Vector3.up, new Vector3(0, 0.25f, 0));
    }

    void Update()
    {
        // Player movement
        UpdateMovement();

        // Player shoot at fire rate
        
        if (Input.GetButton("Fire"))
        {
            if (fireInterval <= 0f)
            {
                Instantiate(bullet, firePoint1.position + firePoint1.forward * 2f, firePoint1.rotation);
                Instantiate(bullet, firePoint2.position + firePoint2.forward * 2f, firePoint2.rotation);
                fireInterval = FireRate;
            } else
            {
                fireInterval -= Time.deltaTime;
            }
        }
        
    }

    void UpdateMovement()
    {
        // Get movement input
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        // Project camera forward and right vectors a horizontal plane
        var forward = camera.transform.forward;
        var right = camera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Form the direction to move in
        var direction = forward * verticalAxis + right * horizontalAxis;

        if (direction != Vector3.zero)
        {
            // Apply movement
            rb.MovePosition(rb.position + direction * MoveSpeed * Time.deltaTime);

            // Rotate body toward direction
            Quaternion goalBodyRotation = Quaternion.LookRotation(direction, Vector3.up);
            var rotation = Quaternion.Slerp(rb.rotation, goalBodyRotation, Time.deltaTime * RotationSpeed);
            rb.MoveRotation(rotation);

            // Animate walk
            anim.SetFloat("Speed", 1);
        }
        else
        {
            // Stop walk animation
            anim.SetFloat("Speed", 0);
        }
    }

    private void FixedUpdate()
    {
        // Generate a ray from the cursor position
        Ray RayCast = camera.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        float HitDist = 0;

        // If the ray is parallel to the plane, Raycast will return false.
        if (aimPlane.Raycast(RayCast, out HitDist))
        {
            // Get the point along the ray that hits the calculated distance.
            aimPos = RayCast.GetPoint(HitDist);

            // Calculate aim rotations for the head and gun arms
            goalRot = Quaternion.Slerp(turretTransform.rotation, Quaternion.LookRotation(aimPos - rb.position), HeadRotSpeed * Time.deltaTime);
            firePoint1GoalRot = Quaternion.LookRotation(aimPos - firePoint1.position);
            firePoint2GoalRot = Quaternion.LookRotation(aimPos - firePoint2.position);
        }
    }

    // Take damage from a hit
    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            // Die instantly
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        // Apply rotations ontop of animation
        turretTransform.rotation = goalRot;
        firePoint1.rotation = firePoint1GoalRot;
        firePoint2.rotation = firePoint2GoalRot;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aimPos, 1f);
    }
}
