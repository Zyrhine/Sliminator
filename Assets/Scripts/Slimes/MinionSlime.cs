using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionSlime : MonoBehaviour
{
    enum State
    {
        Search,
        Chase,
        Merge
    }

    private Player target;
    private NavMeshAgent agent;
    private State state;
    private bool alarmed = true;
    public AudioSource aDeathMini;
    public AudioSource aCombine;

    [HideInInspector] public bool hasGroup = false;
    private bool isLeader = false;

    [Header("Prefabs")]
    public GameObject MassSlime;

    [Header("Enemy Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;

    [Header("AI")]
    public float AllySearchRadius = 10f;
    public float MergeRadius = 3f;

    void Start()
    {
        state = State.Search;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (alarmed && !hasGroup)
        {
            CheckForMerge();
        }

        if (hasGroup)
        {
            state = State.Merge;
        }

        switch (state)
        {
            case State.Search:
                UpdateSearch();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Merge:
                UpdateMerge();
                break;
        }
    }

    void CheckForMerge()
    {
        if (hasGroup) return;

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, AllySearchRadius);
        List<GameObject> minions = new List<GameObject>();
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (collider.gameObject.GetComponent<MinionSlime>() != null)
                {
                    minions.Add(collider.gameObject);
                }
            }
        }
        if (minions.Count >= 3)
        {
            foreach (GameObject minion in minions)
            {
                var slime = minion.GetComponent<MinionSlime>();
                if (isLeader)
                {
                    // Tell the others they have a group
                    slime.hasGroup = true;
                } else
                {
                    if (!slime.hasGroup)
                    {
                        // Assume leadership
                        isLeader = true;
                        slime.hasGroup = true;
                        hasGroup = true;
                    }
                }
            }
            
            state = State.Merge;
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
            //Attack
        }

        if (targetDistance > 5f)
        {
            state = State.Search;
        }
    }

    void UpdateMerge()
    {
        // Get nearby minions
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, AllySearchRadius);
        List<GameObject> minions = new List<GameObject>();
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (collider.gameObject.GetComponent<MinionSlime>() != null)
                {
                    minions.Add(collider.gameObject);
                }
            }
        }

        // Disband from group if out of range
        if (minions.Count < 3)
        {
            hasGroup = false;
            isLeader = false;
            state = State.Search;
            return;
        }

        // Converge on average position
        Vector3 avgPosition = Vector3.zero;
        foreach (GameObject minion in minions)
        {
            avgPosition += minion.transform.position;
        }

        avgPosition /= minions.Count;
        agent.SetDestination(avgPosition);

        // If this minion is the leader, check if the group can merge
        if (isLeader)
        {
            foreach (GameObject minion in minions)
            {
                if (Vector3.Distance(gameObject.transform.position, minion.transform.position) > MergeRadius)
                {
                    return;
                }
            }

            // Everyone is in range
            MergeGroup(minions, avgPosition);
        }
    }

    void MergeGroup(List<GameObject> minions, Vector3 position)
    {
        // Spawn the mass
        Debug.Log("Spawning");
        aCombine.Play();
        Instantiate(MassSlime, position, Quaternion.identity);

        // Destroy the minions
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }
    }

    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            aDeathMini.Play();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {

    }

    private void OnDrawGizmos()
    {
        if (isLeader)
        {
            Gizmos.color = Color.magenta;
        } else if (hasGroup)
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawWireSphere(gameObject.transform.position, AllySearchRadius);
    }
}
