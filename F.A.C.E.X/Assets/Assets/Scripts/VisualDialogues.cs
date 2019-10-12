using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;

public class VisualDialogues : MonoBehaviour
{
    public Dialogues dialoguesUI;

    private int[] emotions_1 = new int[9];
    private int[] emotions_2 = new int[9];
    private string phrase;
    private int trigger;

    private string[] first_character = new string[] { "1_Neutral", "1_Sadness", "1_Joy", "1_Surprise", "1_Anger", "1_Fear", "1_Disgust", "1_Sleepiness", "1_Calmness" };
    private string[] second_character = new string[] { "2_Neutral", "2_Sadness", "2_Joy", "2_Surprise", "2_Anger", "2_Fear", "2_Disgust", "2_Sleepiness", "2_Calmness" };

    private int first_character_count = 0;
    private int second_character_count = 0;

    private int human_human_count = 0;
    private int human_ai_count = 0;
    private int ai_ai_count = 0;
    private int ai_human_count = 0;

    void Awake()
    {
        gameObject.AddComponent<VD>();

        DialogueLoading();
    }

    void DialogueLoading()
    {
        VD.BeginDialogue(GetComponent<VIDE_Assign>());

        if (VD.isActive)
        {
            dialoguesUI.human_answer_text = new string[VD.GetNodeCount(false) / 2];
            dialoguesUI.ai_answer_text = new string[VD.GetNodeCount(false) / 2];

            var data = VD.nodeData;

            for(int i = 0; i < VD.GetNodeCount(false); i++)
            {
                phrase = data.comments[0];

                if (data.extraVars.ContainsKey("Trigger"))
                {
                    trigger = (int)data.extraVars["Trigger"];
                }
                else
                {
                    trigger = 0;
                }

                for (int j = 0; j < first_character.Length; j++)
                {
                    if (data.extraVars.ContainsKey(first_character[j]))
                    {
                        emotions_1[j] = (int)data.extraVars[first_character[j]];
                    }
                    else
                    {
                        emotions_1[j] = 0;
                    }

                    if (data.isPlayer)
                    {
                        dialoguesUI.human_human_status[human_human_count, j] = emotions_1[j];
                    }
                    else
                    {
                        dialoguesUI.ai_human_status[ai_human_count, j] = emotions_1[j];
                    }
                }

                for (int k = 0; k < second_character.Length; k++)
                {
                    if (data.extraVars.ContainsKey(second_character[k]))
                    {
                        emotions_2[k] = (int)data.extraVars[second_character[k]];
                    }
                    else
                    {
                        emotions_2[k] = 0;
                    }

                    if (data.isPlayer)
                    {
                        dialoguesUI.human_ai_status[human_ai_count, k] = emotions_2[k];
                    }
                    else
                    {
                        dialoguesUI.ai_ai_status[ai_ai_count, k] = emotions_2[k];
                    }
                }

                if (data.isPlayer)
                {
                    dialoguesUI.human_answer_text[first_character_count] = phrase;
                    dialoguesUI.human_trigger[first_character_count] = trigger;
                    first_character_count++;
                    human_human_count++;
                    human_ai_count++;
                }
                else
                {
                    dialoguesUI.ai_answer_text[second_character_count] = phrase;
                    dialoguesUI.ai_trigger[second_character_count] = trigger;
                    second_character_count++;
                    ai_ai_count++;
                    ai_human_count++;
                }

                VD.Next();
                data = VD.nodeData;
            }

            VD.EndDialogue();
            Debug.Log("Dialogo Caricato!");

        }
    }
}
