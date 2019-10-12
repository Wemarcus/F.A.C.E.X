using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Dialogues : MonoBehaviour
{
    public GameObject human_character;
    public GameObject ai_character;

    public bool first_character_is_NPC;
    public bool second_character_is_NPC;

    public static int dialogue_number = 10; // 1 dialogue number = 2 possibili scelte

    public string[] human_answer_text;
    public int[,] human_human_status = new int[dialogue_number, 9];
    public int[,] human_ai_status = new int[dialogue_number, 9];
    public int[] human_trigger = new int[dialogue_number];

    public string[] ai_answer_text;
    public int[,] ai_ai_status = new int[dialogue_number, 9];
    public int[,] ai_human_status = new int[dialogue_number, 9];
    public int[] ai_trigger = new int[dialogue_number];

    public int[] dialog_history = new int[dialogue_number]; // cronologia delle scelte (pulsanti) fatte nei dialoghi (1 = left_button, 2 = right_button)
    public int[] trigger_history = new int[dialogue_number]; // cronologia delle scelte (trigger) fatte nei dialoghi (0 = null, 1 = false, 2 = true)
    public int dialog_index = 0;

    public GameObject left_button;
    public GameObject right_button;
    public GameObject answer;
    public GameObject title_text;
    public GameObject skip;

    public TMPro.TextMeshProUGUI left_button_text;
    public TMPro.TextMeshProUGUI right_button_text;
    public TMPro.TextMeshProUGUI answer_text;

    public GameObject final_mask;

    private static int answer_human_count = 0;
    private static int answer_ai_count = 0;
    private static Answer[] answer_human = new Answer[] { };
    private static Answer[] answer_ai = new Answer[] { };
    private static bool speak_turn = false; // false = human , true = ai

    private void Start()
    {
        answer_human = new Answer[dialogue_number];
        answer_ai = new Answer[dialogue_number];

        if (human_answer_text.Length > 0)
        {
            for (int i = 0; i < dialogue_number; i++)
            {
                answer_human[i] = new Answer(human_answer_text[i], human_human_status.GetRow(i), human_ai_status.GetRow(i));
            }
        }

        if (ai_answer_text.Length > 0)
        {
            for (int j = 0; j < dialogue_number; j++)
            {
                answer_ai[j] = new Answer(ai_answer_text[j], ai_human_status.GetRow(j), ai_ai_status.GetRow(j));
            }
        }

        StartCoroutine(nextMessage());
    }

    public IEnumerator setAnswer(string text)
    {
        left_button.GetComponent<Image>().enabled = false;
        left_button.GetComponent<Button>().enabled = false;
        left_button_text.SetText("");
        right_button.GetComponent<Image>().enabled = false;
        right_button.GetComponent<Button>().enabled = false;
        right_button_text.SetText("");
        GetComponent<Image>().enabled = false;
        title_text.SetActive(false);
        answer.SetActive(true);
        answer_text.SetText(text);

        if (!speak_turn)
        {
            StartCoroutine(human_character.GetComponent<FacialExpressions>().Speaking());
        }
        else
        {
            StartCoroutine(ai_character.GetComponent<FacialExpressions>().Speaking());
        }

        yield return new WaitForSeconds(4.0f); // OLD 4.2f

        skip.SetActive(true);

        //StartCoroutine(nextMessage());
    }

    public IEnumerator nextMessage()
    {
        skip.SetActive(false);
        left_button.GetComponent<Image>().enabled = true;
        left_button.GetComponent<Button>().enabled = true;
        right_button.GetComponent<Image>().enabled = true;
        right_button.GetComponent<Button>().enabled = true;
        GetComponent<Image>().enabled = true;
        title_text.SetActive(true);
        answer.SetActive(false);

        if (answer_human_count <= answer_ai_count && answer_human_count < dialogue_number) // FIRST CHARACTER
        {
            speak_turn = false;
            //title_text.GetComponent<TMPro.TextMeshProUGUI>().SetText("Scegli una risposta:");

            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);

            if (first_character_is_NPC)
            {
                setAiUI();
                StartCoroutine(AIResponse(human_character, 0));
            }
            else
            {
                setHumanUI();
            }

            left_button_text.SetText(answer_human[answer_human_count].getText());
            left_button.GetComponent<DialogueButton>().setEmotions1(answer_human[answer_human_count].getHumanStatus());
            left_button.GetComponent<DialogueButton>().setEmotions2(answer_human[answer_human_count].getAiStatus());
            left_button.GetComponent<DialogueButton>().setTrigger(human_trigger[answer_human_count]);

            right_button_text.SetText(answer_human[answer_human_count + 1].getText());
            right_button.GetComponent<DialogueButton>().setEmotions1(answer_human[answer_human_count + 1].getHumanStatus());
            right_button.GetComponent<DialogueButton>().setEmotions2(answer_human[answer_human_count + 1].getAiStatus());
            right_button.GetComponent<DialogueButton>().setTrigger(human_trigger[answer_human_count + 1]);

            //left_button.GetComponent<Button>().interactable = true;
            //right_button.GetComponent<Button>().interactable = true;

            answer_human_count += 2;
        }
        else if (answer_ai_count < answer_human_count && answer_ai_count < dialogue_number) // SECOND CHARACTER
        {
            speak_turn = true;
            //title_text.GetComponent<TMPro.TextMeshProUGUI>().SetText("L'AI sta scegliendo una risposta..");

            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);

            //left_button.GetComponent<Button>().interactable = false;
            //right_button.GetComponent<Button>().interactable = false;

            if (second_character_is_NPC)
            {
                setAiUI();
                StartCoroutine(AIResponse(ai_character, 1));
            }
            else
            {
                setHumanUI();
            }

            left_button_text.SetText(answer_ai[answer_ai_count].getText());
            left_button.GetComponent<DialogueButton>().setEmotions1(answer_ai[answer_ai_count].getHumanStatus());
            left_button.GetComponent<DialogueButton>().setEmotions2(answer_ai[answer_ai_count].getAiStatus());
            left_button.GetComponent<DialogueButton>().setTrigger(ai_trigger[answer_ai_count]);

            right_button_text.SetText(answer_ai[answer_ai_count + 1].getText());
            right_button.GetComponent<DialogueButton>().setEmotions1(answer_ai[answer_ai_count + 1].getHumanStatus());
            right_button.GetComponent<DialogueButton>().setEmotions2(answer_ai[answer_ai_count + 1].getAiStatus());
            right_button.GetComponent<DialogueButton>().setTrigger(ai_trigger[answer_ai_count + 1]);

            //StartCoroutine(AIResponse());

            answer_ai_count += 2;
        }
        else
        {
            left_button.GetComponent<Image>().enabled = false;
            left_button.GetComponent<Button>().enabled = false;
            left_button_text.SetText("");
            right_button.GetComponent<Image>().enabled = false;
            right_button.GetComponent<Button>().enabled = false;
            right_button_text.SetText("");
            GetComponent<Image>().enabled = false;
            title_text.SetActive(false);
            answer.SetActive(false);

            Debug.Log("Dialogo terminato!");
            yield return new WaitForSeconds(2.0f);

            final_mask.SetActive(true);
            Debug.Log("Demo terminata!");
        }

        yield return null;
    }

    private IEnumerator AIResponse(GameObject character, int n)
    {
        float reaction_time = UnityEngine.Random.Range(3.0f, 8.0f);

        yield return new WaitForSeconds(reaction_time);

        int choice;
        
        if (n == 0)
        {
            choice = character.GetComponent<Personality>().chooseAnswer(dialog_index, left_button.GetComponent<DialogueButton>().emotions_1, right_button.GetComponent<DialogueButton>().emotions_1, left_button.GetComponent<DialogueButton>().trigger, right_button.GetComponent<DialogueButton>().trigger);
        }
        else
        {
            choice = character.GetComponent<Personality>().chooseAnswer(dialog_index, left_button.GetComponent<DialogueButton>().emotions_2, right_button.GetComponent<DialogueButton>().emotions_2, left_button.GetComponent<DialogueButton>().trigger, right_button.GetComponent<DialogueButton>().trigger);
        }

        if (choice == -1)
        {
            System.Random rnd = new System.Random();
            choice = rnd.Next(1, 3);
        }

        if (choice == 1)
        {
            trigger_history[dialog_index] = left_button.GetComponent<DialogueButton>().trigger;
            left_button.GetComponent<Button>().interactable = true;
            left_button.GetComponent<Button>().OnSubmit(null);    
        }
        else
        {
            trigger_history[dialog_index] = right_button.GetComponent<DialogueButton>().trigger;
            right_button.GetComponent<Button>().interactable = true;
            right_button.GetComponent<Button>().OnSubmit(null);
        }

        dialog_history[dialog_index] = choice;
        dialog_index++;

        yield return new WaitForSeconds(0.15f);

        left_button.GetComponent<Button>().interactable = false;
        right_button.GetComponent<Button>().interactable = false;
    }

    private void setHumanUI()
    {
        title_text.GetComponent<TMPro.TextMeshProUGUI>().SetText("Scegli una risposta:");
        left_button.GetComponent<Button>().interactable = true;
        right_button.GetComponent<Button>().interactable = true;
    }

    private void setAiUI()
    {
        title_text.GetComponent<TMPro.TextMeshProUGUI>().SetText("L'AI sta scegliendo una risposta..");
        left_button.GetComponent<Button>().interactable = false;
        right_button.GetComponent<Button>().interactable = false;
    }

    public void skipDialogue()
    {
        skip.SetActive(false);
        StartCoroutine(nextMessage());
    }
}
