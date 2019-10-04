using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    public int[] emotions_1;
    public int[] emotions_2;

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
        human_core_affect.setEmotions(emotions_1);

        ai_core_affect.setEmotions(emotions_2);

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
}
