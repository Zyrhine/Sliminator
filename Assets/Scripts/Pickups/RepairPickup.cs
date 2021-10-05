using UnityEngine;

public class RepairPickup : MonoBehaviour
{
    public int repairAmount = 100;
    public AudioClip ClipHeal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GetComponent<AudioSource>().PlayOneShot(ClipHeal);
            other.gameObject.SendMessage("AddHealth", repairAmount);
            Destroy(gameObject, 0.5f);
        }
    }
}
