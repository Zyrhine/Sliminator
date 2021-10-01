using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 100;
    public AudioSource aAmmo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            aAmmo.Play();
            other.gameObject.SendMessage("AddAmmo", ammoAmount);
            Destroy(gameObject);
        }
    }
}