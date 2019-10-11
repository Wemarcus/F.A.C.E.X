using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour
{
    // Attributi Statici (da -5 a 5)
    [Range(-5, 5)] public int Ostile_Amichevole = 0; // Ostile = predilige risposte che aumentano la Rabbia e il Disgusto --- Amichevole = predilige risposte che aumentano la Gioia e la Calma
    [Range(-5, 5)] public int Timoroso_Deciso = 0; // Timoroso = predilige risposte che aumentano la Paura  e la Tristezza --- Deciso = predilige risposte che aumentano la Calma e la Gioia OPPURE la Rabbia
    //[Range(-5, 5)] public int Ingenuo_Astuto = 0; // Ingenuo = maggiori probabilità di scegliere la risposta errata --- Astuto = maggiori probabilità di scegliere la risposta corretta
    [Range(-5, 5)] public int Insensibile_Emotivo = 0; // Emotivo = maggiori probabilità di Rabbia, Paura, Tristezza, Sorpresa, Disgusto --- Insensibile = maggiori probabilità di Neutro, Calma, Sonnolenza

    // Attributi Dinamici/Obiettivi
    //public int Onestà = 0;

    private CoreAffect emotions;

    void Start()
    {
        emotions = GetComponent<CoreAffect>();
    }

    public int chooseAnswer(int[] first_answer, int[] second_answer)
    {
        int first_answer_score = 0;
        int second_answer_score = 0;

        // Parametri per Ostile_Amichevole
        int first_JoyUCalmness = first_answer[2] + first_answer[8];
        int second_JoyUCalmness = second_answer[2] + second_answer[8];
        int first_AngerUDisgust = first_answer[4] + first_answer[6];
        int second_AngerUDisgust = second_answer[4] + second_answer[6];

        // Parametri per Timoroso_Deciso
        int first_SadnessUFear = first_answer[1] + first_answer[5];
        int second_SadnessUFear = second_answer[1] + second_answer[5];
        int first_Calmness = first_answer[8];
        int second_Calmness = second_answer[8];
        int first_Joy = first_answer[2];
        int second_Joy = second_answer[2];
        int first_Anger = first_answer[4];
        int second_Anger = second_answer[4];

        if (emotions.Neutral + emotions.Joy + emotions.Calmness > emotions.Anger + emotions.Disgust)
        {
            if (Ostile_Amichevole > 0)
            {
                if (first_JoyUCalmness >= 0)
                {
                    first_answer_score += emotions.Joy + emotions.Calmness + (first_JoyUCalmness * Mathf.Abs(Ostile_Amichevole));
                }
                else
                {
                    first_answer_score += emotions.Joy + emotions.Calmness + (first_JoyUCalmness / Mathf.Abs(Ostile_Amichevole));
                }

                if (second_JoyUCalmness >= 0)
                {
                    second_answer_score += emotions.Joy + emotions.Calmness + (second_JoyUCalmness * Mathf.Abs(Ostile_Amichevole));
                }
                else
                {
                    second_answer_score += emotions.Joy + emotions.Calmness + (second_JoyUCalmness / Mathf.Abs(Ostile_Amichevole));
                }

                Debug.Log("CALMO E AMICHEVOLE - First answer score: " + first_answer_score);
                Debug.Log("CALMO E AMICHEVOLE - Second answer score: " + second_answer_score);
            }
            else if (Ostile_Amichevole < 0)
            {
                if (first_AngerUDisgust >= 0)
                {
                    first_answer_score += (emotions.Joy + emotions.Calmness + (first_AngerUDisgust * Mathf.Abs(Ostile_Amichevole))) / 2;
                }
                else
                {
                    first_answer_score += (emotions.Joy + emotions.Calmness + (first_AngerUDisgust / Mathf.Abs(Ostile_Amichevole))) * 2;
                }

                if (second_AngerUDisgust >= 0)
                {
                    second_answer_score += (emotions.Joy + emotions.Calmness + (second_AngerUDisgust * Mathf.Abs(Ostile_Amichevole))) / 2;
                }
                else
                {
                    second_answer_score += (emotions.Joy + emotions.Calmness + (second_AngerUDisgust / Mathf.Abs(Ostile_Amichevole))) * 2;
                }

                Debug.Log("CALMO MA OSTILE - First answer score: " + first_answer_score);
                Debug.Log("CALMO MA OSTILE - Second answer score: " + second_answer_score);
            }
        }
        else if (emotions.Neutral + emotions.Anger + emotions.Disgust > emotions.Joy + emotions.Calmness)
        {
            if (Ostile_Amichevole > 0)
            {
                if (first_JoyUCalmness >= 0)
                {
                    first_answer_score += (emotions.Anger + emotions.Disgust + (first_JoyUCalmness * Mathf.Abs(Ostile_Amichevole))) / 2;
                }
                else
                {
                    first_answer_score += (emotions.Anger + emotions.Disgust + (first_JoyUCalmness / Mathf.Abs(Ostile_Amichevole))) * 2;
                }

                if (second_JoyUCalmness >= 0)
                {
                    second_answer_score += (emotions.Anger + emotions.Disgust + (second_JoyUCalmness * Mathf.Abs(Ostile_Amichevole))) / 2;
                }
                else
                {
                    second_answer_score += (emotions.Anger + emotions.Disgust + (second_JoyUCalmness / Mathf.Abs(Ostile_Amichevole))) * 2;
                }

                Debug.Log("ARRABBIATO MA AMICHEVOLE - First answer score: " + first_answer_score);
                Debug.Log("ARRABBIATO MA AMICHEVOLE - Second answer score: " + second_answer_score);
            }
            else if (Ostile_Amichevole < 0)
            {
                if (first_AngerUDisgust >= 0)
                {
                    first_answer_score += emotions.Anger + emotions.Disgust + (first_AngerUDisgust * Mathf.Abs(Ostile_Amichevole));
                }
                else
                {
                    first_answer_score += emotions.Anger + emotions.Disgust + (first_AngerUDisgust / Mathf.Abs(Ostile_Amichevole));
                }

                if (second_AngerUDisgust >= 0)
                {
                    second_answer_score += emotions.Anger + emotions.Disgust + (second_AngerUDisgust * Mathf.Abs(Ostile_Amichevole));
                }
                else
                {
                    second_answer_score += emotions.Anger + emotions.Disgust + (second_AngerUDisgust / Mathf.Abs(Ostile_Amichevole));
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
                    first_answer_score += ((first_JoyUCalmness + (first_AngerUDisgust / 2)) * Mathf.Abs(Ostile_Amichevole)) / 2;
                }
                else
                {
                    first_answer_score += ((first_JoyUCalmness + (first_AngerUDisgust / 2)) / Mathf.Abs(Ostile_Amichevole)) * 2;
                }

                if (second_JoyUCalmness >= 0)
                {
                    second_answer_score += ((second_JoyUCalmness + (second_AngerUDisgust / 2)) * Mathf.Abs(Ostile_Amichevole)) / 2;
                }
                else
                {
                    second_answer_score += ((second_JoyUCalmness + (second_AngerUDisgust / 2)) / Mathf.Abs(Ostile_Amichevole)) * 2;
                }

                Debug.Log("NEUTRALE E AMICHEVOLE - First answer score: " + first_answer_score);
                Debug.Log("NEUTRALE E AMICHEVOLE - Second answer score: " + second_answer_score);
            }
            else if (Ostile_Amichevole < 0)
            {
                if (first_AngerUDisgust >= 0)
                {
                    first_answer_score += (((first_JoyUCalmness / 2) + first_AngerUDisgust) * Mathf.Abs(Ostile_Amichevole)) / 2;
                }
                else
                {
                    first_answer_score += (((first_JoyUCalmness / 2) + first_AngerUDisgust) / Mathf.Abs(Ostile_Amichevole)) * 2;
                }

                if (second_AngerUDisgust >= 0)
                {
                    second_answer_score += (((second_JoyUCalmness / 2) + second_AngerUDisgust) * Mathf.Abs(Ostile_Amichevole)) / 2;
                }
                else
                {
                    second_answer_score += (((second_JoyUCalmness / 2) + second_AngerUDisgust) / Mathf.Abs(Ostile_Amichevole)) * 2;
                }

                Debug.Log("NEUTRALE E OSTILE - First answer score: " + first_answer_score);
                Debug.Log("NEUTRALE E OSTILE - Second answer score: " + second_answer_score);
            }
        }

        /*if (Ostile_Amichevole > 0)
        {
            if (emotions.Neutral + emotions.Joy + emotions.Calmness >= emotions.Anger + emotions.Disgust)
            {
                if (first_JoyUCalmness >= 0)
                {
                    first_answer_score += first_JoyUCalmness * Mathf.Abs(Ostile_Amichevole);
                }
                else
                {
                    first_answer_score += first_JoyUCalmness / Mathf.Abs(Ostile_Amichevole);
                }

                if (second_JoyUCalmness >= 0)
                {
                    second_answer_score += second_JoyUCalmness * Mathf.Abs(Ostile_Amichevole);
                }
                else
                {
                    second_answer_score += second_JoyUCalmness / Mathf.Abs(Ostile_Amichevole);
                }

                Debug.Log("AMICHEVOLE E CALMO - First answer score: " + first_answer_score);
                Debug.Log("AMICHEVOLE E CALMO - Second answer score: " + second_answer_score);
            }
            else
            {
                first_answer_score += - Mathf.Abs(first_JoyUCalmness) / Mathf.Abs(Ostile_Amichevole);
                second_answer_score += - Mathf.Abs(second_JoyUCalmness) / Mathf.Abs(Ostile_Amichevole);

                Debug.Log("AMICHEVOLE MA ARRABBIATO - First answer score: " + first_answer_score);
                Debug.Log("AMICHEVOLE MA ARRABBIATO - Second answer score: " + second_answer_score);
            }
        }
        else if (Ostile_Amichevole < 0)
        {
            if (emotions.Neutral + emotions.Anger + emotions.Disgust >= emotions.Joy + emotions.Calmness)
            {
                if (first_AngerUDisgust >= 0)
                {
                    first_answer_score += first_AngerUDisgust * Mathf.Abs(Ostile_Amichevole);
                }
                else
                {
                    first_answer_score += first_AngerUDisgust / Mathf.Abs(Ostile_Amichevole);
                }

                if (second_AngerUDisgust >= 0)
                {
                    second_answer_score += second_AngerUDisgust * Mathf.Abs(Ostile_Amichevole);
                }
                else
                {
                    second_answer_score += second_AngerUDisgust / Mathf.Abs(Ostile_Amichevole);
                }

                Debug.Log("OSTILE E ARRABBIATO - First answer score: " + first_answer_score);
                Debug.Log("OSTILE E ARRABBIATO - Second answer score: " + second_answer_score);
            }
            else
            {
                first_answer_score += - Mathf.Abs(first_AngerUDisgust) / Mathf.Abs(Ostile_Amichevole);
                second_answer_score += - Mathf.Abs(second_AngerUDisgust) / Mathf.Abs(Ostile_Amichevole);

                Debug.Log("OSTILE MA CALMO - First answer score: " + first_answer_score);
                Debug.Log("OSTILE MA CALMO - Second answer score: " + second_answer_score);
            }
        }*/

        //////////// NEW SOTTO

        if (emotions.Neutral + emotions.Calmness > emotions.Sadness + emotions.Fear)
        {
            if (Timoroso_Deciso > 0)
            {
                if (first_Calmness >= 0)
                {
                    first_answer_score += emotions.Calmness + ((first_Calmness + first_Joy) * Mathf.Abs(Timoroso_Deciso));
                }
                else
                {
                    first_answer_score += emotions.Calmness + ((first_Calmness + first_Joy) / Mathf.Abs(Timoroso_Deciso));
                }

                if (second_Calmness >= 0)
                {
                    second_answer_score += emotions.Calmness + ((second_Calmness + second_Joy) * Mathf.Abs(Timoroso_Deciso));
                }
                else
                {
                    second_answer_score += emotions.Calmness + ((second_Calmness + second_Joy) / Mathf.Abs(Timoroso_Deciso));
                }

                Debug.Log("TRANQUILLO E DECISO - First answer score: " + first_answer_score);
                Debug.Log("TRANQUILLO E DECISO - Second answer score: " + second_answer_score);
            } 
            else if (Timoroso_Deciso < 0)
            {
                if (first_SadnessUFear >= 0)
                {
                    first_answer_score += emotions.Calmness + ((first_SadnessUFear * Mathf.Abs(Timoroso_Deciso)) / 2);
                }
                else
                {
                    first_answer_score += emotions.Calmness + ((first_SadnessUFear / Mathf.Abs(Timoroso_Deciso)) * 2);
                }

                if (second_SadnessUFear >= 0)
                {
                    second_answer_score += emotions.Calmness + ((second_SadnessUFear * Mathf.Abs(Timoroso_Deciso)) / 2);
                }
                else
                {
                    second_answer_score += emotions.Calmness + ((second_SadnessUFear / Mathf.Abs(Timoroso_Deciso)) * 2);
                }

                Debug.Log("TRANQUILLO MA TIMOROSO - First answer score: " + first_answer_score);
                Debug.Log("TRANQUILLO MA TIMOROSO - Second answer score: " + second_answer_score);
            }
        }
        else if (emotions.Neutral + emotions.Sadness + emotions.Fear > emotions.Calmness)
        {
            if (Timoroso_Deciso > 0)
            {
                if (first_Calmness >= 0)
                {
                    first_answer_score += emotions.Sadness + emotions.Fear + ((first_Calmness * Mathf.Abs(Timoroso_Deciso)) / 2);
                }
                else
                {
                    first_answer_score += emotions.Sadness + emotions.Fear + ((first_Calmness / Mathf.Abs(Timoroso_Deciso)) * 2);
                }

                if (second_Calmness >= 0)
                {
                    second_answer_score += emotions.Sadness + emotions.Fear + ((second_Calmness * Mathf.Abs(Timoroso_Deciso)) / 2);
                }
                else
                {
                    second_answer_score += emotions.Sadness + emotions.Fear + ((second_Calmness / Mathf.Abs(Timoroso_Deciso)) * 2);
                }

                Debug.Log("IMPAURITO MA DECISO - First answer score: " + first_answer_score);
                Debug.Log("IMPAURITO MA DECISO - Second answer score: " + second_answer_score);
            }
            else if (Timoroso_Deciso < 0)
            {
                if (first_SadnessUFear >= 0)
                {
                    first_answer_score += emotions.Sadness + emotions.Fear + ((first_SadnessUFear + first_Anger) * Mathf.Abs(Timoroso_Deciso));
                }
                else
                {
                    first_answer_score += emotions.Sadness + emotions.Fear + ((first_SadnessUFear + first_Anger) / Mathf.Abs(Timoroso_Deciso));
                }

                if (second_SadnessUFear >= 0)
                {
                    second_answer_score += emotions.Sadness + emotions.Fear + ((second_SadnessUFear + second_Anger) * Mathf.Abs(Timoroso_Deciso));
                }
                else
                {
                    second_answer_score += emotions.Sadness + emotions.Fear + ((second_SadnessUFear + second_Anger) / Mathf.Abs(Timoroso_Deciso));
                }

                Debug.Log("IMPAURITO E TIMOROSO - First answer score: " + first_answer_score);
                Debug.Log("IMPAURITO E TIMOROSO - Second answer score: " + second_answer_score);
            }
        }
        else if (emotions.Calmness == emotions.Sadness + emotions.Fear)
        {
            if (Timoroso_Deciso > 0)
            {
                if (first_Calmness >= 0)
                {
                    first_answer_score += ((first_Calmness + (first_SadnessUFear/2)) * Mathf.Abs(Timoroso_Deciso)) / 2;
                }
                else
                {
                    first_answer_score += ((first_Calmness + (first_SadnessUFear/2)) / Mathf.Abs(Timoroso_Deciso)) * 2;
                }

                if (second_Calmness >= 0)
                {
                    second_answer_score += ((second_Calmness + (second_SadnessUFear/2)) * Mathf.Abs(Timoroso_Deciso)) / 2;
                }
                else
                {
                    second_answer_score += ((second_Calmness + (second_SadnessUFear/2)) / Mathf.Abs(Timoroso_Deciso)) * 2;
                }

                Debug.Log("NEUTRALE E DECISO - First answer score: " + first_answer_score);
                Debug.Log("NEUTRALE E DECISO - Second answer score: " + second_answer_score);
            }
            else if (Timoroso_Deciso < 0)
            {
                if (first_SadnessUFear >= 0)
                {
                    first_answer_score += (((first_Calmness/2) + first_SadnessUFear) * Mathf.Abs(Timoroso_Deciso)) / 2;
                }
                else
                {
                    first_answer_score += (((first_Calmness/2) + first_SadnessUFear) / Mathf.Abs(Timoroso_Deciso)) * 2;
                }

                if (second_SadnessUFear >= 0)
                {
                    second_answer_score += (((second_Calmness/2) + second_SadnessUFear) * Mathf.Abs(Timoroso_Deciso)) / 2;
                }
                else
                {
                    second_answer_score += (((second_Calmness/2) + second_SadnessUFear) / Mathf.Abs(Timoroso_Deciso)) * 2;
                }

                Debug.Log("NEUTRALE E TIMOROSO - First answer score: " + first_answer_score);
                Debug.Log("NEUTRALE E TIMOROSO - Second answer score: " + second_answer_score);
            }
        }

        //////////// OLD SOTTO

        /*if (Timoroso_Deciso > 0)
        {
            if (emotions.Neutral + emotions.Calmness > emotions.Fear)
            {
                if (first_Calmness >= 0)
                {
                    first_answer_score += first_Calmness * Mathf.Abs(Timoroso_Deciso);
                }
                else
                {
                    first_answer_score += first_Calmness / Mathf.Abs(Timoroso_Deciso);
                }

                if (second_Calmness >= 0)
                {
                    second_answer_score += second_Calmness * Mathf.Abs(Timoroso_Deciso);
                }
                else
                {
                    second_answer_score += second_Calmness / Mathf.Abs(Timoroso_Deciso);
                }

                Debug.Log("DECISO E CALMO - First answer score: " + first_answer_score);
                Debug.Log("DECISO E CALMO - Second answer score: " + second_answer_score);
            }
            else if (emotions.Neutral + emotions.Fear > emotions.Calmness)
            {
                first_answer_score += - Mathf.Abs(first_Calmness) / Mathf.Abs(Timoroso_Deciso);
                second_answer_score += - Mathf.Abs(second_Calmness) / Mathf.Abs(Timoroso_Deciso);

                Debug.Log("DECISO MA IMPAURITO - First answer score: " + first_answer_score);
                Debug.Log("DECISO MA IMPAURITO - Second answer score: " + second_answer_score);
            }
            else if (emotions.Calmness == emotions.Fear)
            {
                first_answer_score += first_Calmness + Mathf.Abs(Timoroso_Deciso);
                second_answer_score += second_Calmness + Mathf.Abs(Timoroso_Deciso);

                Debug.Log("DECISO E NEUTRALE - First answer score: " + first_answer_score);
                Debug.Log("DECISO E NEUTRALE - Second answer score: " + second_answer_score);
            }
        }
        else if (Timoroso_Deciso < 0)
        {
            if (emotions.Neutral + emotions.Fear >= emotions.Calmness)
            {
                if (first_Fear >= 0)
                {
                    first_answer_score += first_Fear * Mathf.Abs(Timoroso_Deciso);
                }
                else
                {
                    first_answer_score += first_Fear / Mathf.Abs(Timoroso_Deciso);
                }

                if (second_Fear >= 0)
                {
                    second_answer_score += second_Fear * Mathf.Abs(Timoroso_Deciso);
                }
                else
                {
                    second_answer_score += second_Fear / Mathf.Abs(Timoroso_Deciso);
                }

                Debug.Log("TIMOROSO E IMPAURITO - First answer score: " + first_answer_score);
                Debug.Log("TIMOROSO E IMPAURITO - Second answer score: " + second_answer_score);
            }
            else
            {
                first_answer_score += - Mathf.Abs(first_Fear) / Mathf.Abs(Timoroso_Deciso);
                second_answer_score += - Mathf.Abs(second_Fear) / Mathf.Abs(Timoroso_Deciso);

                Debug.Log("TIMOROSO MA CALMO - First answer score: " + first_answer_score);
                Debug.Log("TIMOROSO MA CALMO - Second answer score: " + second_answer_score);
            }
        }*/

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

    public int[] applyPersonality(int[] emotions)
    {
        if (Insensibile_Emotivo > 0)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i] != 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Diminuisco Neutral, Calmness e Sleepiness
                    {
                        //emotions[i] = emotions[i] - Mathf.Abs(Insensibile_Emotivo);
                        emotions[i] = emotions[i] - (Mathf.Abs(emotions[i]) * (Mathf.Abs(Insensibile_Emotivo)/10));
                    }
                    else // Aumento Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        //emotions[i] = emotions[i] + Mathf.Abs(Insensibile_Emotivo);
                        emotions[i] = emotions[i] + (Mathf.Abs(emotions[i]) * (Mathf.Abs(Insensibile_Emotivo) / 10));
                    }
                }

                /*if (emotions[i] > 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Diminuisco Neutral, Calmness e Sleepiness
                    {
                        emotions[i] = emotions[i] - Mathf.Abs(Insensibile_Emotivo);
                    }
                    else // Aumento Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        emotions[i] = emotions[i] + Mathf.Abs(Insensibile_Emotivo);
                    }
                } else if (emotions[i] < 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Diminuisco Neutral, Calmness e Sleepiness
                    {
                        emotions[i] = emotions[i] - Insensibile_Emotivo;
                    }
                    else // Aumento Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        emotions[i] = emotions[i] + Insensibile_Emotivo;
                    }
                }*/
            }
        }
        else if (Insensibile_Emotivo < 0)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i] != 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Aumento Neutral, Calmness e Sleepiness
                    {
                        //emotions[i] = emotions[i] + Mathf.Abs(Insensibile_Emotivo);
                        emotions[i] = emotions[i] + (Mathf.Abs(emotions[i]) * (Mathf.Abs(Insensibile_Emotivo) / 10));
                    }
                    else // Diminuisco Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        //emotions[i] = emotions[i] - Mathf.Abs(Insensibile_Emotivo);
                        emotions[i] = emotions[i] - (Mathf.Abs(emotions[i]) * (Mathf.Abs(Insensibile_Emotivo) / 10));
                    }
                }

                /*if (emotions[i] > 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Aumento Neutral, Calmness e Sleepiness
                    {
                        emotions[i] = emotions[i] + Mathf.Abs(Insensibile_Emotivo);
                    }
                    else // Diminuisco Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        emotions[i] = emotions[i] - Mathf.Abs(Insensibile_Emotivo);
                    }
                }*/

            }
        }

        return emotions;
    }
}
