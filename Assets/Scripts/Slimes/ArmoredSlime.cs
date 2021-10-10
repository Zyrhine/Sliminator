using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class ArmoredSlime : Slime
{
    [Header("Parts")]
    public GameObject PlateArmor;

    [Header("Enemy Stats")]
    public float Armor = 100f;
    public float AttackSpeed = 2.5f;
    private float attackInterval = 0f;

    [Header("AI")]
    public float ChaseRange = 10f;
    public float AttackRange = 4f;
    public List<GameObject> PatrolPoints = new List<GameObject>();

    [Header("Sounds")]
    public AudioClip ClipArmorLost;

    protected override void Update()
    {
        if (!Alive) return;
        base.Update();

        switch (state)
        {
            case SlimeState.Search:
                UpdateSearch();
                break;
            case SlimeState.Chase:
                UpdateChase();
                break;
        }
    }

    void UpdateSearch()
    {
        if (agent.remainingDistance < 1f)
        {
            agent.SetDestination(RandomPosition(20f));
        }

        if (Vector2.Distance(transform.position, target.transform.position) <= ChaseRange)
        {
            state = SlimeState.Chase;
        }
    }

    Vector3 RandomPosition(float radius)
    {
        var randDirection = Random.insideUnitSphere * radius;
        randDirection += agent.transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, radius, -1);
        return navHit.position;
    }

    void UpdateChase()
    {
        // Close enough to attack
        if (Vector3.Distance(transform.position, target.transform.position) <= AttackRange)
        {
            if (attackInterval <= 0)
            {
                Attack();
            } else
            {
                attackInterval -= Time.deltaTime;
            }
        } else
        {
            // Advance toward player
            agent.SetDestination(target.transform.position);
        }

        // Has gone out of target range
        if (Vector3.Distance(transform.position, target.transform.position) > ChaseRange)
        {
            state = SlimeState.Search;
        }
    }

    // Attack the player
    void Attack()
    {
        Debug.Log("Attacking");
        Anim.SetTrigger("Attack");
        target.SendMessage("AddDamage", 25);

        // Reset attack cooldown
        attackInterval = AttackSpeed;
    }

    protected override void AddDamage(float damage)
    {
        if (Armor >= damage)
        {
            Armor -= damage;
        }
        else
        {
            damage -= Armor;
            Armor = 0;
            Health -= damage;
        }

        // Remove visual armor if armor is lost
        if (PlateArmor && Armor == 0)
        {
            sound.PlayOneShot(ClipArmorLost);
            Destroy(PlateArmor);
        }

        if (Health <= 0f)
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
