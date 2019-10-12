using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    public int[] emotions_1;
    public int[] emotions_2;

    public int trigger; // utilizzato per risposte particolari..

    public Dialogues setting;

    public GameObject human_character;
    public GameObject ai_character;

    private CoreAffect human_core_affect;
    private CoreAffect ai_core_affect;
    private Personality human_personality;
    private Personality ai_personality;

    private void Start()
    {
        human_core_affect = human_character.GetComponent<CoreAffect>();
        human_personality = human_character.GetComponent<Personality>();
        ai_core_affect = ai_character.GetComponent<CoreAffect>();
        ai_personality = ai_character.GetComponent<Personality>();
    }

    public void setStatus()
    {
        human_core_affect.setEmotions(human_personality.applyPersonality(emotions_1));

        ai_core_affect.setEmotions(ai_personality.applyPersonality(emotions_2));

        StartCoroutine(setting.setAnswer(transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().GetParsedText()));
    }

    public void setEmotions1(int[] emotions)
    {
        emotions_1 = emotions;
    }

    public void setEmotions2(int[] emotions)
    {
        emotions_2 = emotions;
    }

    public void setTrigger(int value)
    {
        trigger = value;
    }
}
