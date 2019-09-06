using System;
using UnityEngine;

public class CircumplexModel : MonoBehaviour
{

    void Start()
    {
        cordinate_print();
    }

    public double cordinate_print()
    {
        int number_of_chunks = 8; // OLD 8
        double degrees = 0;                      
        double[] x_p = new double[8]; // number of chunks 
        double[] y_p = new double[8]; // number of chunks 
        int radius = 1;                       

        for (int i = 0; i < number_of_chunks; i++)
        {
            degrees = i * (360 / number_of_chunks);
            float radian = (float) (degrees * (Math.PI / 180));
            x_p[i] = Math.Round(radius * Math.Cos(radian), 2);
            y_p[i] = Math.Round(radius * Math.Sin(radian), 2);

            Debug.Log("x -> " + x_p[i] + " y -> " + y_p[i] + " degrees -> " + degrees);
        }

        Debug.Log("TEST:");

        double radiant = Math.Atan2(1 - 0, 0 - 0);
        double angle = radiant * (180 / Math.PI);

        if (angle < 0.0)
            angle += 360.0;

        Debug.Log("Angle: " + angle);

        return 0;
    }

}
