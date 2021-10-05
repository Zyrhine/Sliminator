using UnityEngine;

public sealed class MassSlime : Slime
{
    [Header("AI")]
    public float ChargeForce = 500f;
    public float ChaseRange = 20f;
    public float ChargeDistance = 10f;
    public float ChargeCooldown = 10f;
    private float chargeInterval = 0f;

    void Update()
    {
        if (!Alive) return;

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
        // Random wander

        if (Vector3.Distance(transform.position, target.transform.position) <= ChaseRange)
        {
            state = SlimeState.Chase;
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
            state = SlimeState.Search;
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
