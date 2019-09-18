using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Dialogues : MonoBehaviour
{
    public int dialogue_number; // 1 dialogue number = 2 possibili scelte

    public string[] human_answer_text;
    public string[] human_human_status;
    public string[] human_ai_status;

    public string[] ai_answer_text;
    public string[] ai_ai_status;
    public string[] ai_human_status;

    public GameObject left_button;
    public GameObject right_button;

    public TMPro.TextMeshProUGUI left_button_text;
    public TMPro.TextMeshProUGUI right_button_text;

    public GameObject final_mask;

    private static int answer_human_count = 0;
    private static int answer_ai_count = 0;
    private static Answer[] answer_human = new Answer[] { };
    private static Answer[] answer_ai = new Answer[] { };

    private void Start()
    {
        answer_human = new Answer[dialogue_number];
        answer_ai = new Answer[dialogue_number];

        if (human_answer_text.Length > 0)
        {
            for (int i = 0; i < dialogue_number; i++)
            {
                answer_human[i] = new Answer(human_answer_text[i], human_human_status[i], human_ai_status[i]);
            }
        }

        if (ai_answer_text.Length > 0)
        {
            for (int j = 0; j < dialogue_number; j++)
            {
                answer_ai[j] = new Answer(ai_answer_text[j], ai_human_status[j], ai_ai_status[j]);
            }
        }

        StartCoroutine(nextMessage());
    }

    public IEnumerator nextMessage()
    {
        if (answer_human_count <= answer_ai_count && answer_human_count < dialogue_number) // HUMAN CASE
        {
            if(answer_human_count != 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);

            left_button_text.SetText(answer_human[answer_human_count].getText());
            left_button.GetComponent<DialogueButton>().setHumanPleasant(answer_human[answer_human_count].getHumanStatus());
            left_button.GetComponent<DialogueButton>().setHumanAroused(answer_human[answer_human_count].getHumanStatus());
            left_button.GetComponent<DialogueButton>().setAiPleasant(answer_human[answer_human_count].getAiStatus());
            left_button.GetComponent<DialogueButton>().setAiAroused(answer_human[answer_human_count].getAiStatus());

            right_button_text.SetText(answer_human[answer_human_count + 1].getText());
            right_button.GetComponent<DialogueButton>().setHumanPleasant(answer_human[answer_human_count + 1].getHumanStatus());
            right_button.GetComponent<DialogueButton>().setHumanAroused(answer_human[answer_human_count + 1].getHumanStatus());
            right_button.GetComponent<DialogueButton>().setAiPleasant(answer_human[answer_human_count + 1].getAiStatus());
            right_button.GetComponent<DialogueButton>().setAiAroused(answer_human[answer_human_count + 1].getAiStatus());

            left_button.GetComponent<Button>().interactable = true;
            right_button.GetComponent<Button>().interactable = true;

            answer_human_count += 2;
        }
        else if (answer_ai_count < answer_human_count && answer_ai_count < dialogue_number) // AI CASE
        {
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);

            left_button.GetComponent<Button>().interactable = false;
            right_button.GetComponent<Button>().interactable = false;

            left_button_text.SetText(answer_ai[answer_ai_count].getText());
            left_button.GetComponent<DialogueButton>().setAiPleasant(answer_ai[answer_ai_count].getAiStatus());
            left_button.GetComponent<DialogueButton>().setAiAroused(answer_ai[answer_ai_count].getAiStatus());
            left_button.GetComponent<DialogueButton>().setHumanPleasant(answer_ai[answer_ai_count].getHumanStatus());
            left_button.GetComponent<DialogueButton>().setHumanAroused(answer_ai[answer_ai_count].getHumanStatus());

            right_button_text.SetText(answer_ai[answer_ai_count + 1].getText());
            right_button.GetComponent<DialogueButton>().setAiPleasant(answer_ai[answer_ai_count + 1].getAiStatus());
            right_button.GetComponent<DialogueButton>().setAiAroused(answer_ai[answer_ai_count + 1].getAiStatus());
            right_button.GetComponent<DialogueButton>().setHumanPleasant(answer_ai[answer_ai_count + 1].getHumanStatus());
            right_button.GetComponent<DialogueButton>().setHumanAroused(answer_ai[answer_ai_count + 1].getHumanStatus());

            StartCoroutine(AIResponse());

            answer_ai_count += 2;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Image>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            Debug.Log("Dialogo terminato!");
            //yield return new WaitForSeconds(2.0f);
            final_mask.SetActive(true);
            Debug.Log("Demo terminata!");
        }

        yield return null;
    }

    private IEnumerator AIResponse()
    {
        float reaction_time = UnityEngine.Random.Range(3.0f, 8.0f);

        yield return new WaitForSeconds(reaction_time);

        System.Random rnd = new System.Random();
        int choice = rnd.Next(1, 3);

        if (choice == 1)
        {
            //EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(left_button);
            //left_button.GetComponent<Button>().onClick.Invoke();
            //left_button.GetComponent<Button>().Select();
            left_button.GetComponent<Button>().interactable = true;
            left_button.GetComponent<Button>().OnSubmit(null);
        }
        else
        {
            //EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(right_button);
            //right_button.GetComponent<Button>().onClick.Invoke();
            right_button.GetComponent<Button>().interactable = true;
            right_button.GetComponent<Button>().OnSubmit(null);
        }

        yield return new WaitForSeconds(0.15f);

        left_button.GetComponent<Button>().interactable = false;
        right_button.GetComponent<Button>().interactable = false;
    }
}
