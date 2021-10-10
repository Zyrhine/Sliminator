using UnityEngine;

public class MortarMine : MonoBehaviour
{
    public GameObject ExplosionFX;
    public float ExplosionRadius = 5f;

    private void OnTriggerEnter(Collider other)
    {
        // Explode if an enemy collider enters the trigger
        if (other.CompareTag("Enemy") && !other.isTrigger)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Deal damage in radius
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, ExplosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
            {
                collider.SendMessage("AddDamage", 100f);
            }

        }

        // Spawn explosion effect
        var explosion = Instantiate(ExplosionFX, gameObject.transform.position, Quaternion.identity);
        Destroy(explosion, 2);

        // Destroy self
        Destroy(gameObject);
    }
}
