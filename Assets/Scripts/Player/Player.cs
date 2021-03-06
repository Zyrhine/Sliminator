using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Dash Ability Settings
    private Vector3 dashDirection;
    private bool dashing = false;
    private float dashLength = 0.25f;
    private float dashSpeed = 30f;
    private float dashInterval = 0f;

    // Components
    private new Camera camera;
    [HideInInspector] public Rigidbody rb;
    private Animator anim;
    private AudioSource sound;
    private LevelManager levelManager;

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
    public AudioClip ClipMortarLaunch;
    public AudioClip ClipDash;

    void Start()
    {
        // Load unlocked abilities
        LoadAbilities();

        // Get component references
        respawnPoint = transform.position;
        camera = Camera.main;
        sound = GetComponent<AudioSource>();
        rb = GetComponentInChildren<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        turretTransform = gameObject.transform.Find("Mech/Root/Torso/Neck/Head");
        firePoint1 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head/Arm.L/Gun.L");
        firePoint2 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head/Arm.R/Gun.R");
        firePoint3 = gameObject.transform.Find("Mech/Root/Torso/Neck/Head");
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        // Update the hud with the initial player state
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

        // Fire the primary weapon
        if (Input.GetButton("Fire"))
        {
            PrimaryFire();
        }

        // Fire the secondary weapon
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

    /// <summary>
    /// Fires the primary weapon.
    /// </summary>
    void PrimaryFire()
    {
        if (Ammo > 0 && fireInterval <= 0f)
        {
            Ammo--;
            levelManager.AddAmmoUsed();
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

    /// <summary>
    /// Fires the secondary weapon.
    /// </summary> 
    void SecondaryFire()
    {
        if (MortarCharges > 0)
        {
            MortarCharges--;
            levelManager.AddAmmoUsed();
            HUD.UpdateMortarCharges(MortarCharges);
            Rigidbody mortar = Instantiate(MortarMine, firePoint3.position + firePoint3.forward * 2f, firePoint3.rotation).GetComponent<Rigidbody>();

            // Arc the projectile with gravity
            mortar.AddForce(mortar.transform.up * 25);
            mortar.AddForce(mortar.transform.forward * 1000);
        }
    }

    /// <summary>
    /// Regenerate the player shield by the recharge rate if not on cooldown.
    /// </summary>
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
                sound.PlayOneShot(ClipDash);
                dashDirection = direction;
                dashing = true;
                anim.SetBool("Dash", true);
                dashInterval = dashLength;
            }
        }
    }

    private void FixedUpdate()
    {
        // Generate a ray from the cursor position
        Ray RayCast = camera.ScreenPointToRay(Input.mousePosition);
        float HitDist = 0;

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

    void LateUpdate()
    {
        // Apply rotations ontop of animation
        turretTransform.rotation = goalRot;
        firePoint1.rotation = firePoint1GoalRot;
        firePoint2.rotation = firePoint2GoalRot;
    }

    /// <summary>
    /// Take damage from a hit
    /// </summary>
    /// <param name="damage"></param>
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

    /// <summary>
    /// Sequence to take place upon the death of the player.
    /// </summary>
    void Die()
    {
        // Play death sound
        sound.PlayOneShot(ClipDeath);

        // Create death explosion
        GameObject explosion = Instantiate(DeathFX, transform.position, Quaternion.identity);
        Destroy(explosion, 3f);

        if (Lives > 1)
        {
            // Respawn
            Lives--;
            Respawn();
        } else
        {
            // Game Over
            Destroy(gameObject);
            HUD.DisplayFailScreen(true);
        }
    }

    /// <summary>
    /// Respawn the player at the current checkpoint.
    /// </summary>
    void Respawn()
    {
        transform.position = respawnPoint;
        Health = MaxHealth;
    }

    /// <summary>
    /// Updates the position of the checkpoint.
    /// </summary>
    /// <param name="position"></param>
    void SetRespawnPoint(Vector3 position)
    {
        respawnPoint = position;
    }

    /// <summary>
    /// Update the GameHUD with the current player state.
    /// </summary>
    void RefreshHUD()
    {
        HUD.DisplayShield(HasShield);
        HUD.UpdateAmmo(Ammo);
        HUD.DisplayMortarCharges(HasMortarMine);
        HUD.UpdateMortarCharges(MortarCharges);
        HUD.UpdateShield(Shield);
        HUD.UpdateHealth(Health);
    }

    /// <summary>
    /// Add health back to the player.
    /// </summary>
    /// <param name="health"></param>
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

    /// <summary>
    /// Add ammo to the player's primary fire.
    /// </summary>
    /// <param name="ammo"></param>
    public void AddAmmo(int ammo)
    {
        Ammo += ammo;
        HUD.UpdateAmmo(Ammo);
    }

    /// <summary>
    /// Enables a player ability.
    /// </summary>
    /// <param name="ability"></param>
    public void UnlockAbility(PlayerAbility ability)
    {
        switch (ability)
        {
            case PlayerAbility.Dash:
                HasDash = true;
                PlayerPrefs.SetInt("HasDash", 1);
                break;
            case PlayerAbility.MortarMine:
                HasMortarMine = true;
                PlayerPrefs.SetInt("HasMortarMine", 1);
                break;
            case PlayerAbility.Resistance:
                HasResistance = true;
                PlayerPrefs.SetInt("HasResistance", 1);
                break;
            case PlayerAbility.Shield:
                HasShield = true;
                PlayerPrefs.SetInt("HasShield", 1);
                break;
        }

        RefreshHUD();
        levelManager.AddUnlock();
    }

    /// <summary>
    /// Loads the currently unlocked abilities.
    /// </summary>
    void LoadAbilities()
    {
        HasDash = PlayerPrefs.GetInt("HasDash", 0) == 1;
        HasMortarMine = PlayerPrefs.GetInt("HasMortarMine", 0) == 1;
        HasResistance = PlayerPrefs.GetInt("HasResistance", 0) == 1;
        HasShield = PlayerPrefs.GetInt("HasShield", 0) == 1;
    }

    void OnDrawGizmos()
    {
        // Highlight where the player is aiming
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(aimPos, 1f);
    }

    public enum PlayerAbility
    {
        None,
        Dash,
        Shield,
        MortarMine,
        Resistance
    }
}
