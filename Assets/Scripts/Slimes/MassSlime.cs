using UnityEngine;

public sealed class MassSlime : Slime
{
    [Header("AI")]
    public float ChargeSpeed = 100f;
    public float ChaseRange = 30f;
    public float ChargeDistance = 30f;
    public float ChargeCooldown = 10f;
    private float chargeInterval = 0f;
    private float chargeTimer = 0;
    private float defaultAcceleration;

    [Header("Prefabs")]
    public GameObject DeathSplat;

    protected override void Start()
    {
        base.Start();
        defaultAcceleration = agent.acceleration;
    }

    protected override void Update()
    {
        if (!Alive) return;
        base.Update();

        if (agent.isStopped)
        {
            Anim.SetFloat("Speed", 0);
        }
        else
        {
            Anim.SetFloat("Speed", 2);
        }

        switch (state)
        {
            case SlimeState.Search:
                UpdateSearch();
                break;
            case SlimeState.Chase:
                UpdateChase();
                break;
            case SlimeState.ChargeUp:
                UpdateChargeUp();
                break;
            case SlimeState.Charge:
                UpdateCharge();
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

        // Prepare to charge
        if (chargeInterval > 0)
        {
            chargeInterval -= Time.deltaTime;

            if (chargeInterval < 0)
            {
                chargeInterval = 0;
            }
        }

        if (chargeInterval == 0)
        {
            if (Vector3.Distance(transform.position, target.transform.position) <= ChargeDistance)
            {
                state = SlimeState.ChargeUp;
            }
        }

        if (Vector3.Distance(transform.position, target.transform.position) > ChaseRange)
        {
            state = SlimeState.Search;
        }
    }

    void UpdateCharge()
    {
        if (agent.remainingDistance < 1f)
        {
            // Reached the charge destination, stop moving
            agent.isStopped = true;
        }

        // Completed the charge, resume chasing
        if (agent.velocity.sqrMagnitude < 1 && agent.isStopped)
        {
            Anim.SetTrigger("Slam");
            agent.acceleration = defaultAcceleration;
            agent.speed = MoveSpeed;
            state = SlimeState.Chase;
            agent.isStopped = false;
        }
    }

    void UpdateChargeUp()
    {
        agent.isStopped = true;
        Anim.SetBool("Charging", true);

        // Determine which direction to rotate towards
        Vector3 targetDirection = target.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = 2 * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= 2f)
        {
            chargeTimer = 0;
            Anim.SetBool("Charging", false);
            Charge();
        }
    }

    void Charge()
    {
        Anim.SetTrigger("Charged");
        chargeInterval = ChargeCooldown;
        Vector3 targetPos = target.transform.position;
        agent.isStopped = false;
        agent.speed = ChargeSpeed;
        agent.acceleration = 100f;
        agent.SetDestination(targetPos);
        state = SlimeState.Charge;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == SlimeState.Charge) {
            if (collision.rigidbody.CompareTag("Player"))
            {
                collision.rigidbody.gameObject.SendMessage("AddDamage", 100f);
            }
            
            // Play slam sound effect
        }
    }

    protected override void Die(float delay = 1)
    {
        base.Die(delay);
        Instantiate(DeathSplat, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {

    }
}
