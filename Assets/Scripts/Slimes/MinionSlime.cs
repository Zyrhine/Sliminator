using System.Collections.Generic;
using UnityEngine;

public sealed class MinionSlime : Slime
{
    [HideInInspector] public bool hasGroup = false;
    private bool alarmed = true;
    private bool isLeader = false;

    [Header("AI")]
    public float AllySearchRadius = 10f;
    public float MergeRadius = 3f;
    public float ChaseRadius = 20f;

    [Header("Sounds")]
    public AudioClip ClipCombine;

    [Header("Prefabs")]
    public GameObject MassSlime;

    private ParticleSystem trail;

    protected override void Start()
    {
        base.Start();
        trail = transform.Find("Trail").GetComponent<ParticleSystem>();
    }

    protected override void Update()
    {
        if (!Alive) return;
        base.Update();

        // Grouping
        if (alarmed && !hasGroup)
        {
            CheckForMerge();
        }

        if (hasGroup)
        {
            state = SlimeState.Merge;
        }

        // States
        switch (state)
        {
            case SlimeState.Search:
                UpdateSearch();
                break;
            case SlimeState.Chase:
                UpdateChase();
                break;
            case SlimeState.Merge:
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
            
            state = SlimeState.Merge;
        }
    }

    void UpdateSearch()
    {
        agent.SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) < ChaseRadius)
        {
            state = SlimeState.Chase;
            agent.speed = 10f;
        }
    }

    void UpdateChase()
    {
        // Try to get infront of the player
        agent.SetDestination(target.transform.position + target.rb.velocity * 10);

        if (Vector3.Distance(transform.position, target.transform.position) > ChaseRadius)
        {
            state = SlimeState.Search;
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
            state = SlimeState.Search;
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
        sound.PlayOneShot(ClipCombine);
        Instantiate(MassSlime, position, Quaternion.identity);

        // Destroy the minions
        foreach (GameObject minion in minions)
        {
            Destroy(minion);
        }
    }

    protected override void Die(float delay = 1)
    {
        base.Die(delay);

        // Disconnect the particle system and delay its destruction
        trail.transform.parent = null;
        trail.Stop();
        Destroy(trail, 5f);
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
