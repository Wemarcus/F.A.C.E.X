using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour
{
    // Attributi Statici (da -5 a 5)
    [Range(-5, 5)] public int Ostile_Amichevole = 0; // Ostile = predilige risposte che aumentano la Rabbia e il Disgusto --- Amichevole = predilige risposte che aumentano la Gioia e la Calma
    [Range(-5, 5)] public int Timoroso_Deciso = 0; // Timoroso = predilige risposte che aumentano la Paura --- Deciso = predilige risposte che aumentano la Calma e la Gioia OPPURE la Rabbia
    [Range(-5, 5)] public int Malinconico_Allegro = 0; // Malinconico = predilige risposte che aumentano la Tristezza --- Allegro = predilige risposte che aumentano la Gioia
    //[Range(-5, 5)] public int Ingenuo_Astuto = 0; // Ingenuo = maggiori probabilità di scegliere la risposta errata --- Astuto = maggiori probabilità di scegliere la risposta corretta
    [Range(-5, 5)] public int Insensibile_Emotivo = 0; // Insensibile = maggiori probabilità di Neutro e Sonnolenza --- Emotivo = maggiori probabilità di Rabbia, Paura, Tristezza, Sorpresa, Disgusto, Gioia e Calma

    // Attributi Dinamici/Obiettivi
    //public int Onestà = 0;

    public Dialogues dialogues;
    private CoreAffect emotions;

    void Start()
    {
        emotions = GetComponent<CoreAffect>();
    }

    public int chooseAnswer(int dialog_index, int[] first_answer, int[] second_answer, int first_trigger, int second_trigger)
    {
        int first_answer_score = 0;
        int second_answer_score = 0;

        // Parametri per Ostile_Amichevole
        int first_JoyUCalmness = first_answer[2] + first_answer[8];
        int second_JoyUCalmness = second_answer[2] + second_answer[8];
        int first_AngerUDisgust = first_answer[4] + first_answer[6];
        int second_AngerUDisgust = second_answer[4] + second_answer[6];

        // Parametri per Timoroso_Deciso
        int first_Fear = first_answer[5];
        int second_Fear = second_answer[5];
        int first_Calmness = first_answer[8];
        int second_Calmness = second_answer[8];

        // Parametri per Malinconico_Allegro
        int first_Joy = first_answer[2];
        int second_Joy = second_answer[2];
        int first_Sadness = first_answer[1];
        int second_Sadness = second_answer[1];

        if (dialog_index > 0 && dialogues.trigger_history[dialog_index - 1] != 0 && dialogues.trigger_history[dialog_index-1] == first_trigger)
        {
            return 1;
        }
        else if (dialog_index > 0 && dialogues.trigger_history[dialog_index - 1] != 0 && dialogues.trigger_history[dialog_index-1] == second_trigger)
        {
            return 2;
        }
        else
        {
            // CASO: OSTILE-AMICHEVOLE

            if (emotions.Joy + emotions.Calmness > emotions.Anger + emotions.Disgust) // OLD emotions.Neutral + emotions.Joy + emotions.Calmness > emotions.Anger + emotions.Disgust
            {
                if (Ostile_Amichevole > 0)
                {
                    if (first_JoyUCalmness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Joy + emotions.Calmness + ((float)first_JoyUCalmness * Mathf.Abs(Ostile_Amichevole)));
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Joy + emotions.Calmness + ((float)first_JoyUCalmness / Mathf.Abs(Ostile_Amichevole)));
                    }

                    if (second_JoyUCalmness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Joy + emotions.Calmness + ((float)second_JoyUCalmness * Mathf.Abs(Ostile_Amichevole)));
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Joy + emotions.Calmness + ((float)second_JoyUCalmness / Mathf.Abs(Ostile_Amichevole)));
                    }

                    Debug.Log("CALMO E AMICHEVOLE - First answer score: " + first_answer_score);
                    Debug.Log("CALMO E AMICHEVOLE - Second answer score: " + second_answer_score);
                }
                else if (Ostile_Amichevole < 0)
                {
                    if (first_AngerUDisgust >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Joy + emotions.Calmness + ((float)first_AngerUDisgust * Mathf.Abs(Ostile_Amichevole))) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Joy + emotions.Calmness + ((float)first_AngerUDisgust / Mathf.Abs(Ostile_Amichevole))) * 2);
                    }

                    if (second_AngerUDisgust >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Joy + emotions.Calmness + ((float)second_AngerUDisgust * Mathf.Abs(Ostile_Amichevole))) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Joy + emotions.Calmness + ((float)second_AngerUDisgust / Mathf.Abs(Ostile_Amichevole))) * 2);
                    }

                    Debug.Log("CALMO MA OSTILE - First answer score: " + first_answer_score);
                    Debug.Log("CALMO MA OSTILE - Second answer score: " + second_answer_score);
                }
            }
            else if (emotions.Anger + emotions.Disgust > emotions.Joy + emotions.Calmness) // OLD emotions.Neutral + emotions.Anger + emotions.Disgust > emotions.Joy + emotions.Calmness
            {
                if (Ostile_Amichevole > 0)
                {
                    if (first_JoyUCalmness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Anger + emotions.Disgust + ((float)first_JoyUCalmness * Mathf.Abs(Ostile_Amichevole))) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Anger + emotions.Disgust + ((float)first_JoyUCalmness / Mathf.Abs(Ostile_Amichevole))) * 2);
                    }

                    if (second_JoyUCalmness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Anger + emotions.Disgust + ((float)second_JoyUCalmness * Mathf.Abs(Ostile_Amichevole))) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Anger + emotions.Disgust + ((float)second_JoyUCalmness / Mathf.Abs(Ostile_Amichevole))) * 2);
                    }

                    Debug.Log("ARRABBIATO MA AMICHEVOLE - First answer score: " + first_answer_score);
                    Debug.Log("ARRABBIATO MA AMICHEVOLE - Second answer score: " + second_answer_score);
                }
                else if (Ostile_Amichevole < 0)
                {
                    if (first_AngerUDisgust >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Anger + emotions.Disgust + ((float)first_AngerUDisgust * Mathf.Abs(Ostile_Amichevole)));
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Anger + emotions.Disgust + ((float)first_AngerUDisgust / Mathf.Abs(Ostile_Amichevole)));
                    }

                    if (second_AngerUDisgust >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Anger + emotions.Disgust + ((float)second_AngerUDisgust * Mathf.Abs(Ostile_Amichevole)));
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Anger + emotions.Disgust + ((float)second_AngerUDisgust / Mathf.Abs(Ostile_Amichevole)));
                    }

                    Debug.Log("ARRABBIATO E OSTILE - First answer score: " + first_answer_score);
                    Debug.Log("ARRABBIATO E OSTILE - Second answer score: " + second_answer_score);
                }
            }
            else if (emotions.Joy + emotions.Calmness == emotions.Anger + emotions.Disgust)
            {
                if (Ostile_Amichevole > 0)
                {
                    if (first_JoyUCalmness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(((first_JoyUCalmness + ((float)first_AngerUDisgust / 2)) * Mathf.Abs(Ostile_Amichevole)) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(((first_JoyUCalmness + ((float)first_AngerUDisgust / 2)) / Mathf.Abs(Ostile_Amichevole)) * 2);
                    }

                    if (second_JoyUCalmness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(((second_JoyUCalmness + ((float)second_AngerUDisgust / 2)) * Mathf.Abs(Ostile_Amichevole)) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(((second_JoyUCalmness + ((float)second_AngerUDisgust / 2)) / Mathf.Abs(Ostile_Amichevole)) * 2);
                    }

                    Debug.Log("NEUTRALE E AMICHEVOLE - First answer score: " + first_answer_score);
                    Debug.Log("NEUTRALE E AMICHEVOLE - Second answer score: " + second_answer_score);
                }
                else if (Ostile_Amichevole < 0)
                {
                    if (first_AngerUDisgust >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(((((float)first_JoyUCalmness / 2) + first_AngerUDisgust) * Mathf.Abs(Ostile_Amichevole)) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(((((float)first_JoyUCalmness / 2) + first_AngerUDisgust) / Mathf.Abs(Ostile_Amichevole)) * 2);
                    }

                    if (second_AngerUDisgust >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(((((float)second_JoyUCalmness / 2) + second_AngerUDisgust) * Mathf.Abs(Ostile_Amichevole)) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(((((float)second_JoyUCalmness / 2) + second_AngerUDisgust) / Mathf.Abs(Ostile_Amichevole)) * 2);
                    }

                    Debug.Log("NEUTRALE E OSTILE - First answer score: " + first_answer_score);
                    Debug.Log("NEUTRALE E OSTILE - Second answer score: " + second_answer_score);
                }
            }

            // CASO: TIMOROSO-DECISO

            if (emotions.Calmness > emotions.Fear) // OLD emotions.Neutral + emotions.Calmness > emotions.Sadness + emotions.Fear
            {
                if (Timoroso_Deciso > 0)
                {
                    if (first_Calmness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Calmness + ((float)first_Calmness * Mathf.Abs(Timoroso_Deciso)));
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Calmness + ((float)first_Calmness / Mathf.Abs(Timoroso_Deciso)));
                    }

                    if (second_Calmness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Calmness + ((float)second_Calmness * Mathf.Abs(Timoroso_Deciso)));
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Calmness + ((float)second_Calmness / Mathf.Abs(Timoroso_Deciso)));
                    }

                    Debug.Log("TRANQUILLO E DECISO - First answer score: " + first_answer_score);
                    Debug.Log("TRANQUILLO E DECISO - Second answer score: " + second_answer_score);
                }
                else if (Timoroso_Deciso < 0)
                {
                    if (first_Fear >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Calmness + ((float)first_Fear * Mathf.Abs(Timoroso_Deciso))) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Calmness + ((float)first_Fear / Mathf.Abs(Timoroso_Deciso))) * 2);
                    }

                    if (second_Fear >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Calmness + ((float)second_Fear * Mathf.Abs(Timoroso_Deciso))) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Calmness + ((float)second_Fear / Mathf.Abs(Timoroso_Deciso))) * 2);
                    }

                    Debug.Log("TRANQUILLO MA TIMOROSO - First answer score: " + first_answer_score);
                    Debug.Log("TRANQUILLO MA TIMOROSO - Second answer score: " + second_answer_score);
                }
            }
            else if (emotions.Fear > emotions.Calmness) // OLD emotions.Neutral + emotions.Sadness + emotions.Fear > emotions.Calmness
            {
                if (Timoroso_Deciso > 0)
                {
                    if (first_Calmness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Fear + ((float)first_Calmness * Mathf.Abs(Timoroso_Deciso))) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Fear + ((float)first_Calmness / Mathf.Abs(Timoroso_Deciso))) * 2);
                    }

                    if (second_Calmness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Fear + ((float)second_Calmness * Mathf.Abs(Timoroso_Deciso))) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Fear + ((float)second_Calmness / Mathf.Abs(Timoroso_Deciso))) * 2);
                    }

                    Debug.Log("IMPAURITO MA DECISO - First answer score: " + first_answer_score);
                    Debug.Log("IMPAURITO MA DECISO - Second answer score: " + second_answer_score);
                }
                else if (Timoroso_Deciso < 0)
                {
                    if (first_Fear >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Fear + ((float)first_Fear * Mathf.Abs(Timoroso_Deciso)));
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Fear + ((float)first_Fear / Mathf.Abs(Timoroso_Deciso)));
                    }

                    if (second_Fear >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Fear + ((float)second_Fear * Mathf.Abs(Timoroso_Deciso)));
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Fear + ((float)second_Fear / Mathf.Abs(Timoroso_Deciso)));
                    }

                    Debug.Log("IMPAURITO E TIMOROSO - First answer score: " + first_answer_score);
                    Debug.Log("IMPAURITO E TIMOROSO - Second answer score: " + second_answer_score);
                }
            }
            else if (emotions.Calmness == emotions.Fear)
            {
                if (Timoroso_Deciso > 0)
                {
                    if (first_Calmness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(((first_Calmness + ((float)first_Fear / 2)) * Mathf.Abs(Timoroso_Deciso)) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(((first_Calmness + ((float)first_Fear / 2)) / Mathf.Abs(Timoroso_Deciso)) * 2);
                    }

                    if (second_Calmness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(((second_Calmness + ((float)second_Fear / 2)) * Mathf.Abs(Timoroso_Deciso)) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(((second_Calmness + ((float)second_Fear / 2)) / Mathf.Abs(Timoroso_Deciso)) * 2);
                    }

                    Debug.Log("NEUTRALE E DECISO - First answer score: " + first_answer_score);
                    Debug.Log("NEUTRALE E DECISO - Second answer score: " + second_answer_score);
                }
                else if (Timoroso_Deciso < 0)
                {
                    if (first_Fear >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(((((float)first_Calmness / 2) + first_Fear) * Mathf.Abs(Timoroso_Deciso)) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(((((float)first_Calmness / 2) + first_Fear) / Mathf.Abs(Timoroso_Deciso)) * 2);
                    }

                    if (second_Fear >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(((((float)second_Calmness / 2) + second_Fear) * Mathf.Abs(Timoroso_Deciso)) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(((((float)second_Calmness / 2) + second_Fear) / Mathf.Abs(Timoroso_Deciso)) * 2);
                    }

                    Debug.Log("NEUTRALE E TIMOROSO - First answer score: " + first_answer_score);
                    Debug.Log("NEUTRALE E TIMOROSO - Second answer score: " + second_answer_score);
                }
            }

            // CASO: MALINCONICO-ALLEGRO

            if (emotions.Joy > emotions.Sadness)
            {
                if (Malinconico_Allegro > 0)
                {
                    if (first_Joy >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Joy + ((float)first_Joy * Mathf.Abs(Malinconico_Allegro)));
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Joy + ((float)first_Joy / Mathf.Abs(Malinconico_Allegro)));
                    }

                    if (second_Joy >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Joy + ((float)second_Joy * Mathf.Abs(Malinconico_Allegro)));
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Joy + ((float)second_Joy / Mathf.Abs(Malinconico_Allegro)));
                    }

                    Debug.Log("GIOIOSO E ALLEGRO - First answer score: " + first_answer_score);
                    Debug.Log("GIOIOSO E ALLEGRO - Second answer score: " + second_answer_score);
                }
                else if (Malinconico_Allegro < 0)
                {
                    if (first_Sadness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Joy + ((float)first_Sadness * Mathf.Abs(Malinconico_Allegro))) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Joy+ ((float)first_Sadness / Mathf.Abs(Malinconico_Allegro))) * 2);
                    }

                    if (second_Sadness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Joy + ((float)second_Sadness * Mathf.Abs(Malinconico_Allegro))) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Joy + ((float)second_Sadness / Mathf.Abs(Malinconico_Allegro))) * 2);
                    }

                    Debug.Log("GIOIOSO MA MALINCONICO - First answer score: " + first_answer_score);
                    Debug.Log("GIOIOSO MA MALINCONICO - Second answer score: " + second_answer_score);
                }
            }
            else if (emotions.Sadness > emotions.Joy)
            {
                if (Malinconico_Allegro > 0)
                {
                    if (first_Joy >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Sadness + ((float)first_Joy * Mathf.Abs(Malinconico_Allegro))) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt((emotions.Sadness + ((float)first_Joy / Mathf.Abs(Malinconico_Allegro))) * 2);
                    }

                    if (second_Joy >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Sadness + ((float)second_Joy * Mathf.Abs(Malinconico_Allegro))) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt((emotions.Sadness + ((float)second_Joy / Mathf.Abs(Malinconico_Allegro))) * 2);
                    }

                    Debug.Log("TRISTE MA ALLEGRO - First answer score: " + first_answer_score);
                    Debug.Log("TRISTE MA ALLEGRO - Second answer score: " + second_answer_score);
                }
                else if (Malinconico_Allegro < 0)
                {
                    if (first_Sadness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Sadness + ((float)first_Sadness * Mathf.Abs(Malinconico_Allegro)));
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(emotions.Sadness + ((float)first_Sadness / Mathf.Abs(Malinconico_Allegro)));
                    }

                    if (second_Sadness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Sadness + ((float)second_Sadness * Mathf.Abs(Malinconico_Allegro)));
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(emotions.Sadness + ((float)second_Sadness / Mathf.Abs(Malinconico_Allegro)));
                    }

                    Debug.Log("TRISTE E MALINCONICO - First answer score: " + first_answer_score);
                    Debug.Log("TRISTE E MALINCONICO - Second answer score: " + second_answer_score);
                }
            }
            else if (emotions.Joy == emotions.Sadness)
            {
                if (Malinconico_Allegro > 0)
                {
                    if (first_Joy >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(((first_Joy + ((float)first_Sadness / 2)) * Mathf.Abs(Malinconico_Allegro)) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(((first_Joy + ((float)first_Sadness / 2)) / Mathf.Abs(Malinconico_Allegro)) * 2);
                    }

                    if (second_Joy >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(((second_Joy + ((float)second_Sadness / 2)) * Mathf.Abs(Malinconico_Allegro)) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(((second_Joy + ((float)second_Sadness / 2)) / Mathf.Abs(Malinconico_Allegro)) * 2);
                    }

                    Debug.Log("NEUTRALE E ALLEGRO - First answer score: " + first_answer_score);
                    Debug.Log("NEUTRALE E ALLEGRO - Second answer score: " + second_answer_score);
                }
                else if (Malinconico_Allegro < 0)
                {
                    if (first_Sadness >= 0)
                    {
                        first_answer_score += Mathf.RoundToInt(((((float)first_Joy / 2) + first_Sadness) * Mathf.Abs(Malinconico_Allegro)) / 2);
                    }
                    else
                    {
                        first_answer_score += Mathf.RoundToInt(((((float)first_Joy / 2) + first_Sadness) / Mathf.Abs(Malinconico_Allegro)) * 2);
                    }

                    if (second_Sadness >= 0)
                    {
                        second_answer_score += Mathf.RoundToInt(((((float)second_Joy / 2) + second_Sadness) * Mathf.Abs(Malinconico_Allegro)) / 2);
                    }
                    else
                    {
                        second_answer_score += Mathf.RoundToInt(((((float)second_Joy / 2) + second_Sadness) / Mathf.Abs(Malinconico_Allegro)) * 2);
                    }

                    Debug.Log("NEUTRALE E MALINCONICO - First answer score: " + first_answer_score);
                    Debug.Log("NEUTRALE E MALINCONICO - Second answer score: " + second_answer_score);
                }
            }

            if (first_answer_score > second_answer_score)
            {
                return 1;
            }
            else if (first_answer_score < second_answer_score)
            {
                return 2;
            }
            else
            {
                return -1; // risultato uguale, scelta random!
            }
        }
    }

    public int[] applyPersonality(int[] emotions)
    {
        //Debug.Log("Prima: " + emotions[0] + emotions[1] + emotions[2] + emotions[3] + emotions[4] + emotions[5] + emotions[6] + emotions[7] + emotions[8]);

        if (Insensibile_Emotivo > 0)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i] != 0)
                {
                    if (i == 0 || i == 7) // Diminuisco Neutral e Sleepiness
                    {
                        emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                    }

                    if (Ostile_Amichevole > 0)
                    {
                        if (i == 2 || i == 8) // Aumento Joy e Calmness                        
                        {
                            emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Ostile_Amichevole < 0)
                    {
                        if (i == 4 || i == 6) // Aumento Anger e Disgust                     
                        {
                            emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Timoroso_Deciso > 0)
                    {
                        if (i == 8) // Aumento Calmness                      
                        {
                            emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Timoroso_Deciso < 0)
                    {
                        if (i == 5) // Aumento Fear                      
                        {
                            emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Malinconico_Allegro > 0)
                    {
                        if (i == 2) // Aumento Joy                      
                        {
                            emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Malinconico_Allegro < 0)
                    {
                        if (i == 1) // Aumento Sadness                      
                        {
                            emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (i == 3) // Aumento Surprise
                    {
                        emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                    }
                }
            }
        }

        if (Insensibile_Emotivo < 0)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i] != 0)
                {
                    if (i == 0 || i == 7) // Aumento Neutral e Sleepiness
                    {
                        emotions[i] = emotions[i] + Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                    }

                    if (Ostile_Amichevole > 0)
                    {
                        if (i == 2 || i == 8) // Diminuisco Joy e Calmness                       
                        {
                            emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Ostile_Amichevole < 0)
                    {
                        if (i == 4 || i == 6) // Diminuisco Anger e Disgust                     
                        {
                            emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Timoroso_Deciso > 0)
                    {
                        if (i == 8) // Diminuisco Calmness                      
                        {
                            emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Timoroso_Deciso < 0)
                    {
                        if (i == 5) // Diminuisco Fear                      
                        {
                            emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Malinconico_Allegro > 0)
                    {
                        if (i == 2) // Diminuisco Joy                      
                        {
                            emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (Malinconico_Allegro < 0)
                    {
                        if (i == 1) // Diminuisco Sadness                      
                        {
                            emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                        }
                    }

                    if (i == 3) // Diminuisco Surprise 
                    {
                        emotions[i] = emotions[i] - Mathf.RoundToInt((Mathf.Abs(emotions[i]) * ((float)Mathf.Abs(Insensibile_Emotivo) / 5)));
                    }
                }
            }
        }

        //Debug.Log("Dopo: " + emotions[0] + emotions[1] + emotions[2] + emotions[3] + emotions[4] + emotions[5] + emotions[6] + emotions[7] + emotions[8]);

        return emotions;
    }
}
