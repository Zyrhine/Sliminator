using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private new Camera camera;
    private Rigidbody rb;
    private Animator anim;
    private AudioSource sound;

    // Transforms
    private Transform turretTransform;
    private Transform firePoint1;
    private Transform firePoint2;
    private Transform firePoint3;
    private Vector3 respawnPoint;

    [HideInInspector] public Vector3 aimPos;
    private Plane aimPlane;
    private Quaternion goalRot;
    private Quaternion firePoint1GoalRot;
    private Quaternion firePoint2GoalRot;
    private float fireInterval = 0f;
    private float shieldRechargeCounter = 0f;

    [Header("Player Stats")]
    public int Lives = 3;
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float MaxShield = 0f;
    public float Shield = 0f;
    public int Ammo = 100;
    public int MortarCharges = 5;

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
    public float ShieldRechargeRate = 0.1f;
    public float ShieldRechargeDelay = 5f;

    [Header("Prefabs")]
    public GameObject Bullet;
    public GameObject MortarMine;
    public GameObject DeathFX;
    public GameHUD HUD;

    [Header("Sounds")]
    public AudioClip[] ClipDamageImpacts;
    public AudioClip ClipDeath;

    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = transform.position;
        camera = Camera.main;
        sound = GetComponent<AudioSource>();
        rb = GetComponentInChildren<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        turretTransform = gameObject.transform.Find("Mech/Root/Torso/Neck/Head");
        firePoint1 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head/Arm.L/Gun.L");
        firePoint2 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head/Arm.R/Gun.R");
        firePoint3 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head");

        RefreshHUD();
    }

    void Update()
    {
        // Update the aimPlane
        aimPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y + 0.25f, 0));

        // Player movement
        if (dashing)
        {
            UpdateDash();
        }
        else
        {
            UpdateMovement();
        }

        // Player shoot at fire rate
        if (Input.GetButton("Fire"))
        {
            PrimaryFire();
        }

        if (HasMortarMine)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                SecondaryFire();
            }
        }

        // Shield Ability
        if (HasShield)
        {
            RegenerateShield();
        }
    }

    void PrimaryFire()
    {
        if (Ammo > 0 && fireInterval <= 0f)
        {
            Ammo--;
            sound.PlayOneShot(sound.clip);
            HUD.UpdateAmmo(Ammo);
            Instantiate(Bullet, firePoint1.position + firePoint1.forward * 2f, firePoint1.rotation);
            Instantiate(Bullet, firePoint2.position + firePoint2.forward * 2f, firePoint2.rotation);
            fireInterval = FireRate;
        }
        else
        {
            fireInterval -= Time.deltaTime;
        }
    }

    void SecondaryFire()
    {
        if (MortarCharges > 0)
        {
            MortarCharges--;
            //sound.PlayOneShot(sound.clip);
            HUD.UpdateMortarCharges(MortarCharges);
            Rigidbody mortar = Instantiate(MortarMine, firePoint3.position + firePoint3.forward * 2f, firePoint3.rotation).GetComponent<Rigidbody>();
            mortar.AddForce(mortar.transform.up * 25);
            mortar.AddForce(mortar.transform.forward * 1000);
        }
    }

    // Regenerate shield after delay
    void RegenerateShield()
    {
        if (shieldRechargeCounter > 0)
        {
            shieldRechargeCounter -= Time.deltaTime;
        }
        else
        {
            if (Shield < MaxShield)
            {
                Shield += ShieldRechargeRate;

                // Cap shield regeneration for potentially larger intervals
                if (Shield > MaxShield)
                {
                    Shield = MaxShield;
                }

                HUD.UpdateShield(Shield);
            }
        }
    }

    void UpdateDash()
    {
        // Apply dash movement
        rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);

        // Rotate body toward direction
        Quaternion goalBodyRotation = Quaternion.LookRotation(dashDirection, Vector3.up);
        var rotation = Quaternion.Slerp(rb.rotation, goalBodyRotation, Time.deltaTime * RotationSpeed);
        rb.MoveRotation(rotation);

        dashInterval -= Time.deltaTime;

        if (dashInterval <= 0)
        {
            dashing = false;
            anim.SetBool("Dash", false);
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
        Vector3 direction = forward * verticalAxis + right * horizontalAxis;

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

        // Activate Dash Ability
        if (HasDash)
        {
            if (Input.GetButtonDown("Dash"))
            {
                dashDirection = direction;
                dashing = true;
                anim.SetBool("Dash", true);
                dashInterval = dashLength;
            }
        }
    }

    Vector3 dashDirection;
    bool dashing = false;
    float dashLength = 0.25f;
    float dashSpeed = 30f;
    float dashInterval = 0f;

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
        int i = Random.Range(0, ClipDamageImpacts.Length - 1);
        sound.PlayOneShot(ClipDamageImpacts[i]);

        if (HasShield)
        {
            if (Shield >= damage)
            {
                Shield -= damage;
            }
            else
            {
                damage -= Shield;
                Shield = 0;
                Health -= damage;
            }

            shieldRechargeCounter = ShieldRechargeDelay;

            HUD.UpdateShield(Shield);
            HUD.UpdateHealth(Health);
        }
        else
        {
            Health -= damage;
            HUD.UpdateHealth(Health);
        }

        if (Health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Play death sound
        sound.PlayOneShot(ClipDeath);

        // Create death explosion
        GameObject explosion = Instantiate(DeathFX, transform.position, Quaternion.identity);
        Destroy(explosion, 3f);

        if (Lives > 1)
        {
            Lives--;
            Respawn();
        } else
        {
            Destroy(gameObject);

            // Trigger game over
        }
    }

    void Respawn()
    {
        transform.position = respawnPoint;
        Health = MaxHealth;
    }

    void SetRespawnPoint(Vector3 position)
    {
        respawnPoint = position;
    }

    void RefreshHUD()
    {
        HUD.DisplayShield(HasShield);
        HUD.UpdateAmmo(Ammo);
        HUD.UpdateMortarCharges(MortarCharges);
        HUD.UpdateShield(Shield);
        HUD.UpdateHealth(Health);
    }

    // Regain health
    void AddHealth(float health)
    {
        Health += health;

        // Cap health
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        HUD.UpdateHealth(Health);
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

    public void AddAmmo(int ammo)
    {
        Ammo += ammo;
        HUD.UpdateAmmo(Ammo);
    }
}
