using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    public float human_pleasant;
    public float human_aroused;
    public float ai_pleasant;
    public float ai_aroused;

    public Dialogues setting;

    public GameObject human_character;
    public GameObject ai_character;

    private CoreAffect human_core_affect;
    private CoreAffect ai_core_affect;

    private void Start()
    {
        human_core_affect = human_character.GetComponent<CoreAffect>();
        ai_core_affect = ai_character.GetComponent<CoreAffect>();
    }

    public void setStatus()
    {
        /*human_core_affect.setPleasant(human_core_affect.getPleasant() + human_pleasant);
        human_core_affect.setAroused(human_core_affect.getAroused() + human_aroused);

        ai_core_affect.setPleasant(ai_core_affect.getPleasant() + ai_pleasant);
        ai_core_affect.setAroused(ai_core_affect.getAroused() + ai_aroused);*/

        // CODICE PROVVISORIO!
        human_core_affect.setPleasant(human_pleasant);
        human_core_affect.setAroused(human_aroused);

        ai_core_affect.setPleasant(ai_pleasant);
        ai_core_affect.setAroused(ai_aroused);

        StartCoroutine(setting.setAnswer(transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().GetParsedText()));

        //StartCoroutine(setting.nextMessage());
    }

    public void setHumanPleasant(string pleasant)
    {
        this.human_pleasant = stringToFloat(pleasant, 0);
    }

    public void setHumanAroused(string aroused)
    {
        this.human_aroused = stringToFloat(aroused, 1);
    }

    public void setAiPleasant(string pleasant)
    {
        this.ai_pleasant = stringToFloat(pleasant, 0);
    }

    public void setAiAroused(string aroused)
    {
        this.ai_aroused = stringToFloat(aroused, 1);
    }

    private float stringToFloat(string status, int value)
    {
        float pleasant = 0;
        float aroused = 0;

        status.ToLower();

        if (status == "neutral")
        {
            pleasant = 0;
            aroused = 0;
        }
        else if (status == "sadness")
        {
            pleasant = -0.35f;
            aroused = -0.85f;
        }
        else if (status == "joy")
        {
            pleasant = 0.85f;
            aroused = 0.35f;
        }
        else if (status == "surprise")
        {
            pleasant = 0.35f;
            aroused = 0.85f;
        }
        else if (status == "anger")
        {
            pleasant = -0.85f;
            aroused = 0.35f;
        }
        else if (status == "fear")
        {
            pleasant = -0.35f;
            aroused = 0.85f;
        }
        else if (status == "disgust")
        {
            pleasant = -0.85f;
            aroused = -0.35f;
        }
        else if (status == "sleepiness")
        {
            pleasant = 0.35f;
            aroused = -0.85f;
        }
        else if (status == "calmness")
        {
            pleasant = 0.85f;
            aroused = -0.35f;
        }
        else
        {
            Debug.Log("Attenzione, non hai specificato un'espressione facciale! E' stata impostata in automatico su Neutrale..");
        }

        if (value == 0)
        {
            return pleasant;
        }
        else
        {
            return aroused;
        }
    }
}
