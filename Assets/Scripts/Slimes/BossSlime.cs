using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossSlime : MonoBehaviour
{
    enum State
    {
        Search,
        Chase
    }

    private State state;
    private Player target;
    private NavMeshAgent agent;

    [Header("Enemy Stats")]
    public float MaxHealth = 1000f;
    public float Health = 1000f;

    void Start()
    {

    }

    void Update()
    {

    }

    void AddDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
