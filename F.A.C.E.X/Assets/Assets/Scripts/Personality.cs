using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour
{
    // Attributi Statici
    public int Scortese_Amichevole = 5; // Scortese = disonesto e con alte probabilità di Rabbia --- Amichevole = onesto e con alte probabilità di Gioia
    public int Negligente_Coscienzioso = 5; // Negligente = alte probabilità di sbagliare la risposta --- Coscienzioso = basse probabilità di sbagliare la risposta
    public int Emotivo_Stabile = 5; // Emotivo = alte probabilità di Rabbia, Paura, Tristezza --- Stabile = alte probabilità di Neutro, Calma

    // Attributi Dinamici
    public int Onestà = 0;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
