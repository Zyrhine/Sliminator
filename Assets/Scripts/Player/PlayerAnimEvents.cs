using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    public AudioClip[] Footsteps;
    private AudioSource sound;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    void Footstep()
    {
        int i = Random.Range(0, Footsteps.Length - 1);
        sound.PlayOneShot(Footsteps[i]);
    }
}
