using UnityEngine;
using UnityEngine.AI;

public class VolatileSlime : MonoBehaviour
{
    enum State
    {
        Search,
        Chase,
        Explode
    }

    private Player target;
    private NavMeshAgent agent;
    private State state;
    private bool isExploding;

    [Header("Enemy Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float ExplosionRadius = 2f;

    void Start()
    {
        state = State.Search;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        switch(state)
        {
            case State.Search:
                UpdateSearch();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Explode:
                UpdateExplode();
                break;
        }
    }

    void UpdateSearch()
    {
        agent.SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) < 5f)
        {
            state = State.Chase;
        }
    }

    void UpdateChase()
    {
        agent.SetDestination(target.transform.position);

        var targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance < 1f)
        {
            state = State.Explode;
        }

        if (targetDistance > 5f)
        {
            state = State.Search;
        }
    }

    void UpdateExplode()
    {
        if (!isExploding)
        {
            agent.isStopped = true;
            Destroy(gameObject, 3);
            isExploding = true;
        }
    }

    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            // Die instantly
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Trigger death explosion

        // Deal damage in radius
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, ExplosionRadius);
        foreach (Collider collider in colliders)
        {
            collider.SendMessage("AddDamage", 25f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(gameObject.transform.position, ExplosionRadius);
    }
}
