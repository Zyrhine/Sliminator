using UnityEngine;

public class RepairPickup : MonoBehaviour
{
    public int repairAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.SendMessage("AddHealth", repairAmount);
            Destroy(gameObject);
        }
    }
}
