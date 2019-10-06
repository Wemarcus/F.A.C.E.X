using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour
{
    // Attributi Statici (da -5 a 5)
    //[Range(-5, 5)] public int Scortese_Amichevole = 0; // Scortese = disonesto e con alte probabilità di Rabbia --- Amichevole = onesto e con alte probabilità di Gioia
    //[Range(-5, 5)] public int Negligente_Coscienzioso = 0; // Negligente = alte probabilità di sbagliare la risposta --- Coscienzioso = basse probabilità di sbagliare la risposta
    [Range(-5, 5)] public int Insensibile_Emotivo = 0; // Emotivo = alte probabilità di Rabbia, Paura, Tristezza --- Insensibile = alte probabilità di Neutro, Calma

    // Attributi Dinamici
    //public int Onestà = 0;

    public int[] applyPersonality(int[] emotions)
    {
        if (Insensibile_Emotivo > 0)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i] > 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Diminuisco Neutral, Calmness e Sleepiness
                    {
                        emotions[i] = emotions[i] - Insensibile_Emotivo;
                    }
                    else // Aumento Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        emotions[i] = emotions[i] + Insensibile_Emotivo;
                    }
                } /*else if (emotions[i] < 0)
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
                if (emotions[i] > 0)
                {
                    if (i == 0 || i == emotions.Length - 1 || i == emotions.Length - 2) // Aumento Neutral, Calmness e Sleepiness
                    {
                        emotions[i] = emotions[i] - Insensibile_Emotivo;
                    }
                    else // Diminuisco Sadness, Joy, Surprise, Anger, Fear, Disgust
                    {
                        emotions[i] = emotions[i] + Insensibile_Emotivo;
                    }
                }

            }
        }

        return emotions;
    }
}
