using UnityEngine;

public sealed class VolatileSlime : Slime
{
    [Header("Prefabs")]
    public GameObject ExplosionFX;

    [Header("AI")]
    public float ExplosionRadius = 3f;

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

        // Animation
        if (agent.isStopped)
        {
            Anim.SetFloat("Speed", 0);
        }
        else
        {
            Anim.SetFloat("Speed", 2);
        }
    }

    void UpdateSearch()
    {
        agent.SetDestination(target.transform.position);

        if (Vector3.Distance(transform.position, target.transform.position) < 5f)
        {
            state = SlimeState.Chase;
        }
    }

    void UpdateChase()
    {
        agent.SetDestination(target.transform.position);

        var targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (targetDistance <= ExplosionRadius - 1)
        {
            // Explode
            Die(0.25f);
        }

        if (targetDistance > 5f)
        {
            state = SlimeState.Search;
        }
    }

    protected override void Die(float delay = 1f)
    {
        if (Alive)
        {
            base.Die(0.25f);
            var explosion = Instantiate(ExplosionFX, gameObject.transform.position, Quaternion.identity);
            Destroy(explosion, 2);

            // Deal damage in radius
            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, ExplosionRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
                {
                    collider.SendMessage("AddDamage", 25f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, ExplosionRadius);
    }
}
