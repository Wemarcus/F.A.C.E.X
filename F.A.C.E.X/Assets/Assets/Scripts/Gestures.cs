using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestures : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Anger - Punch
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetTrigger("HandAnger");
        }
    }
}
