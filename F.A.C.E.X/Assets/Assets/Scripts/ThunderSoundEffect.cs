using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderSoundEffect : MonoBehaviour
{
    public GameObject thunder_light;
    private AudioSource thunder_sound;
    private Color thunder_color;
    public AuraAPI.LightFlicker aura_thunder_color;

    void Start()
    {
        thunder_sound = GetComponent<AudioSource>();
        thunder_color = thunder_light.GetComponent<Light>().color;
        thunder_light.GetComponent<Light>().color = new Color(0, 0, 0, 1);
        aura_thunder_color.baseColor = new Color(0, 0, 0, 1);
        StartCoroutine(ThunderON());
    }

    IEnumerator ThunderON()
    {
        while (true)
        {
            float frequency = Random.Range(3.0f, 10.0f);

            yield return new WaitForSeconds(frequency);

            thunder_sound.Play();

            yield return new WaitForSeconds(0.5f);

            //thunder_light.SetActive(true);
            thunder_light.GetComponent<Light>().color = thunder_color;
            aura_thunder_color.baseColor = thunder_color;

            yield return new WaitForSeconds(4.0f);

            float time = 0f;

            while (time <= 1)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / 1);

                thunder_light.GetComponent<Light>().color = new Color(thunder_light.GetComponent<Light>().color.r - percent, thunder_light.GetComponent<Light>().color.g - percent, thunder_light.GetComponent<Light>().color.b - percent, percent);
                aura_thunder_color.baseColor = new Color(thunder_light.GetComponent<Light>().color.r - percent, thunder_light.GetComponent<Light>().color.g - percent, thunder_light.GetComponent<Light>().color.b - percent, percent);

                yield return null;
            }
            
            //thunder_light.SetActive(false);
        }
    }

}
