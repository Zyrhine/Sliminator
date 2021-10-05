using UnityEngine;
using UnityEngine.AI;

public abstract class Slime : MonoBehaviour
{
    public enum SlimeState
    {
        Search,
        Chase,
        Explode,
        Charge,
        Merge
    }

    private GameObject owner;
    protected Player target;
    protected NavMeshAgent agent;
    protected SlimeState state;
    protected AudioSource sound;
    protected Rigidbody rb;

    [Header("Enemy Stats")]
    public float MaxHealth = 100f;
    public float Health = 100f;
    public float MoveSpeed = 10f;
    protected bool Alive = true;

    [Header("Sounds")]
    public AudioClip ClipDeath;

    protected virtual void Start()
    {
        state = SlimeState.Search;
        rb = GetComponent<Rigidbody>();
        sound = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = MoveSpeed;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void SetOwner(GameObject gameObject)
    {
        owner = gameObject;
    }

    protected virtual void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die(float delay = 1f)
    {
        if (Alive)
        {
            Alive = false;
            agent.enabled = false;
            sound.PlayOneShot(ClipDeath);
            Destroy(gameObject, delay);
        }
    }

    private void OnDestroy()
    {
        if (owner)
        {
            // Inform the owner that this slime has been destroyed.
            owner.SendMessage("OnChildDestroyed", gameObject.GetInstanceID());
        }
    }
}
