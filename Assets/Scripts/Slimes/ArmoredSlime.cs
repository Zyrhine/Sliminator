using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArmoredSlime : MonoBehaviour
{
    enum State
    {
        Search,
        Chase
    }

    private State state;
    private Player target;
    private NavMeshAgent agent;

    //Audio
    public AudioSource aArmour;
    public AudioSource aDeath;

    [Header("Parts")]
    public GameObject PlateArmor;

    [Header("Enemy Stats")]
    public float Armor = 100f;
    public float MaxHealth = 200f;
    public float Health = 200f;

    [Header("AI")]
    public float ChaseRange = 20f;
    public List<GameObject> PatrolPoints = new List<GameObject>();

    void Start()
    {
        state = State.Search;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
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

    }

    void UpdateChase()
    {

    }

    void AddDamage(float damage)
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
            aArmour.Play();
            Destroy(PlateArmor);
        }

        if (Health <= 0f)
        {
            aDeath.Play();
            Destroy(gameObject);
        }
    }
}
