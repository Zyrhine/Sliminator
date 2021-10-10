using UnityEngine;

public class TrailDamage : MonoBehaviour
{
    private Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (!player.HasResistance)
            {
                other.SendMessage("AddDamage", 0.1);
            }
        }
    }
}
