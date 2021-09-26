using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ToxicTurret : MonoBehaviour
{
    private Player target;
    private NavMeshAgent agent;

    [Header("Prefabs")]
    public GameObject ToxicBlob;

    [Header("Enemy Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float FireRate = 3f;
    private float fireInterval = 0f;

    [Header("AI")]
    public float AttackRange = 30f;
    public List<GameObject> WarpPoints = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
