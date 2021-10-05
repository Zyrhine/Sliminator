using UnityEngine;

public class RepairPickup : MonoBehaviour
{
    public int repairAmount = 100;
    public AudioSource aHeal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            aHeal.Play();
            other.gameObject.SendMessage("AddHealth", repairAmount);
            Destroy(gameObject);
        }
    }
}
