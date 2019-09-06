using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public float pleasant;
    public float aroused;

    public float value; // pleasant AND aroused
    public string status;

    public GameObject character;
    private CoreAffect core_affect;
    private double angle;

    private void Start()
    {
        core_affect = character.GetComponent<CoreAffect>();
    }

    public void setStatus()
    {
        angle = core_affect.calculateAngle();
        pleasant = core_affect.getPleasant();
        aroused = core_affect.getAroused();

        if (status == "anger")
        {
            //float pleasant_tmp = 
        }


        core_affect.setPleasant(pleasant);
        core_affect.setAroused(aroused);
    }
}
