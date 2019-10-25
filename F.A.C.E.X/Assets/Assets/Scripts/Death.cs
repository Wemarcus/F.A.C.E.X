using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public AudioSource sound;
    public AudioSource sound_2;

    public GameObject blood;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Sword")
        {
            blood.SetActive(true);
            anim.SetTrigger("Death");
            sound.Play();
            sound_2.Play();
        }
    }
}
