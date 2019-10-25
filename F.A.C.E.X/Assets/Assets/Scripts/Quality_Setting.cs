using System.Collections;
using System.Threading;
using UnityEngine;

public class Quality_Setting : MonoBehaviour
{
    public float Rate = 50.0f;
    float currentFrameTime;

    void Start()
    {
        // Switch to 1920 x 1080 windowed
        //Screen.SetResolution(1920, 1080, true); // OLD ,60
        //Screen.SetResolution(2048, 1152, true); // OLD ,60
        Screen.SetResolution(4096, 2160, true); // OLD ,60

        // Set quality setting to Very High Level
        QualitySettings.SetQualityLevel(5, true);
    }

    void Update()
    {
        switch (Input.inputString)
        {
            case "1":
                QualitySettings.SetQualityLevel(0, true);
                Debug.Log("Quality settings set to 'Fastest'");
                break;
            case "2":
                QualitySettings.SetQualityLevel(1, true);
                Debug.Log("Quality settings set to 'Fast'");
                break;
            case "3":
                QualitySettings.SetQualityLevel(2, true);
                Debug.Log("Quality settings set to 'Simple'");
                break;
            case "4":
                QualitySettings.SetQualityLevel(3, true);
                Debug.Log("Quality settings set to 'Good'");
                break;
            case "5":
                QualitySettings.SetQualityLevel(4, true);
                Debug.Log("Quality settings set to 'Beautiful'");
                break;
            case "6":
                QualitySettings.SetQualityLevel(5, true);
                Debug.Log("Quality settings set to 'Fantastic'");
                break;
            default:
                //Debug.Log("Button does not change the quality settings!");
                break;
        }
    }
}
