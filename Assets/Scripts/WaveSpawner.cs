using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public float TriggerRadius = 20f;
    public List<Transform> SpawnPoints = new List<Transform>();
    private GameHUD PlayerHUD;

    private bool WavesActive = false;
    private bool WavesComplete = false;
    private int CurrentWave = 0;

    public enum EnemyType
    {
        VolatileSlime,
        ArmoredSlime,
        ToxicTurret,
        MinionSlime,
        MassSlime,
        BossSlime
    }

    [System.Serializable]
    public class EnemyWave {
        [HideInInspector] public bool HasSpawned = false;
        public List<EnemyGroup> Enemies = new List<EnemyGroup>();
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public EnemyType EnemyType;
        public int Amount;

        public EnemyGroup(EnemyType type, int amount)
        {
            EnemyType = type;
            Amount = amount;
        }
    }

    public List<EnemyWave> waves = new List<EnemyWave>();

    private List<int> SpawnedEnemies = new List<int>();

    [Header("Enemy Prefabs")]
    public GameObject VolatileSlime;
    public GameObject ArmoredSlime;
    public GameObject ToxicTurret;
    public GameObject MinionSlime;
    public GameObject MassSlime;
    public GameObject BossSlime;

    void Start()
    {
        if (SpawnPoints.Count == 0)
        {
            Debug.Log("[Wave Spawner] No spawn points defined. Destroying self.");
            Destroy(gameObject);
        }

        // Get the HUD from the player
        PlayerHUD = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().HUD;

        // Set trigger radius on collider
        GetComponent<SphereCollider>().radius = TriggerRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        if (!WavesComplete && !WavesActive)
        {
            WavesActive = true;
            PlayerHUD.DisplayWaves(true);
            PlayerHUD.UpdateWave(CurrentWave + 1, 0);
        }
    }

    void Update()
    {
        if (WavesActive)
        {
            if (CurrentWave < waves.Count)
            {
                // Spawn the wave
                Debug.Log(waves[CurrentWave].HasSpawned);

                if (waves[CurrentWave].HasSpawned == false)
                {
                    Debug.Log("Spawning Wave: " + CurrentWave);
                    SpawnWave(waves[CurrentWave]);
                }
                
                // When all enemies are killed, move to the next wave
                if (SpawnedEnemies.Count == 0)
                {
                    Debug.Log("Wave " + CurrentWave + " Cleared.");

                    CurrentWave++;
                    if (CurrentWave == waves.Count)
                    {
                        Complete();
                    }
                }
            }
        }
    }

    void SpawnWave(EnemyWave wave)
    {
        foreach (EnemyGroup group in wave.Enemies)
        {
            GameObject enemyPrefab = null;

            switch (group.EnemyType)
            {
                case EnemyType.VolatileSlime:
                    enemyPrefab = VolatileSlime;
                    break;
                case EnemyType.ArmoredSlime:
                    enemyPrefab = ArmoredSlime;
                    break;
                case EnemyType.MinionSlime:
                    enemyPrefab = MinionSlime;
                    break;
                case EnemyType.MassSlime:
                    enemyPrefab = MassSlime;
                    break;
                case EnemyType.ToxicTurret:
                    enemyPrefab = ToxicTurret;
                    break;
                case EnemyType.BossSlime:
                    enemyPrefab = BossSlime;
                    break;
            }

            for (int i = 0; i < group.Amount; i++)
            {
                // Pick a random spawn location from locations
                Random.InitState(System.DateTime.Now.Millisecond);
                Vector3 spawn = SpawnPoints[Random.Range(0, SpawnPoints.Count - 1)].position;
                Vector3 position = new Vector3(spawn.x + Random.Range(-3f, 3f), spawn.y, spawn.z + Random.Range(-3f, 3f));

                // Spawn the enemy
                GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);

                // Become the owner of the slime
                enemy.SendMessage("SetOwner", gameObject);

                // Add the enemy to the list of spawned enemies
                SpawnedEnemies.Add(enemy.GetInstanceID());
            }
        }

        wave.HasSpawned = true;
        PlayerHUD.UpdateWave(CurrentWave + 1, SpawnedEnemies.Count);
    }

    // All waves have been completed
    void Complete()
    {
        WavesComplete = true;
        PlayerHUD.DisplayWaves(false);
        Destroy(gameObject);
    }

    // A wave enemy was destroyed
    void OnChildDestroyed(int instanceId)
    {
        // Remove them from the remaining enemies
        SpawnedEnemies.Remove(instanceId);
        PlayerHUD.UpdateWave(CurrentWave + 1, SpawnedEnemies.Count);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, TriggerRadius);
    }
}
