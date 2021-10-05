using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int AmmoAmount = 100;
    public AudioClip ClipAmmoPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GetComponent<Animator>().Play("AmmoBoxOpen");
            GetComponent<AudioSource>().PlayOneShot(ClipAmmoPickup);
            other.gameObject.SendMessage("AddAmmo", AmmoAmount);
            Destroy(this);
        }
    }
}