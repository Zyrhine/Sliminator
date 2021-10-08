using System.Collections.Generic;
using UnityEngine;

public sealed class ToxicTurret : Slime
{
    private Transform firePoint;

    [Header("Enemy Stats")]
    public float FireRate = 4f;
    private float fireInterval = 0f;

    [Header("AI")]
    public float AttackRange = 30f;
    public List<Transform> WarpPoints = new List<Transform>();

    [Header("Sounds")]
    public AudioClip ClipShoot;

    [Header("Prefabs")]
    public GameObject ToxicBlob;

    protected override void Start()
    {
        base.Start();
        firePoint = transform.Find("ToxicTurret/Inside");
    }

    protected override void Update()
    {
        if (!Alive) return;
        base.Update();

        if (Vector3.Distance(transform.position, target.transform.position) <= AttackRange)
        {
            // Rotate toward the player
            Vector3 targetDirection = target.transform.position - transform.position;
            float singleStep = 2f * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            // Shoot at the player when ready
            if (fireInterval <= 0f)
            {
                Shoot();
            }
            else
            {
                fireInterval -= Time.deltaTime;
            }
        }
    }

    protected override void AddDamage(float damage)
    {
        base.AddDamage(damage);

        // 50% chance to teleport to a new location
        float rand = Random.value;
        if (rand < .5f)
        {
            Teleport();
        }
    }

    /// <summary>
    /// Shoot a ToxicBlob at the target.
    /// </summary>
    void Shoot()
    {
        // Fire blob directed at target
        var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        Instantiate(ToxicBlob, firePoint.position, rotation);
        sound.PlayOneShot(ClipShoot);

        // Animate
        Anim.SetTrigger("Shoot");

        // Reset shoot cooldown
        fireInterval = FireRate;
    }

    /// <summary>
    /// Warp the ToxicTurret to a new location from their available WarpPoints. 
    /// </summary>
    void Teleport()
    {
        // Need at least 2 points
        if (WarpPoints.Count < 2) return;

        // Pick a random warp point to teleport to 
        Vector3 point;
        while (true)
        {
            int rand = Random.Range(0, WarpPoints.Count - 1);
            point = WarpPoints[rand].position;

            // If we're already at that position, loop to pick a different one
            if (WarpPoints[rand].position != transform.position)
            {
                break;
            }
        }

        // Warp to it
        agent.Warp(point);
    }
}
