using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    Vector3 spawnLocation;

    private void Start()
    {
        spawnLocation = transform.Find("SpawnLocation").position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Set the player's respawn point to this location
        if (other.CompareTag("Player"))
        {
            
            other.gameObject.SendMessage("SetRespawnPoint", spawnLocation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnLocation, new Vector3(1, 1, 1));
    }
}
