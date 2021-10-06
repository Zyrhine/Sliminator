using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    public GameObject InteractTarget;
    private GameObject prompt;

    private void Start()
    {
        prompt = transform.Find("Prompt").gameObject;
        prompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetButton("Interact"))
        {
            InteractTarget.SendMessage("OnInteract");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        prompt.SetActive(false);
    }
}
