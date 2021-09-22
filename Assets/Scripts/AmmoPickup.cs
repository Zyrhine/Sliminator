using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.SendMessage("AddAmmo", ammoAmount);
            Destroy(gameObject);
        }
    }
}