using System.Collections.Generic;
using UnityEngine;

public sealed class ToxicTurret : Slime
{
    [Header("Enemy Stats")]
    public float FireRate = 3f;
    private float fireInterval = 0f;

    [Header("AI")]
    public float AttackRange = 30f;
    public List<GameObject> WarpPoints = new List<GameObject>();

    [Header("Sounds")]
    public AudioClip ClipShoot;

    [Header("Prefabs")]
    public GameObject ToxicBlob;

    void Update()
    {
        if (!Alive) return;

        if (fireInterval <= 0f)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= AttackRange)
            {
                Shoot();
                fireInterval = FireRate;
            }
        } else
        {
            fireInterval -= Time.deltaTime;
        }
        
    }

    void Shoot()
    {
        var rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        Instantiate(ToxicBlob, transform.position, rotation);
        sound.PlayOneShot(ClipShoot);
    }
}
