using UnityEngine;
using UnityEngine.AI;

public class MassSlime : MonoBehaviour
{
    enum State
    {
        Search,
        Chase
    }

    private State state;
    private Player target;
    private NavMeshAgent agent;
    private Rigidbody rb;

    [Header("Enemy Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;

    [Header("AI")]
    public float ChargeForce = 500f;
    public float ChaseRange = 20f;
    public float ChargeDistance = 10f;
    public float ChargeCooldown = 10f;
    private float chargeInterval = 0f;

    void Start()
    {
        state = State.Search;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (chargeInterval > 0)
        {
            chargeInterval -= Time.deltaTime;

            if (chargeInterval < 0)
            {
                chargeInterval = 0;
            }
        }

        switch (state)
        {
            case State.Search:
                UpdateSearch();
                break;
            case State.Chase:
                UpdateChase();
                break;
        }
    }

    void UpdateSearch()
    {
        // Random wander

        if (Vector3.Distance(transform.position, target.transform.position) <= ChaseRange)
        {
            state = State.Chase;
        }
    }

    void UpdateChase()
    {
        agent.SetDestination(target.transform.position);

        if (chargeInterval == 0)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= ChargeDistance)
            {
                ChargeAttack();
            }
        }

        if (Vector3.Distance(transform.position, target.transform.position) > ChaseRange)
        {
            state = State.Search;
        }
    }

    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void ChargeAttack()
    {
        Debug.Log("Charging");
    }

    private void OnDrawGizmos()
    {

    }
}
