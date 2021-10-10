using UnityEngine;
using UnityEngine.AI;

public abstract class Slime : MonoBehaviour
{
    public enum SlimeState
    {
        Idle,
        Search,
        Chase,
        Explode,
        ChargeUp,
        Charge,
        Merge
    }

    private LevelManager levelManager;
    private GameObject owner;
    protected Player target;
    protected NavMeshAgent agent;
    protected SlimeState state;
    protected AudioSource sound;
    protected Rigidbody rb;
    public Animator Anim;

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
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    protected virtual void Update()
    {
        // Go back to idle if no target
        if (!target)
        {
            state = SlimeState.Idle;
        }

        // Move Animation
        if (agent.isStopped)
        {
            Anim.SetFloat("Speed", 0);
        }
        else
        {
            Anim.SetFloat("Speed", 1);
        }
    }

    /// <summary>
    /// Set a reference to an entity that owns this slime so that it can callback when this slime is destroyed.
    /// </summary>
    /// <param name="gameObject"></param>
    void SetOwner(GameObject gameObject)
    {
        owner = gameObject;
    }

    /// <summary>
    /// Add damage to the slime.
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Kills the slime.
    /// </summary>
    /// <param name="delay"></param>
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

        // Add death to counter
        levelManager.AddKill();
    }
}
