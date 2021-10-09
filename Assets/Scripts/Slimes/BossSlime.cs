using System.Collections.Generic;
using UnityEngine;

public sealed class BossSlime : Slime
{
    private GameHUD BossHUD;
    public List<Transform> SummonPoints = new List<Transform>();

    [Header("Enemy Prefabs")]
    public GameObject VolatileSlime;
    public GameObject ArmoredSlime;
    public GameObject ToxicTurret;
    public GameObject MinionSlime;
    public GameObject MassSlime;

    private List<int> SpawnedEnemies = new List<int>();

    [Header("AI")]
    public float SummonCooldown = 15;
    private float summonInterval = 0;

    bool inFight = false;

    protected override void Start()
    {
        base.Start();

        // Require there to be places to spawn enemies
        if (SummonPoints.Count == 0)
        {
            Debug.Log("[BossSlime]: No spawn points set, destroying self.");
            Destroy(gameObject);
        }

        BossHUD = target.HUD;
    }

    protected override void Update()
    {
        if (!Alive) return;
        base.Update();

        if (inFight)
        {
            // Spawn more enemies if there are none and the cooldown has passed
            if (SpawnedEnemies.Count == 0)
            {
                if (summonInterval <= 0)
                {
                    Summon();
                } else
                {
                    summonInterval -= Time.deltaTime;
                }
            }
        }
    }

    void BeginFight()
    {
        inFight = true;
        BossHUD.DisplayBoss(true);
        BossHUD.UpdateBoss(Health, MaxHealth);
    }

    void Summon()
    {
        // Reset summon cooldown
        summonInterval = SummonCooldown;

        // Animate
        Anim.SetTrigger("Summon");

        // Pick how many enemies to summon based on health remaining
        int toSpawn = 0;
        switch (Health)
        {
            case float n when (n > (MaxHealth / 4) * 3): // 100 - 75% health
                toSpawn = 4;
                break;
            case float n when (n < (MaxHealth / 4) * 3): // 75 - 50% health
                toSpawn = 6;
                break;
            case float n when (n < (MaxHealth / 2)): // 50 - 25% health
                toSpawn = 8;
                break;
            case float n when (n < (MaxHealth / 4) * 3): // 25 - 0% health
                toSpawn = 10;
                break;
        }

        // Spawn the enemies
        int point = 0;
        for (int i = 0; i < toSpawn; i++)
        {
            GameObject enemyPrefab = null;

            int rand = Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    enemyPrefab = VolatileSlime;
                    break;
                case 1:
                    enemyPrefab = ArmoredSlime;
                    break;
                case 2:
                    enemyPrefab = MinionSlime;
                    break;
                case 3:
                    enemyPrefab = MassSlime;
                    break;
                case 4:
                    enemyPrefab = ToxicTurret;
                    break;
            }

            // Pick the next spawn location with a random offset
            Random.InitState(i);
            Vector3 spawn = SummonPoints[point].position;
            if (point < SummonPoints.Count - 1)
            {
                point++;
            }
            else
            {
                point = 0;
            }
            Vector3 position = new Vector3(spawn.x + Random.Range(-3f, 3f), spawn.y, spawn.z + Random.Range(-3f, 3f));

            // Spawn the enemy
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

            // Become the owner of the slime
            enemy.SendMessage("SetOwner", gameObject);

            // Add the enemy to the list of spawned enemies
            SpawnedEnemies.Add(enemy.GetInstanceID());
        }
    }

    protected override void AddDamage(float damage)
    {
        base.AddDamage(damage);
        BossHUD.UpdateBoss(Health, MaxHealth);
    }

    protected override void Die(float delay = 5)
    {
        base.Die(delay);
        Anim.SetBool("IsDead", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BeginFight();
        }
    }

    // A wave enemy was destroyed
    void OnChildDestroyed(int instanceId)
    {
        // Remove them from the remaining enemies
        SpawnedEnemies.Remove(instanceId);
    }
}
