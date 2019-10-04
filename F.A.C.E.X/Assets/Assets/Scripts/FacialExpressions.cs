using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using System;
using System.Linq;

public class FacialExpressions : MonoBehaviour
{
    public SkinnedMeshRenderer face;
    public SkinnedMeshRenderer eyelashes;
    public Mesh body;
    public Mesh eyelashes_mesh;
    public Mesh moustaches_mesh;
    public GameObject[] tears;
    public SkinnedMeshRenderer moustaches;
    public GameObject left_eye;
    public GameObject right_eye;
    public GameObject head;
    //public Material hair_material;
    //public Material eyelashes_material;
    //public Material eyelashes_backfaces_material;
    public GameObject interlocutor;
    private LookAtConstraint left_lookAtConstraint;
    private LookAtConstraint right_lookAtConstraint;
    private LookAtConstraint head_lookAtConstraint;

    [SerializeField]
    private AnimationCurve animationCurve;

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private AnimationCurve end_curve;

    [SerializeField]
    private AnimationCurve exp_curve;

    private bool blinking = false;
    private float blink_frequency = 3.5f;
    private int emotion_id = -1; // -1 = Nothing
    private bool eyes_movement = false;
    private float eyes_frequency = 0.2f; // OLD 2.0
    private bool cry = false;
    private bool head_movement = false;
    private float head_frequency = 0.5f;

    private float eyes_up_down = 0;
    private float eyes_left_right = 0;
    private float head_up_down = 0;
    private float head_left_right = 0;
    private float head_rotation = 0;

    private int[] sadness_face = new int[] { 0, 1, 4, 5, 6, 7, 10, 11, 14, 15, 17, 26, 27, 28, 32 };
    private int[] sadness_eyelashes = new int[] { 0, 1, 4, 5, 6, 7 };
    private int[] sadness_moustaches = new int[] { 5, 6, 9, 10, 12, 21, 22, 23, 27 };

    private int[] joy_face = new int[] { 6, 7, 10, 11, 12, 13, 26, 27, 30, 31, 33, 34, 35, 37, 38, 39, 40, 41, 42, 43, 44, 46, 48, 49 };
    private int[] joy_eyelashes = new int[] { 6, 7, 12, 13, 43, 44 };
    private int[] joy_moustaches = new int[] { 5, 6, 7, 8, 21, 22, 25, 26, 28, 29, 30, 32, 33, 34, 35, 36, 37, 38, 40, 41 };

    private int[] surprise_face = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 22, 26, 27, 28, 29, 30, 31, 33, 34, 35, 39, 40, 48, 49 };
    private int[] surprise_eyelashes = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 12, 13 };
    private int[] surprise_moustaches = new int[] { 5, 6, 9, 10, 17, 21, 22, 23, 24, 25, 26, 28, 29, 30, 34, 35, 40, 41 };

    private int[] anger_face = new int[] { 2, 3, 6, 7, 10, 11, 12, 13, 14, 15, 25, 28, 35, 36, 39, 40, 43, 44, 46 };
    private int[] anger_eyelashes = new int[] { 2, 3, 6, 7, 12, 13, 43, 44 };
    private int[] anger_moustaches = new int[] { 5, 6, 9, 10, 20, 23, 30, 31, 34, 35, 38 };

    private int[] fear_face = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15, 16, 22, 26, 27, 30, 31, 32, 33, 34, 39, 40, 41, 42, 46, 48, 49 };
    private int[] fear_eyelashes = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 12, 13 };
    private int[] fear_moustaches = new int[] { 9, 10, 11, 17, 21, 22, 25, 26, 27, 28, 29, 34, 35, 36, 37, 38, 40, 41 };

    private int[] disgust_face = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 14, 15, 17, 25, 26, 27, 28, 29, 33, 34, 36, 39, 40, 43, 44, 47, 48, 49 }; // NEW 28
    private int[] disgust_eyelashes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 43, 44 };
    private int[] disgust_moustaches = new int[] { 9, 10, 12, 20, 23, 24, 28, 29, 31, 34, 35, 39, 40, 41 }; // NEW 23

    private bool animation_lock = true; // true = libera, false = occupata
    private bool animation_interpolation = true; // true = in azione, false = ferma
    private bool speak = false; // true = parlando, false = non parlando
    private bool listen = false; // true = in ascolto, false = non in ascolto

    private Coroutine coroutine_Eyes;
    private Coroutine coroutine_Head;

    void Awake()
    {
        left_lookAtConstraint = left_eye.GetComponent<LookAtConstraint>();
        right_lookAtConstraint = right_eye.GetComponent<LookAtConstraint>();
        head_lookAtConstraint = head.GetComponent<LookAtConstraint>();

        // Fix for Fade Shader
        //hair_material.SetInt("_ZWrite", 1);
        //eyelashes_material.SetInt("_ZWrite", 1);
        //eyelashes_backfaces_material.SetInt("_ZWrite", 1);

        // Active Standard Blinking
        blinking = true;
        StartCoroutine(Blink(0.3f));

        // Active Standard Eyes Movement
        eyes_movement = true;
        coroutine_Eyes = StartCoroutine(Eyes(0.8f)); //OLD 1.5

        // Active Standard Head Movement
        head_movement = true;
        coroutine_Head = StartCoroutine(Head(1.5f));

        // Active Standard Animation ===> OLD dall'introduzione della FSM!
        // StartCoroutine(Neutral_Animation());
    }

    // Update is called once per frame
    void Update()
    {
        // 0.Neutral
        if (Input.GetKeyDown(KeyCode.Alpha0) && emotion_id != 0)
        {
            emotion_id = 0;
            StartCoroutine(Neutral(0.5f));
        }

        // 1.Sadness
        if (Input.GetKeyDown(KeyCode.Alpha1) && emotion_id != 1)
        {
            emotion_id = 1;
            StartCoroutine(Sadness(0.5f));
        }

        // 2.Joy
        if (Input.GetKeyDown(KeyCode.Alpha2) && emotion_id != 2)
        {
            emotion_id = 2;
            StartCoroutine(Joy(0.5f));
        }

        // 3.Surprise
        if (Input.GetKeyDown(KeyCode.Alpha3) && emotion_id != 3)
        {
            emotion_id = 3;
            StartCoroutine(Surprise(0.5f));
        }

        // 4.Anger
        if (Input.GetKeyDown(KeyCode.Alpha4) && emotion_id != 4)
        {
            emotion_id = 4;
            StartCoroutine(Anger(0.5f));
        }

        // 5.Fear
        if (Input.GetKeyDown(KeyCode.Alpha5) && emotion_id != 5)
        {
            emotion_id = 5;
            StartCoroutine(Fear(0.5f));
        }

        // 6.Disgust
        if (Input.GetKeyDown(KeyCode.Alpha6) && emotion_id != 6)
        {
            emotion_id = 6;
            StartCoroutine(Disgust(0.5f));
        }

        // Speak
        if (Input.GetKeyDown(KeyCode.S) && !speak)
        {
            speak = true;
            StartCoroutine(Speaking());
        }
    }

    IEnumerator Head(float duration)
    {
        while (head_movement)
        {
            yield return new WaitForSeconds(head_frequency); // impostato a 0 quando si cambia espressione facciale o quando si parla!

            float time = 0f;

            System.Random rnd = new System.Random();

            if (!speak && !listen)
            {
                if (emotion_id == 0) // Neutral
                {
                    duration = 3.0f; // OLD 2.5
                    head_frequency = UnityEngine.Random.Range(1.5f, 2.5f); // OLD 1.0 2.0

                    if (eyes_up_down >= 0)
                    {
                        head_up_down = UnityEngine.Random.Range(0.0f, 15.0f);
                    }
                    else
                    {
                        head_up_down = UnityEngine.Random.Range(-15.0f, 0.0f);
                    }

                    if (eyes_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 30.0f);
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-30.0f, 0.0f);
                    }

                    head_rotation = UnityEngine.Random.Range(-5.0f, 5.0f);

                    while (time <= duration && emotion_id == 0)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 1) // Sadness
                {
                    duration = 3.0f;
                    head_frequency = UnityEngine.Random.Range(0f, 0.5f); // OLD 1.0 2.0 // OLD 1.5 3.0

                    if (head_up_down >= 10)
                    {
                        head_up_down = UnityEngine.Random.Range(5.0f, 10.0f);
                    }
                    else
                    {
                        head_up_down = UnityEngine.Random.Range(10.0f, 15.0f);
                    }

                    if (eyes_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 10.0f); // OLD 0.0 15.0
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-10.0f, 0.0f); // OLD -15.0 0.0
                    }

                    head_rotation = UnityEngine.Random.Range(-5.0f, 5.0f);

                    while (time <= duration && emotion_id == 1)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 2) // Joy
                {
                    duration = 2.5f;
                    head_frequency = UnityEngine.Random.Range(1f, 2.0f);

                    if (eyes_up_down >= 0)
                    {
                        head_up_down = UnityEngine.Random.Range(-10.0f, -5.0f); // OLD -5.0f 0.0f
                    }
                    else
                    {
                        head_up_down = UnityEngine.Random.Range(-15.0f, -10.0f); // OLD -10.0f -5.0f
                    }

                    if (eyes_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 10.0f); // OLD 0.0f 15.0f
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-10.0f, 0.0f); // OLD -15.0f 0.0f
                    }

                    head_rotation = UnityEngine.Random.Range(-5.0f, 5.0f);

                    while (time <= duration && emotion_id == 2)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 3) // Surprise
                {
                    duration = 3.0f;
                    head_frequency = UnityEngine.Random.Range(1.0f, 2.0f);

                    head_up_down = UnityEngine.Random.Range(-10.0f, 0.0f);

                    if (eyes_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 2.0f);
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-2.0f, 0.0f);
                    }

                    head_rotation = UnityEngine.Random.Range(-1.0f, 1.0f);

                    while (time <= duration && emotion_id == 3)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 4) // Anger
                {
                    duration = 2.0f; // OLD 3.0
                    head_frequency = UnityEngine.Random.Range(0.0f, 1.5f); // OLD 1.0 2.0

                    if (eyes_up_down >= 0)
                    {
                        head_up_down = UnityEngine.Random.Range(-9.0f, -6.0f);
                    }
                    else
                    {
                        head_up_down = UnityEngine.Random.Range(-12.0f, -9.0f);
                    }

                    if (eyes_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 5.0f);
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-5.0f, 0.0f);
                    }

                    head_rotation = UnityEngine.Random.Range(-5.0f, 5.0f);

                    while (time <= duration && emotion_id == 4)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 5) // Fear
                {
                    duration = 2.0f;
                    head_frequency = UnityEngine.Random.Range(0.5f, 1.0f);

                    head_up_down = UnityEngine.Random.Range(-10.0f, -8.0f);

                    if (head_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 5.0f);
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-5.0f, 0.0f);
                    }

                    head_rotation = UnityEngine.Random.Range(-2.0f, 2.0f);

                    while (time <= duration && emotion_id == 5)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 6) // Disgust
                {
                    duration = 1.5f; // OLD 1.0 // OLD 3.0 => forse meglio 1.5
                    head_frequency = UnityEngine.Random.Range(0.5f, 1.0f); // OLD 1.0 2.0

                    /*if (eyes_up_down <= 0) // OLD >= 0
                    {
                        head_up_down = UnityEngine.Random.Range(-2.0f, 0.0f);
                    }
                    else
                    {
                        head_up_down = UnityEngine.Random.Range(-5.0f, -2.0f);
                    }*/

                    head_up_down = UnityEngine.Random.Range(-15.0f, -10.0f); // OLD -13.0 -8.0

                    if (head_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(-3.0f, 0.0f);
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 3.0f);
                    }

                    head_rotation = UnityEngine.Random.Range(-0.5f, 0.5f);

                    while (time <= duration && emotion_id == 6)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                        }

                        if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                        {
                            float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                        }
                        else
                        {
                            float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                            head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                        }

                        yield return null;
                    }
                }
            }
            else
            {
                duration = 1.5f;
                head_frequency = UnityEngine.Random.Range(1.5f, 2.5f);

                if(emotion_id == 1)
                {
                    head_up_down = UnityEngine.Random.Range(0.0f, 5.0f);

                    if (eyes_left_right >= 0)
                    {
                        head_left_right = UnityEngine.Random.Range(0.0f, 10.0f); // OLD 0.0 15.0
                    }
                    else
                    {
                        head_left_right = UnityEngine.Random.Range(-10.0f, 0.0f); // OLD -15.0 0.0
                    }

                    head_rotation = UnityEngine.Random.Range(-5.0f, 5.0f);
                }
                else
                {
                    head_up_down = UnityEngine.Random.Range(-15.0f, -13.0f);
                    head_left_right = UnityEngine.Random.Range(-2.0f, 2.0f);
                    head_rotation = UnityEngine.Random.Range(-2.0f, 2.0f);
                }

                while (time <= duration && (speak || listen))
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    if (head_up_down > head_lookAtConstraint.rotationAtRest.x)
                    {
                        float difference = head_up_down - head_lookAtConstraint.rotationAtRest.x;
                        head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                    }
                    else
                    {
                        float difference = head_lookAtConstraint.rotationAtRest.x - head_up_down;
                        head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z);
                    }

                    if (head_left_right > head_lookAtConstraint.rotationAtRest.y)
                    {
                        float difference = head_left_right - head_lookAtConstraint.rotationAtRest.y;
                        head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                    }
                    else
                    {
                        float difference = head_lookAtConstraint.rotationAtRest.y - head_left_right;
                        head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, head_lookAtConstraint.rotationAtRest.z);
                    }

                    if (head_rotation > head_lookAtConstraint.rotationAtRest.z)
                    {
                        float difference = head_rotation - head_lookAtConstraint.rotationAtRest.z;
                        head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z + curve.Evaluate(percent) * difference);
                    }
                    else
                    {
                        float difference = head_lookAtConstraint.rotationAtRest.z - head_rotation;
                        head_lookAtConstraint.rotationAtRest = new Vector3(head_lookAtConstraint.rotationAtRest.x, head_lookAtConstraint.rotationAtRest.y, head_lookAtConstraint.rotationAtRest.z - curve.Evaluate(percent) * difference);
                    }

                    yield return null;
                }
            }
        }
    }

    IEnumerator Eyes(float duration)
    {
        while (eyes_movement)
        {
            yield return new WaitForSeconds(eyes_frequency); // impostato a 0 quando si cambia espressione facciale!

            float time = 0f;
            System.Random rnd = new System.Random();

            if (!speak && !listen)
            {
                if (left_lookAtConstraint.weight > 0.0f || right_lookAtConstraint.weight > 0.0f)
                {
                    time = 0f;
                    duration = 1.0f;
                    float value = 1.0f;

                    while (time <= duration)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        left_lookAtConstraint.weight = end_curve.Evaluate(percent) * value;
                        right_lookAtConstraint.weight = end_curve.Evaluate(percent) * value;

                        yield return null;
                    }
                }

                time = 0f;

                if (emotion_id == 0) // Neutral
                {
                    eyes_frequency = UnityEngine.Random.Range(0.0f, 7.0f); // OLD 0.2 5.0
                    eyes_up_down = UnityEngine.Random.Range(-5.0f, 15.0f); // OLD -5 15 // OLD -5 8 // OLD -10 10
                    eyes_left_right = UnityEngine.Random.Range(-20.0f, 20.0f); // OLD -25 25

                    while (time <= duration && emotion_id == 0)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (eyes_up_down > left_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = eyes_up_down - left_lookAtConstraint.rotationAtRest.x;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.x - eyes_up_down;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }

                        if (eyes_left_right > left_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = eyes_left_right - left_lookAtConstraint.rotationAtRest.y;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.y - eyes_left_right;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 1) // Sadness
                {
                    eyes_frequency = UnityEngine.Random.Range(0.0f, 4.0f);
                    eyes_up_down = UnityEngine.Random.Range(-1.5f, 3.5f);
                    eyes_left_right = UnityEngine.Random.Range(-6.0f, 6.0f);

                    while (time <= duration && emotion_id == 1)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (eyes_up_down > left_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = eyes_up_down - left_lookAtConstraint.rotationAtRest.x;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.x - eyes_up_down;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }

                        if (eyes_left_right > left_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = eyes_left_right - left_lookAtConstraint.rotationAtRest.y;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.y - eyes_left_right;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 2) // Joy
                {
                    eyes_frequency = UnityEngine.Random.Range(0.0f, 4.0f); // OLD 0.0 7.0
                    eyes_up_down = UnityEngine.Random.Range(-2.0f, 8.0f);
                    eyes_left_right = UnityEngine.Random.Range(-8.0f, 5.0f);

                    while (time <= duration && emotion_id == 2)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (eyes_up_down > left_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = eyes_up_down - left_lookAtConstraint.rotationAtRest.x;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.x - eyes_up_down;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }

                        if (eyes_left_right > left_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = eyes_left_right - left_lookAtConstraint.rotationAtRest.y;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.y - eyes_left_right;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 3) // Surprise
                {
                    eyes_frequency = UnityEngine.Random.Range(0.0f, 4.0f); // OLD 2.0 4.0
                    eyes_up_down = UnityEngine.Random.Range(-1.5f, 3.5f); // OLD -1.0 3.0
                    eyes_left_right = UnityEngine.Random.Range(-3.0f, 3.0f); // OLD -2.0 2.0

                    while (time <= duration && emotion_id == 3)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (eyes_up_down > left_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = eyes_up_down - left_lookAtConstraint.rotationAtRest.x;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.x - eyes_up_down;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }

                        if (eyes_left_right > left_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = eyes_left_right - left_lookAtConstraint.rotationAtRest.y;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.y - eyes_left_right;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 4 && emotion_id == 6) // Anger & Disgust
                {
                    eyes_frequency = UnityEngine.Random.Range(0.0f, 4.0f);
                    eyes_up_down = UnityEngine.Random.Range(-2.0f, 8.0f);
                    eyes_left_right = UnityEngine.Random.Range(-8.0f, 3.0f);

                    while (time <= duration && (emotion_id == 4 || emotion_id == 6))
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (eyes_up_down > left_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = eyes_up_down - left_lookAtConstraint.rotationAtRest.x;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.x - eyes_up_down;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }

                        if (eyes_left_right > left_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = eyes_left_right - left_lookAtConstraint.rotationAtRest.y;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.y - eyes_left_right;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        }

                        yield return null;
                    }
                }
                else if (emotion_id == 5) // Fear
                {
                    eyes_frequency = UnityEngine.Random.Range(0.0f, 2.0f); // OLD 0.0 4.0
                    eyes_up_down = UnityEngine.Random.Range(9.0f, 12.0f); // OLD 9.0 11.0
                    eyes_left_right = UnityEngine.Random.Range(-7.0f, 5.0f); // OLD -5.0 3.0

                    while (time <= duration && emotion_id == 5)
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        if (eyes_up_down > left_lookAtConstraint.rotationAtRest.x)
                        {
                            float difference = eyes_up_down - left_lookAtConstraint.rotationAtRest.x;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.x - eyes_up_down;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        }

                        if (eyes_left_right > left_lookAtConstraint.rotationAtRest.y)
                        {
                            float difference = eyes_left_right - left_lookAtConstraint.rotationAtRest.y;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        }
                        else
                        {
                            float difference = left_lookAtConstraint.rotationAtRest.y - eyes_left_right;
                            left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        }

                        yield return null;
                    }
                }
                else // Other Case (Momentaneo)
                {
                    if (left_lookAtConstraint.rotationAtRest.x != 0 || left_lookAtConstraint.rotationAtRest.y != 0)
                    {
                        while (time <= duration)
                        {
                            time = time + Time.deltaTime;
                            float percent = Mathf.Clamp01(time / duration);

                            left_lookAtConstraint.rotationAtRest = new Vector3(end_curve.Evaluate(percent) * left_lookAtConstraint.rotationAtRest.x, end_curve.Evaluate(percent) * left_lookAtConstraint.rotationAtRest.y, 0);
                            right_lookAtConstraint.rotationAtRest = new Vector3(end_curve.Evaluate(percent) * left_lookAtConstraint.rotationAtRest.x, end_curve.Evaluate(percent) * left_lookAtConstraint.rotationAtRest.y, 0);

                            yield return null;
                        }
                    }
                }
            }
            else
            {
                if ((left_lookAtConstraint.weight < 1.0f || right_lookAtConstraint.weight < 1.0f) && emotion_id != 1) // No durante Sadness
                {
                    time = 0f;
                    duration = 1.0f;
                    float value = 1.0f;

                    while (time <= duration && (speak || listen))
                    {
                        time = time + Time.deltaTime;
                        float percent = Mathf.Clamp01(time / duration);

                        left_lookAtConstraint.weight = curve.Evaluate(percent) * value;
                        right_lookAtConstraint.weight = curve.Evaluate(percent) * value;

                        yield return null;
                    }
                }
            }
        }
    }

    IEnumerator Blink(float duration)
    {
        while (blinking)
        {
            yield return new WaitForSeconds(blink_frequency);

            float time = 0f;

            System.Random rnd = new System.Random();

            while (time <= duration)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                face.SetBlendShapeWeight(0, (animationCurve.Evaluate(percent) * 80)); // OLD 100
                face.SetBlendShapeWeight(1, (animationCurve.Evaluate(percent) * 80)); // OLD 100
                eyelashes.SetBlendShapeWeight(0, (animationCurve.Evaluate(percent) * 80)); // OLD 100
                eyelashes.SetBlendShapeWeight(1, (animationCurve.Evaluate(percent) * 80)); // OLD 100

                yield return null;
            }

            // Special cases

            if (emotion_id == 1) // sadness
            {
                int Blink = (int)rnd.Next(20, 30);

                face.SetBlendShapeWeight(0, Blink); // Blink_Left
                face.SetBlendShapeWeight(1, Blink); // Blink_Right
                eyelashes.SetBlendShapeWeight(0, Blink); // Blink_Left
                eyelashes.SetBlendShapeWeight(1, Blink); // Blink_Right
            } else if (emotion_id == 2) // joy
            {
                int Squint = (int)rnd.Next(50, 70); // OLD 70 80

                face.SetBlendShapeWeight(43, Squint); // Squint_Left
                face.SetBlendShapeWeight(44, Squint); // Squint_Right
                eyelashes.SetBlendShapeWeight(43, Squint); // Squint_Left
                eyelashes.SetBlendShapeWeight(44, Squint); // Squint_Right
            } else if (emotion_id == 3) // surprise
            {
                int EyesWide = (int)rnd.Next(120, 150);

                face.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                face.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
                eyelashes.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                eyelashes.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
            } else if (emotion_id == 4) // anger
            {
                int EyesWide = (int)rnd.Next(0, 80); // NEW
                int Squint = (int)rnd.Next(0, 30); // NEW

                face.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                face.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right               
                face.SetBlendShapeWeight(43, Squint); // Squint_Left
                face.SetBlendShapeWeight(44, Squint); // Squint_Right
                eyelashes.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                eyelashes.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
                eyelashes.SetBlendShapeWeight(43, Squint); // Squint_Left
                eyelashes.SetBlendShapeWeight(44, Squint); // Squint_Right
            } else if (emotion_id == 5) // fear
            {
                int EyesWide = 100;

                face.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                face.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
                eyelashes.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                eyelashes.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
            } else if (emotion_id == 6) // disgust
            {
                int Blink = (int)rnd.Next(0, 5);
                int Squint_Left = (int)rnd.Next(100, 140); // OLD 110 140
                int Squint_Right = (int)rnd.Next(100, 140); // OLD 110 140

                face.SetBlendShapeWeight(0, Blink); // Blink_Left
                face.SetBlendShapeWeight(1, Blink); // Blink_Right
                face.SetBlendShapeWeight(43, Squint_Left); // Squint_Left
                face.SetBlendShapeWeight(44, Squint_Right); // Squint_Right
                eyelashes.SetBlendShapeWeight(0, Blink); // Blink_Left
                eyelashes.SetBlendShapeWeight(1, Blink); // Blink_Right
                eyelashes.SetBlendShapeWeight(43, Squint_Left); // Squint_Left
                eyelashes.SetBlendShapeWeight(44, Squint_Right); // Squint_Right
            }
        }
    }

    IEnumerator Neutral(float duration)
    {
        StartCoroutine(tightenEyes());

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 3.5f;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        float time = 0f;

        while (time <= duration && emotion_id == 0)
        {
            time = time + Time.deltaTime;
            float percent = Mathf.Clamp01(time / duration);

            for (int i = 0; i < body.blendShapeCount; i++)
            {
                face.SetBlendShapeWeight(i, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(i)));
            }

            for (int j = 0; j < eyelashes_mesh.blendShapeCount; j++)
            {
                eyelashes.SetBlendShapeWeight(j, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(j)));
            }

            if (moustaches_mesh != null)
            {
                for (int k = 0; k < moustaches_mesh.blendShapeCount; k++)
                {
                    moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                }
            }

            yield return null;
        }

        StartCoroutine(Neutral_Animation());
    }

    IEnumerator Neutral_Animation()
    {
        System.Random rnd = new System.Random();
        float duration = 1.5f; // OLD 2.0

        //Debug.Log("Neutral_Animation - START");

        while (emotion_id == 0)
        {
            if (animation_lock)
            {
                animation_lock = false;

                //Debug.Log("Neutral_Animation - MODIFY");

                float animation_frequency = UnityEngine.Random.Range(1.0f, 2.0f);

                yield return new WaitForSeconds(animation_frequency);

                int CheekPuff_Left;
                int CheekPuff_Right;
                int Midmouth_Left;
                int Midmouth_Right;
                int MouthNarrow_Left;
                int MouthNarrow_Right;
                int MouthUp;
                int NoseScrunch_Left;
                int NoseScrunch_Right;
                int LowerLipIn;
                int Smile_Left;
                int Smile_Right;
                int UpperLipIn;
                int BrowsUp_Left;
                int BrowsUp_Right;
                int BrowsDown_Left;
                int BrowsDown_Right;
                int BrowsOuterLower_Left;
                int BrowsOuterLower_Right;

                int[] neutral_micro_expressions;
                int[] face_index;
                int[] moustaches_index;
                float[] original_expressions;

                float time = 0f;
                int category = (int)rnd.Next(1, 8); // OLD 1 10

                switch (category)
                {
                    case 1: // movimento bocca
                        CheekPuff_Left = (int)rnd.Next(10, 30); // OLD 20 40
                        CheekPuff_Right = (int)rnd.Next(10, 30); // OLD 20 40
                        Midmouth_Left = (int)rnd.Next(40, 100); // OLD 60 100
                        Midmouth_Right = (int)rnd.Next(40, 100); // OLD 60 100
                        MouthNarrow_Left = (int)rnd.Next(0, 40); // OLD 50 100
                        MouthNarrow_Right = (int)rnd.Next(0, 40); // OLD 50 100
                        MouthUp = (int)rnd.Next(10, 20);
                        NoseScrunch_Left = (int)rnd.Next(40, 60);
                        NoseScrunch_Right = (int)rnd.Next(40, 60);

                        neutral_micro_expressions = new int[] { CheekPuff_Left, CheekPuff_Right, Midmouth_Left, Midmouth_Right, MouthNarrow_Left,
                                                                  MouthNarrow_Right, MouthUp, NoseScrunch_Left, NoseScrunch_Right };
                        face_index = new int[] { 10, 11, 30, 31, 33, 34, 36, 39, 40 };
                        moustaches_index = new int[] { 5, 6, 25, 26, 28, 29, 31, 34, 35 };
                        original_expressions = new float[9];

                        for (int k = 0; k < face_index.Length; k++)
                        {
                            original_expressions[k] = face.GetBlendShapeWeight(face_index[k]);
                        }

                        int random = (int)rnd.Next(1, 3); // compreso tra 1 e 2

                        while (time <= duration && emotion_id == 0)
                        {
                            time = time + Time.deltaTime;
                            float percent = Mathf.Clamp01(time / duration);

                            if (random == 1)
                            {
                                face.SetBlendShapeWeight(face_index[0], (original_expressions[0] + exp_curve.Evaluate(percent) * neutral_micro_expressions[0]));
                                face.SetBlendShapeWeight(face_index[2], (original_expressions[2] + exp_curve.Evaluate(percent) * neutral_micro_expressions[2]));
                                face.SetBlendShapeWeight(face_index[4], (original_expressions[4] + exp_curve.Evaluate(percent) * neutral_micro_expressions[4]));
                                face.SetBlendShapeWeight(face_index[6], (original_expressions[6] + exp_curve.Evaluate(percent) * neutral_micro_expressions[6]));
                                face.SetBlendShapeWeight(face_index[7], (original_expressions[7] + exp_curve.Evaluate(percent) * neutral_micro_expressions[7]));

                                if (moustaches != null)
                                {
                                    moustaches.SetBlendShapeWeight(moustaches_index[0], (original_expressions[0] + exp_curve.Evaluate(percent) * neutral_micro_expressions[0]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[2], (original_expressions[2] + exp_curve.Evaluate(percent) * neutral_micro_expressions[2]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[4], (original_expressions[4] + exp_curve.Evaluate(percent) * neutral_micro_expressions[4]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[6], (original_expressions[6] + exp_curve.Evaluate(percent) * neutral_micro_expressions[6]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[7], (original_expressions[7] + exp_curve.Evaluate(percent) * neutral_micro_expressions[7]));
                                }
                            }
                            else
                            {
                                face.SetBlendShapeWeight(face_index[1], (original_expressions[1] + exp_curve.Evaluate(percent) * neutral_micro_expressions[1]));
                                face.SetBlendShapeWeight(face_index[3], (original_expressions[3] + exp_curve.Evaluate(percent) * neutral_micro_expressions[3]));
                                face.SetBlendShapeWeight(face_index[5], (original_expressions[5] + exp_curve.Evaluate(percent) * neutral_micro_expressions[5]));
                                face.SetBlendShapeWeight(face_index[6], (original_expressions[6] + exp_curve.Evaluate(percent) * neutral_micro_expressions[6]));
                                face.SetBlendShapeWeight(face_index[8], (original_expressions[8] + exp_curve.Evaluate(percent) * neutral_micro_expressions[8]));

                                if (moustaches != null)
                                {
                                    moustaches.SetBlendShapeWeight(moustaches_index[1], (original_expressions[1] + exp_curve.Evaluate(percent) * neutral_micro_expressions[1]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[3], (original_expressions[3] + exp_curve.Evaluate(percent) * neutral_micro_expressions[3]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[5], (original_expressions[5] + exp_curve.Evaluate(percent) * neutral_micro_expressions[5]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[6], (original_expressions[6] + exp_curve.Evaluate(percent) * neutral_micro_expressions[6]));
                                    moustaches.SetBlendShapeWeight(moustaches_index[8], (original_expressions[8] + exp_curve.Evaluate(percent) * neutral_micro_expressions[8]));
                                }
                            }

                            yield return null;
                        }

                        break;
                    case 2: // movimento labbra
                        CheekPuff_Left = (int)rnd.Next(10, 20);
                        CheekPuff_Right = (int)rnd.Next(10, 20);
                        LowerLipIn = (int)rnd.Next(70, 100); // OLD 50 100
                        Smile_Left = (int)rnd.Next(10, 40);
                        Smile_Right = Smile_Left;
                        UpperLipIn = LowerLipIn;

                        neutral_micro_expressions = new int[] { CheekPuff_Left, CheekPuff_Right, LowerLipIn, Smile_Left, Smile_Right, UpperLipIn };
                        face_index = new int[] { 10, 11, 28, 41, 42, 46 };
                        moustaches_index = new int[] { 5, 6, 23, 36, 37, 38 };
                        original_expressions = new float[7];

                        for (int k = 0; k < face_index.Length; k++)
                        {
                            original_expressions[k] = face.GetBlendShapeWeight(face_index[k]);
                        }

                        while (time <= duration && emotion_id == 0)
                        {
                            time = time + Time.deltaTime;
                            float percent = Mathf.Clamp01(time / duration);

                            for (int i = 0; i < face_index.Length; i++)
                            {
                                face.SetBlendShapeWeight(face_index[i], (original_expressions[i] + exp_curve.Evaluate(percent) * neutral_micro_expressions[i]));
                            }

                            if (moustaches != null)
                            {
                                for (int k = 0; k < moustaches_index.Length; k++)
                                {
                                    moustaches.SetBlendShapeWeight(moustaches_index[k], (original_expressions[k] + exp_curve.Evaluate(percent) * neutral_micro_expressions[k]));
                                }
                            }

                            yield return null;
                        }

                        break;
                    case 3: // movimento sopracciglia
                        BrowsDown_Left = (int)rnd.Next(10, 50);
                        BrowsDown_Right = BrowsDown_Left;
                        BrowsOuterLower_Left = (int)rnd.Next(10, 50);
                        BrowsOuterLower_Right = BrowsOuterLower_Left;

                        neutral_micro_expressions = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsOuterLower_Left, BrowsOuterLower_Right };
                        face_index = new int[] { 2, 3, 6, 7 };
                        original_expressions = new float[4];

                        for (int k = 0; k < face_index.Length; k++)
                        {
                            original_expressions[k] = face.GetBlendShapeWeight(face_index[k]);
                        }

                        while (time <= duration && emotion_id == 0)
                        {
                            time = time + Time.deltaTime;
                            float percent = Mathf.Clamp01(time / duration);

                            for (int i = 0; i < face_index.Length; i++)
                            {
                                face.SetBlendShapeWeight(face_index[i], (original_expressions[i] + exp_curve.Evaluate(percent) * neutral_micro_expressions[i]));
                            }

                            yield return null;
                        }

                        break;
                    default: // respiro standard
                        BrowsUp_Left = (int)rnd.Next(10, 30);
                        BrowsUp_Right = BrowsUp_Left;
                        MouthUp = (int)rnd.Next(20, 30);
                        NoseScrunch_Left = (int)rnd.Next(10, 50); // OLD 30 50
                        NoseScrunch_Right = NoseScrunch_Left;

                        neutral_micro_expressions = new int[] { BrowsUp_Left, BrowsUp_Right, MouthUp, NoseScrunch_Left, NoseScrunch_Right };
                        face_index = new int[] { 8, 9, 36, 39, 40 };
                        moustaches_index = new int[] { 3, 4, 31, 34, 35 };
                        original_expressions = new float[5];

                        for (int k = 0; k < face_index.Length; k++)
                        {
                            original_expressions[k] = face.GetBlendShapeWeight(face_index[k]);
                        }

                        while (time <= duration && emotion_id == 0)
                        {
                            time = time + Time.deltaTime;
                            float percent = Mathf.Clamp01(time / duration);

                            for (int i = 0; i < face_index.Length; i++)
                            {
                                face.SetBlendShapeWeight(face_index[i], (original_expressions[i] + exp_curve.Evaluate(percent) * neutral_micro_expressions[i]));
                            }

                            if (moustaches != null)
                            {
                                for (int k = 0; k < moustaches_index.Length; k++)
                                {
                                    moustaches.SetBlendShapeWeight(moustaches_index[k], (original_expressions[k] + exp_curve.Evaluate(percent) * neutral_micro_expressions[k]));
                                }
                            }

                            yield return null;
                        }

                        break;
                }
            }

            animation_lock = true;
        }

        //Debug.Log("Neutral_Animation - END");
    }

    IEnumerator Sadness(float duration)
    {
        StartCoroutine(tightenEyes());

        //float time = 0f;
        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 2;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        int[] face_joyUsurpriseUangerUfearUdisgust = joy_face.Union(surprise_face).Union(anger_face).Union(fear_face).Union(disgust_face).ToArray();
        int[] eyelashes_joyUsurpriseUangerUfearUdisgust = joy_eyelashes.Union(surprise_eyelashes).Union(anger_eyelashes).Union(fear_eyelashes).Union(disgust_eyelashes).ToArray();
        int[] moustaches_joyUsurpriseUangerUfearUdisgust = joy_moustaches.Union(surprise_moustaches).Union(anger_moustaches).Union(fear_moustaches).Union(disgust_moustaches).ToArray();

        Array.Sort(face_joyUsurpriseUangerUfearUdisgust);
        Array.Sort(eyelashes_joyUsurpriseUangerUfearUdisgust);
        Array.Sort(moustaches_joyUsurpriseUangerUfearUdisgust);

        IEnumerable<int> face_result = face_joyUsurpriseUangerUfearUdisgust.Except(sadness_face);
        IEnumerable<int> eyelashes_result = eyelashes_joyUsurpriseUangerUfearUdisgust.Except(sadness_eyelashes);
        IEnumerable<int> moustaches_result = moustaches_joyUsurpriseUangerUfearUdisgust.Except(sadness_moustaches);

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 4.0f); // OLD 0.5 3

        if (!cry)
        {
            cry = true;
            StartCoroutine(Crying(time_to_cry)); // Set Crying ON
        }

        while (emotion_id == 1)
        {
            if (animation_lock)
            {
                animation_lock = false;
                float time = 0f;

                float animation_frequency = UnityEngine.Random.Range(0.0f, 1.0f);

                yield return new WaitForSeconds(animation_frequency);

                int Blink_Left = (int)rnd.Next(20, 30);
                int Blink_Right = Blink_Left;
                int BrowsIn_Left = (int)rnd.Next(50, 80);
                int BrowsIn_Right = (int)rnd.Next(50, 80);
                int BrowsOuterLower_Left = (int)rnd.Next(80, 100);
                int BrowsOuterLower_Right = (int)rnd.Next(80, 100);
                int CheekPuff_Left = (int)rnd.Next(0, 20);
                int CheekPuff_Right = (int)rnd.Next(0, 20);
                int Frown_Left = (int)rnd.Next(50, 100);
                int Frown_Right = (int)rnd.Next(50, 100);
                int JawForeward = (int)rnd.Next(0, 50);
                int LowerLipDown_Left = (int)rnd.Next(0, 15);
                int LowerLipDown_Right = (int)rnd.Next(0, 15);
                int LowerLipIn = (int)rnd.Next(0, 100);
                int MouthDown = (int)rnd.Next(0, 30);

                int[] face_sadness = new int[] { Blink_Left, Blink_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right,
                                         CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, JawForeward, LowerLipDown_Left,
                                         LowerLipDown_Right, LowerLipIn, MouthDown };

                int[] eyelashes_sadness = new int[] { Blink_Left, Blink_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right };

                int[] moustaches_sadness = new int[] { CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, JawForeward, LowerLipDown_Left,
                                               LowerLipDown_Right, LowerLipIn, MouthDown };

                while (time <= duration && emotion_id == 1)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    // Reset unused facial muscles
                    foreach (var n in face_result)
                    {
                        face.SetBlendShapeWeight(n, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(n)));
                    }

                    // Reset unused eyelashes muscles
                    foreach (var m in eyelashes_result)
                    {
                        eyelashes.SetBlendShapeWeight(m, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(m)));
                    }

                    // Reset unused moustaches 
                    if (moustaches != null)
                    {
                        foreach (var k in moustaches_result)
                        {
                            moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                        }
                    }

                    // Set sadness facial muscles
                    for (int k = 0; k < face_sadness.Length; k++)
                    {
                        if (face_sadness[k] > face.GetBlendShapeWeight(sadness_face[k]))
                        {
                            int difference = face_sadness[k] - (int)face.GetBlendShapeWeight(sadness_face[k]);
                            face.SetBlendShapeWeight(sadness_face[k], (face.GetBlendShapeWeight(sadness_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(sadness_face[k]) - face_sadness[k];
                            face.SetBlendShapeWeight(sadness_face[k], (face.GetBlendShapeWeight(sadness_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set sadness eyelashes muscles
                    for (int j = 0; j < eyelashes_sadness.Length; j++)
                    {
                        if (eyelashes_sadness[j] > eyelashes.GetBlendShapeWeight(sadness_eyelashes[j]))
                        {
                            int difference = eyelashes_sadness[j] - (int)eyelashes.GetBlendShapeWeight(sadness_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(sadness_eyelashes[j], (eyelashes.GetBlendShapeWeight(sadness_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(sadness_eyelashes[j]) - eyelashes_sadness[j];
                            eyelashes.SetBlendShapeWeight(sadness_eyelashes[j], (eyelashes.GetBlendShapeWeight(sadness_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set sadness moustaches
                    if (moustaches != null)
                    {
                        for (int j = 0; j < moustaches_sadness.Length; j++)
                        {
                            if (moustaches_sadness[j] > moustaches.GetBlendShapeWeight(sadness_moustaches[j]))
                            {
                                int difference = moustaches_sadness[j] - (int)moustaches.GetBlendShapeWeight(sadness_moustaches[j]);
                                moustaches.SetBlendShapeWeight(sadness_moustaches[j], (moustaches.GetBlendShapeWeight(sadness_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(sadness_moustaches[j]) - moustaches_sadness[j];
                                moustaches.SetBlendShapeWeight(sadness_moustaches[j], (moustaches.GetBlendShapeWeight(sadness_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }

                    yield return null;
                }
            }

            duration = 2.0f;
            animation_lock = true;
        }
        //StartCoroutine(Sadness_Animation());
    }

    // OLD IDEA
    /*IEnumerator Sadness_Animation()
    {
        System.Random rnd = new System.Random();
        float duration = 2.0f; // OLD 0.5f

        //Debug.Log("Sadness_Animation - START");

        while (emotion_id == 1)
        {
            if (animation_lock)
            {
                animation_lock = false;
                //Debug.Log("Sadness_Animation - MODIFY");

                float animation_frequency = UnityEngine.Random.Range(0.0f, 1.0f); // OLD 1.0 4.0

                yield return new WaitForSeconds(animation_frequency);

                float time = 0f;

                int BrowsUp_Left = (int)rnd.Next(0, 25); // OLD 0 10
                int BrowsUp_Right = BrowsUp_Left; // OLD 0 10
                int JawForeward = (int)rnd.Next(20, 25); // NEW
                int LowerLipDown_Left = (int)rnd.Next(0, 10); // NEW
                int LowerLipDown_Right = (int)rnd.Next(0, 10); // NEW
                //int LowerLipOut = 40;
                int Midmouth_Left = 5;
                int Midmouth_Right = 5;
                int MouthUp = (int)rnd.Next(20, 30); // OLD 10 20
                int NoseScrunch_Left = (int)rnd.Next(0, 40); // OLD 0 20
                int NoseScrunch_Right = NoseScrunch_Left;
                //int UpperLipIn = (int)rnd.Next(30, 40); // OLD 20 50

                int[] sadness_micro_expressions = new int[] { BrowsUp_Left, BrowsUp_Right, JawForeward, LowerLipDown_Left, LowerLipDown_Right,
                                                          Midmouth_Left, Midmouth_Right, MouthUp, NoseScrunch_Left, NoseScrunch_Right };

                int[] face_index = new int[] { 8, 9, 17, 26, 27, 30, 31, 36, 39, 40 }; // OLD 8, 9, 29, 30, 31, 36, 39, 40, 46

                int[] moustaches_index = new int[] { 3, 4, 12, 21, 22, 25, 26, 31, 34, 35 };

                float[] original_expressions = new float[10]; // OLD 8

                for (int k = 0; k < face_index.Length; k++)
                {
                    original_expressions[k] = face.GetBlendShapeWeight(face_index[k]);
                }

                while (time <= duration && emotion_id == 1)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    for (int i = 0; i < face_index.Length; i++)
                    {
                        face.SetBlendShapeWeight(face_index[i], (original_expressions[i] + exp_curve.Evaluate(percent) * sadness_micro_expressions[i]));
                    }

                    if (moustaches != null)
                    {
                        for (int k = 0; k < moustaches_index.Length; k++)
                        {
                            moustaches.SetBlendShapeWeight(moustaches_index[k], (original_expressions[k] + exp_curve.Evaluate(percent) * sadness_micro_expressions[k]));
                        }
                    }

                    yield return null;
                }
            }

            animation_lock = true;
        }
        
        //Debug.Log("Sadness_Animation - END");
    }*/

    IEnumerator Crying(float duration)
    {
        if (cry)
        {
            float alpha = 1f;
            float time = 0f;

            while (time <= duration && cry)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                tears[0].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, curve.Evaluate(percent) * alpha);
                tears[1].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, curve.Evaluate(percent) * alpha);

                yield return null;
            }
        }
        else
        {
            float time = 0f;

            while (time <= duration && !cry)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                tears[0].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, end_curve.Evaluate(percent) * tears[0].GetComponent<MeshRenderer>().material.color.a);
                tears[1].GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, end_curve.Evaluate(percent) * tears[0].GetComponent<MeshRenderer>().material.color.a);

                yield return null;
            }
        }
    }

    IEnumerator Joy(float duration)
    {
        StartCoroutine(tightenEyes());

        //float time = 0f;
        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 3.5f;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        int[] face_sadnessUsurpriseUangerUfearUdisgust = sadness_face.Union(surprise_face).Union(anger_face).Union(fear_face).Union(disgust_face).ToArray();
        int[] eyelashes_sadnessUsurpriseUangerUfearUdisgust = sadness_eyelashes.Union(surprise_eyelashes).Union(anger_eyelashes).Union(fear_eyelashes).Union(disgust_eyelashes).ToArray();
        int[] moustaches_sadnessUsurpriseUangerUfearUdisgust = sadness_moustaches.Union(surprise_moustaches).Union(anger_moustaches).Union(fear_moustaches).Union(disgust_moustaches).ToArray();

        Array.Sort(face_sadnessUsurpriseUangerUfearUdisgust);
        Array.Sort(eyelashes_sadnessUsurpriseUangerUfearUdisgust);
        Array.Sort(moustaches_sadnessUsurpriseUangerUfearUdisgust);

        IEnumerable<int> face_result = face_sadnessUsurpriseUangerUfearUdisgust.Except(joy_face);
        IEnumerable<int> eyelashes_result = eyelashes_sadnessUsurpriseUangerUfearUdisgust.Except(joy_eyelashes);
        IEnumerable<int> moustaches_result = moustaches_sadnessUsurpriseUangerUfearUdisgust.Except(joy_moustaches);

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        while (emotion_id == 2)
        {
            if (animation_lock)
            {
                animation_lock = false;
                float time = 0f;

                float animation_frequency = UnityEngine.Random.Range(0.0f, 1.0f);

                yield return new WaitForSeconds(animation_frequency);

                int BrowsOuterLower_Left = (int)rnd.Next(70, 100);
                int BrowsOuterLower_Right = BrowsOuterLower_Left; // OLD 70 100
                int CheekPuff_Left = (int)rnd.Next(0, 5); // OLD 0 20
                int CheekPuff_Right = (int)rnd.Next(0, 5); // OLD 0 20
                int EyesWide_Left = (int)rnd.Next(30, 50); // NEW
                int EyesWide_Right = EyesWide_Left; // NEW
                int LowerLipDown_Left = (int)rnd.Next(0, 30); // NEW
                int LowerLipDown_Right = LowerLipDown_Left; // NEW
                int Midmouth_Left = (int)rnd.Next(0, 20); // NEW
                int Midmouth_Right = (int)rnd.Next(0, 20); // NEW
                int MouthNarrow_Left = (int)rnd.Next(0, 30); // NEW
                int MouthNarrow_Right = MouthNarrow_Left; // NEW
                int Mouth_Open = (int)rnd.Next(0, 10); // OLD 0-25
                int MouthWhistle_NarrowAdjust_Left = (int)rnd.Next(0, 20); // NEW
                int MouthWhistle_NarrowAdjust_Right = MouthWhistle_NarrowAdjust_Left; // NEW
                int NoseScrunch_Left = (int)rnd.Next(0, 40);
                int NoseScrunch_Right = NoseScrunch_Left; // OLD 0-40
                int Smile_Left = (int)rnd.Next(70, 120); // OLD 50-100
                int Smile_Right = (int)rnd.Next(70, 120); // OLD 50-100
                int Squint_Left = (int)rnd.Next(50, 70); // OLD 70-80
                int Squint_Right = Squint_Left;
                int UpperLipIn = 0; // Only for Policeman
                int UpperLipUp_Left = (int)rnd.Next(0, 40); // OLD 30
                int UpperLipUp_Right = (int)rnd.Next(0, 40); // OLD 30

                // NEW TEMPORAL CODE
                if (name == "Policeman")
                {
                    CheekPuff_Left = (int)rnd.Next(-30, -20);
                    CheekPuff_Right = (int)rnd.Next(-30, -20);
                    Smile_Left = (int)rnd.Next(70, 100);
                    Smile_Right = (int)rnd.Next(70, 100);
                    UpperLipIn = (int)rnd.Next(50, 80);
                }
                //

                int[] face_joy = new int[] { BrowsOuterLower_Left, BrowsOuterLower_Right, CheekPuff_Left, CheekPuff_Right, EyesWide_Left, EyesWide_Right, LowerLipDown_Left,
                                             LowerLipDown_Right, Midmouth_Left, Midmouth_Right, MouthNarrow_Left, MouthNarrow_Right, Mouth_Open, MouthWhistle_NarrowAdjust_Left,
                                             MouthWhistle_NarrowAdjust_Right, NoseScrunch_Left, NoseScrunch_Right, Smile_Left, Smile_Right, Squint_Left, Squint_Right,
                                             UpperLipIn, UpperLipUp_Left, UpperLipUp_Right};

                int[] eyelashes_joy = new int[] { BrowsOuterLower_Left, BrowsOuterLower_Right, EyesWide_Left, EyesWide_Right, Squint_Left, Squint_Right };

                int[] moustaches_joy = new int[] { CheekPuff_Left, CheekPuff_Right, EyesWide_Left, EyesWide_Right, LowerLipDown_Left, LowerLipDown_Right, Midmouth_Left,
                                                   Midmouth_Right, MouthNarrow_Left, MouthNarrow_Right, Mouth_Open, MouthWhistle_NarrowAdjust_Left, MouthWhistle_NarrowAdjust_Right,
                                                   NoseScrunch_Left, NoseScrunch_Right, Smile_Left, Smile_Right, UpperLipIn, UpperLipUp_Left, UpperLipUp_Right };

                while (time <= duration && emotion_id == 2)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    // Reset unused facial muscles
                    foreach (var n in face_result)
                    {
                        face.SetBlendShapeWeight(n, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(n)));
                    }

                    // Reset unused eyelashes muscles
                    foreach (var m in eyelashes_result)
                    {
                        eyelashes.SetBlendShapeWeight(m, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(m)));
                    }

                    // Reset unused moustaches 
                    if (moustaches != null)
                    {
                        foreach (var k in moustaches_result)
                        {
                            moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                        }
                    }

                    // Set joy facial muscles
                    for (int k = 0; k < face_joy.Length; k++)
                    {
                        if (face_joy[k] > face.GetBlendShapeWeight(joy_face[k]))
                        {
                            int difference = face_joy[k] - (int)face.GetBlendShapeWeight(joy_face[k]);
                            face.SetBlendShapeWeight(joy_face[k], (face.GetBlendShapeWeight(joy_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(joy_face[k]) - face_joy[k];
                            face.SetBlendShapeWeight(joy_face[k], (face.GetBlendShapeWeight(joy_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set joy eyelashes muscles
                    for (int j = 0; j < eyelashes_joy.Length; j++)
                    {
                        if (eyelashes_joy[j] > eyelashes.GetBlendShapeWeight(joy_eyelashes[j]))
                        {
                            int difference = eyelashes_joy[j] - (int)eyelashes.GetBlendShapeWeight(joy_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(joy_eyelashes[j], (eyelashes.GetBlendShapeWeight(joy_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(joy_eyelashes[j]) - eyelashes_joy[j];
                            eyelashes.SetBlendShapeWeight(joy_eyelashes[j], (eyelashes.GetBlendShapeWeight(joy_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set joy moustaches
                    if (moustaches != null)
                    {
                        for (int j = 0; j < moustaches_joy.Length; j++)
                        {
                            if (moustaches_joy[j] > moustaches.GetBlendShapeWeight(joy_moustaches[j]))
                            {
                                int difference = moustaches_joy[j] - (int)moustaches.GetBlendShapeWeight(joy_moustaches[j]);
                                moustaches.SetBlendShapeWeight(joy_moustaches[j], (moustaches.GetBlendShapeWeight(joy_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(joy_moustaches[j]) - moustaches_joy[j];
                                moustaches.SetBlendShapeWeight(joy_moustaches[j], (moustaches.GetBlendShapeWeight(joy_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }

                    yield return null;
                }
            }

            duration = 2.0f;
            animation_lock = true;
        }
    }

    IEnumerator Surprise(float duration)
    {
        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 6;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        int[] face_sadnessUjoyUangerUfearUdisgust = sadness_face.Union(joy_face).Union(anger_face).Union(fear_face).Union(disgust_face).ToArray();
        int[] eyelashes_sadnessUjoyUangerUfearUdisgust = sadness_eyelashes.Union(joy_eyelashes).Union(anger_eyelashes).Union(fear_eyelashes).Union(disgust_eyelashes).ToArray();
        int[] moustaches_sadnessUjoyUangerUfearUdisgust = sadness_moustaches.Union(joy_moustaches).Union(anger_moustaches).Union(fear_moustaches).Union(disgust_moustaches).ToArray();

        Array.Sort(face_sadnessUjoyUangerUfearUdisgust);
        Array.Sort(eyelashes_sadnessUjoyUangerUfearUdisgust);
        Array.Sort(moustaches_sadnessUjoyUangerUfearUdisgust);

        IEnumerable<int> face_result = face_sadnessUjoyUangerUfearUdisgust.Except(surprise_face);
        IEnumerable<int> eyelashes_result = eyelashes_sadnessUjoyUangerUfearUdisgust.Except(surprise_eyelashes);
        IEnumerable<int> moustaches_result = moustaches_sadnessUjoyUangerUfearUdisgust.Except(surprise_moustaches);

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        bool first_time = true;

        while (emotion_id == 3)
        {
            if (animation_lock)
            {
                animation_lock = false;
                float time = 0f;
                float animation_frequency;

                if (first_time)
                {
                    animation_frequency = UnityEngine.Random.Range(0.0f, 1.0f);
                    duration = 0.5f;
                    first_time = false;
                }
                else
                {
                    animation_frequency = UnityEngine.Random.Range(1.5f, 2.5f); // OLD 0.0f 1.0f
                    animation_interpolation = true; // NEW
                    duration = 2.0f; // OLD 5.0f
                    StartCoroutine(SurpriseInterpolation(animation_frequency / 2)); // NEW
                }

                yield return new WaitForSeconds(animation_frequency);

                animation_interpolation = false; // NEW

                int BrowsDown_Left = (int)rnd.Next(0, 50); // NEW
                int BrowsDown_Right = BrowsDown_Left; // NEW
                int BrowsIn_Left = (int)rnd.Next(0, 30); // NEW
                int BrowsIn_Right = BrowsIn_Left; // NEW
                int BrowsOuterLower_Left = (int)rnd.Next(40, 80);
                int BrowsOuterLower_Right = BrowsOuterLower_Left; // OLD 40-80
                int BrowsUp_Left = (int)rnd.Next(40, 60);
                int BrowsUp_Right = BrowsUp_Left; // OLD 40-60
                int CheekPuff_Left = (int)rnd.Next(0, 10); // OLD 0-20
                int CheekPuff_Right = CheekPuff_Left; // OLD 0-20
                int EyesWide_Left = (int)rnd.Next(120, 150);
                int EyesWide_Right = EyesWide_Left;
                int Frown_Left = (int)rnd.Next(0, 50); // OLD 30-60
                int Frown_Right = Frown_Left; // OLD 0-50
                int Jaw_Down = (int)rnd.Next(20, 80); // OLD 50-80
                int LowerLipDown_Left = (int)rnd.Next(0, 20); // NEW
                int LowerLipDown_Right = (int)rnd.Next(0, 20); // NEW
                int LowerLipIn = (int)rnd.Next(0, 100); // NEW
                int LowerLipOut = (int)rnd.Next(40, 60);
                int MidMouth_Left = (int)rnd.Next(0, 10); // NEW
                int MidMouth_Right = MidMouth_Left; // OLD 0-10
                int MouthNarrow_Left = (int)rnd.Next(20, 40); // OLD 20-40
                int MouthNarrow_Right = MouthNarrow_Left; // OLD 20-40
                int Mouth_Open = (int)rnd.Next(20, 40); // OLD 10 40
                int NoseScrunch_Left = (int)rnd.Next(0, 20); // NEW
                int NoseScrunch_Right = NoseScrunch_Left; // NEW
                int UpperLipUp_Left = (int)rnd.Next(0, 30); // NEW
                int UpperLipUp_Right = (int)rnd.Next(0, 30); // NEW

                // Fix for Speaking
                if (speak)
                {
                    Mouth_Open = (int)rnd.Next(0, 5);
                }

                int[] face_surprise = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right, BrowsUp_Left, BrowsUp_Right,
                                                  CheekPuff_Left, CheekPuff_Right, EyesWide_Left, EyesWide_Right, Frown_Left, Frown_Right, Jaw_Down, LowerLipDown_Left, LowerLipDown_Right,
                                                  LowerLipIn, LowerLipOut, MidMouth_Left, MidMouth_Right, MouthNarrow_Left, MouthNarrow_Right, Mouth_Open, NoseScrunch_Left,
                                                  NoseScrunch_Right, UpperLipUp_Left, UpperLipUp_Right};

                int[] eyelashes_surprise = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right, BrowsUp_Left,
                                                       BrowsUp_Right, EyesWide_Left, EyesWide_Right };

                int[] moustaches_surprise = new int[] { CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, Jaw_Down, LowerLipDown_Left, LowerLipDown_Right, LowerLipIn, LowerLipOut,
                                                        MidMouth_Left, MidMouth_Right, MouthNarrow_Left, MouthNarrow_Right, Mouth_Open, NoseScrunch_Left, NoseScrunch_Right,
                                                        UpperLipUp_Left, UpperLipUp_Right};

                StartCoroutine(widensEyes());

                while (time <= (duration / 2) && emotion_id == 3) // OLD duration
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    //Debug.Log("Basic Time: " + time + " Duration: " + duration + " Percent: " + percent);

                    // Reset unused facial muscles
                    foreach (var n in face_result)
                    {
                        face.SetBlendShapeWeight(n, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(n)));
                    }

                    // Reset unused eyelashes muscles
                    foreach (var m in eyelashes_result)
                    {
                        eyelashes.SetBlendShapeWeight(m, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(m)));
                    }

                    // Reset unused moustaches 
                    if (moustaches != null)
                    {
                        foreach (var k in moustaches_result)
                        {
                            moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                        }
                    }

                    // Fix for Speaking
                    if (speak)
                    {
                        face_surprise[23] = (int)rnd.Next(0, 5);
                        moustaches_surprise[13] = face_surprise[23];
                    }

                    // Set surprise facial muscles
                    for (int k = 0; k < face_surprise.Length; k++)
                    {
                        if (face_surprise[k] > face.GetBlendShapeWeight(surprise_face[k]))
                        {
                            int difference = face_surprise[k] - (int)face.GetBlendShapeWeight(surprise_face[k]);
                            face.SetBlendShapeWeight(surprise_face[k], (face.GetBlendShapeWeight(surprise_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(surprise_face[k]) - face_surprise[k];
                            face.SetBlendShapeWeight(surprise_face[k], (face.GetBlendShapeWeight(surprise_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set surprise eyelashes muscles
                    for (int j = 0; j < eyelashes_surprise.Length; j++)
                    {
                        if (eyelashes_surprise[j] > eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]))
                        {
                            int difference = eyelashes_surprise[j] - (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(surprise_eyelashes[j], (eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) - eyelashes_surprise[j];
                            eyelashes.SetBlendShapeWeight(surprise_eyelashes[j], (eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set surprise moustaches
                    if (moustaches != null)
                    {
                        for (int j = 0; j < moustaches_surprise.Length; j++)
                        {
                            if (moustaches_surprise[j] > moustaches.GetBlendShapeWeight(surprise_moustaches[j]))
                            {
                                int difference = moustaches_surprise[j] - (int)moustaches.GetBlendShapeWeight(surprise_moustaches[j]);
                                moustaches.SetBlendShapeWeight(surprise_moustaches[j], (moustaches.GetBlendShapeWeight(surprise_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(surprise_moustaches[j]) - moustaches_surprise[j];
                                moustaches.SetBlendShapeWeight(surprise_moustaches[j], (moustaches.GetBlendShapeWeight(surprise_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }

                    yield return null;
                }
            }

            //duration = 2.0f; // OLD 5.0f
            animation_lock = true;
        }
    }

    IEnumerator widensEyes()
    {
        float time = 0f;
        float duration = 0.25f; // OLD 0.5

        if (left_eye.transform.localScale.x <= 1 || left_eye.transform.localScale.y <= 1 || left_eye.transform.localScale.z <= 1 ||
            right_eye.transform.localScale.x <= 1 || right_eye.transform.localScale.y <= 1 || right_eye.transform.localScale.z <= 1)
        {
            while (time <= duration)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                left_eye.transform.localScale = new Vector3(1 + curve.Evaluate(percent) * 0.05f, 1 + curve.Evaluate(percent) * 0.1f, 1 + curve.Evaluate(percent) * 0.1f);
                right_eye.transform.localScale = new Vector3(1 + curve.Evaluate(percent) * 0.05f, 1 + curve.Evaluate(percent) * 0.1f, 1 + curve.Evaluate(percent) * 0.1f);

                yield return null;
            }
        }
    }

    IEnumerator tightenEyes()
    {
        float time = 0f;
        float duration = 0.5f; // OLD 0.25

        if (left_eye.transform.localScale.x > 1 || left_eye.transform.localScale.y > 1 || left_eye.transform.localScale.z > 1 ||
            right_eye.transform.localScale.x > 1 || right_eye.transform.localScale.y > 1 || right_eye.transform.localScale.z > 1)
        {
            while (time <= duration)
            {
                time = time + Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                left_eye.transform.localScale = new Vector3(1.05f - curve.Evaluate(percent) * 0.05f, 1.1f - curve.Evaluate(percent) * 0.1f, 1.1f - curve.Evaluate(percent) * 0.1f);
                right_eye.transform.localScale = new Vector3(1.05f - curve.Evaluate(percent) * 0.05f, 1.1f - curve.Evaluate(percent) * 0.1f, 1.1f - curve.Evaluate(percent) * 0.1f);

                yield return null;
            }
        }
    }

    IEnumerator SurpriseInterpolation(float duration)
    {
        System.Random rnd = new System.Random();

        while (emotion_id == 3 && animation_interpolation)
        {
            float time = 0f;
            int mod = (int)rnd.Next(-2, 2); // OLD -2 2
            int[] face_surprise = new int[surprise_face.Length];
            int[] eyelashes_surprise = new int[surprise_eyelashes.Length];
            int[] moustaches_surprise = new int[surprise_moustaches.Length];

            for (int k = 0; k < face_surprise.Length; k++)
            {
                if (mod < 0 && face.GetBlendShapeWeight(surprise_face[k]) >= (0 - mod))
                {
                    face_surprise[k] = (int)face.GetBlendShapeWeight(surprise_face[k]) + mod;
                }
                else if (mod > 0 && face.GetBlendShapeWeight(surprise_face[k]) <= (100 - mod))
                {
                    face_surprise[k] = (int)face.GetBlendShapeWeight(surprise_face[k]) + mod;
                }
                else
                {
                    face_surprise[k] = (int)face.GetBlendShapeWeight(surprise_face[k]);
                }
            }

            for (int k = 0; k < eyelashes_surprise.Length; k++)
            {
                if (mod < 0 && eyelashes.GetBlendShapeWeight(surprise_eyelashes[k]) >= (0 - mod))
                {
                    eyelashes_surprise[k] = (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[k]) + mod;
                }
                else if (mod > 0 && eyelashes.GetBlendShapeWeight(surprise_eyelashes[k]) <= (100 - mod))
                {
                    eyelashes_surprise[k] = (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[k]) + mod;
                }
                else
                {
                    eyelashes_surprise[k] = (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[k]);
                }
            }

            if (moustaches != null)
            {
                for (int k = 0; k < moustaches_surprise.Length; k++)
                {
                    if (mod < 0 && moustaches.GetBlendShapeWeight(surprise_moustaches[k]) >= (0 - mod))
                    {
                        moustaches_surprise[k] = (int)moustaches.GetBlendShapeWeight(surprise_moustaches[k]) + mod;
                    }
                    else if (mod > 0 && moustaches.GetBlendShapeWeight(surprise_moustaches[k]) <= (100 - mod))
                    {
                        moustaches_surprise[k] = (int)moustaches.GetBlendShapeWeight(surprise_moustaches[k]) + mod;
                    }
                    else
                    {
                        moustaches_surprise[k] = (int)moustaches.GetBlendShapeWeight(surprise_moustaches[k]);
                    }
                }
            }

            while (time <= (duration / 2) && emotion_id == 3 && animation_interpolation) // OLD duration
            {
                time += Time.deltaTime;
                float percent = Mathf.Clamp01(time / duration);

                //Debug.Log("Interpolation Time: " + time + " Duration: " + duration);

                // Fix for Speaking
                if (speak)
                {
                    face_surprise[23] = (int)rnd.Next(0, 5);
                    moustaches_surprise[13] = face_surprise[23];
                }

                // Set mod of facial muscles
                for (int k = 0; k < surprise_face.Length; k++)
                {
                    if (!animation_interpolation)
                    {
                        break;
                    }
                    else {
                        if (face_surprise[k] > face.GetBlendShapeWeight(surprise_face[k]))
                        {
                            int difference = face_surprise[k] - (int)face.GetBlendShapeWeight(surprise_face[k]);
                            face.SetBlendShapeWeight(surprise_face[k], (face.GetBlendShapeWeight(surprise_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(surprise_face[k]) - face_surprise[k];
                            face.SetBlendShapeWeight(surprise_face[k], (face.GetBlendShapeWeight(surprise_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }
                }

                // Set mod of eyelashes muscles
                for (int j = 0; j < surprise_eyelashes.Length; j++)
                {
                    if (!animation_interpolation)
                    {
                        break;
                    }
                    else
                    {
                        if (eyelashes_surprise[j] > eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]))
                        {
                            int difference = eyelashes_surprise[j] - (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(surprise_eyelashes[j], (eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) - eyelashes_surprise[j];
                            eyelashes.SetBlendShapeWeight(surprise_eyelashes[j], (eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }
                }

                // Set mod of moustaches
                if (moustaches != null)
                {
                    for (int j = 0; j < surprise_moustaches.Length; j++)
                    {
                        if (!animation_interpolation)
                        {
                            break;
                        }
                        else
                        {
                            if (moustaches_surprise[j] > moustaches.GetBlendShapeWeight(surprise_moustaches[j]))
                            {
                                int difference = moustaches_surprise[j] - (int)moustaches.GetBlendShapeWeight(surprise_moustaches[j]);
                                moustaches.SetBlendShapeWeight(surprise_moustaches[j], (moustaches.GetBlendShapeWeight(surprise_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(surprise_moustaches[j]) - moustaches_surprise[j];
                                moustaches.SetBlendShapeWeight(surprise_moustaches[j], (moustaches.GetBlendShapeWeight(surprise_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }
                }

                yield return null;
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator Anger(float duration)
    {
        StartCoroutine(tightenEyes());

        //float time = 0f;
        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 5;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        int[] face_sadnessUjoyUsurpriseUfearUdisgust = sadness_face.Union(joy_face).Union(surprise_face).Union(fear_face).Union(disgust_face).ToArray();
        int[] eyelashes_sadnessUjoyUsurpriseUfearUdisgust = sadness_eyelashes.Union(joy_eyelashes).Union(surprise_eyelashes).Union(fear_eyelashes).Union(disgust_eyelashes).ToArray();
        int[] moustaches_sadnessUjoyUsurpriseUfearUdisgust = sadness_moustaches.Union(joy_moustaches).Union(surprise_moustaches).Union(fear_moustaches).Union(disgust_moustaches).ToArray();

        Array.Sort(face_sadnessUjoyUsurpriseUfearUdisgust);
        Array.Sort(eyelashes_sadnessUjoyUsurpriseUfearUdisgust);
        Array.Sort(moustaches_sadnessUjoyUsurpriseUfearUdisgust);

        IEnumerable<int> face_result = face_sadnessUjoyUsurpriseUfearUdisgust.Except(anger_face);
        IEnumerable<int> eyelashes_result = eyelashes_sadnessUjoyUsurpriseUfearUdisgust.Except(anger_eyelashes);
        IEnumerable<int> moustaches_result = moustaches_sadnessUjoyUsurpriseUfearUdisgust.Except(anger_moustaches);

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        // NEW TEMPORAL CODE
        if(name == "Policeman")
        {
            GetComponent<Animator>().SetTrigger("HandAnger");
        }
        //

        while (emotion_id == 4)
        {
            if (animation_lock)
            {
                animation_lock = false;
                float time = 0f;

                float animation_frequency = UnityEngine.Random.Range(0.0f, 1.0f);

                yield return new WaitForSeconds(animation_frequency);

                int BrowsDown_Left = (int)rnd.Next(80, 120); // OLD 80 140
                int BrowsDown_Right = (int)rnd.Next(80, 120); // OLD 80 140
                int BrowsOuterLower_Left = (int)rnd.Next(-50, 0); // OLD 0 50
                int BrowsOuterLower_Right = (int)rnd.Next(-50, 0); // OLD 0 50
                int CheekPuff_Left = (int)rnd.Next(0, 10); // OLD 20 30
                int CheekPuff_Right = CheekPuff_Left; // OLD 20 30
                int EyesWide_Left = (int)rnd.Next(0, 80); // OLD 50 100
                int EyesWide_Right = EyesWide_Left; // OLD 50 100
                int Frown_Left = (int)rnd.Next(20, 40); // OLD 20 35
                int Frown_Right = (int)rnd.Next(20, 40); // OLD 20 35
                int Jaw_Up = (int)rnd.Next(0, 50); // OLD 20 50
                int LowerLipIn = (int)rnd.Next(70, 120); // OLD 70 100
                int MouthOpen = (int)rnd.Next(0, 1); // NEW => serve per Speaking
                int MouthUp = (int)rnd.Next(80, 120); // OLD 80 100
                int NoseScrunch_Left = (int)rnd.Next(70, 110); // OLD 80 100
                int NoseScrunch_Right = (int)rnd.Next(70, 110); // OLD 80 100
                int Squint_Left = (int)rnd.Next(0, 30); // OLD 0 100
                int Squint_Right = Squint_Left; // OLD 0 100
                int UpperLipIn = (int)rnd.Next(80, 100); // OLD 40 50

                // NEW TEMPORAL CODE
                if (name == "Policeman")
                {
                    CheekPuff_Left = (int)rnd.Next(-30, -20);
                    CheekPuff_Right = CheekPuff_Left;
                    UpperLipIn = (int)rnd.Next(40, 60);
                }
                //

                // Fix for Speaking
                if (speak)
                {
                    LowerLipIn = (int)rnd.Next(10, 20);
                    MouthOpen = (int)rnd.Next(10, 20);
                    MouthUp = (int)rnd.Next(20, 30);
                    NoseScrunch_Left = (int)rnd.Next(50, 60);
                    NoseScrunch_Right = (int)rnd.Next(50, 60);
                    UpperLipIn = (int)rnd.Next(20, 30);
                }

                int[] face_anger = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsOuterLower_Left, BrowsOuterLower_Right, CheekPuff_Left, CheekPuff_Right, EyesWide_Left, EyesWide_Right, Frown_Left,
                                               Frown_Right, Jaw_Up, LowerLipIn, MouthOpen, MouthUp, NoseScrunch_Left, NoseScrunch_Right, Squint_Left, Squint_Right, UpperLipIn};

                int[] eyelashes_anger = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsOuterLower_Left, BrowsOuterLower_Right, EyesWide_Left, EyesWide_Right, Squint_Left, Squint_Right };

                int[] moustaches_anger = new int[] { CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, Jaw_Up, LowerLipIn, MouthOpen, MouthUp, NoseScrunch_Left,
                                                     NoseScrunch_Right, UpperLipIn };

                while (time <= duration && emotion_id == 4)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    // Reset unused facial muscles
                    foreach (var n in face_result)
                    {
                        face.SetBlendShapeWeight(n, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(n)));
                    }

                    // Reset unused eyelashes muscles
                    foreach (var m in eyelashes_result)
                    {
                        eyelashes.SetBlendShapeWeight(m, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(m)));
                    }

                    // Reset unused moustaches 
                    if (moustaches != null)
                    {
                        foreach (var k in moustaches_result)
                        {
                            moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                        }
                    }

                    // Fix for Speaking
                    if (speak)
                    {
                        face_anger[11] = (int)rnd.Next(10, 20);
                        face_anger[12] = (int)rnd.Next(10, 20);
                        face_anger[13] = (int)rnd.Next(20, 30);
                        face_anger[14] = (int)rnd.Next(50, 60);
                        face_anger[15] = (int)rnd.Next(50, 60);
                        face_anger[18] = (int)rnd.Next(20, 30);
                        moustaches_anger[5] = face_anger[11];
                        moustaches_anger[6] = face_anger[12];
                        moustaches_anger[7] = face_anger[13];
                        moustaches_anger[8] = face_anger[14];
                        moustaches_anger[9] = face_anger[15];
                        moustaches_anger[10] = face_anger[18];
                    }

                    // Set anger facial muscles
                    for (int k = 0; k < face_anger.Length; k++)
                    {
                        if (face_anger[k] > face.GetBlendShapeWeight(anger_face[k]))
                        {
                            int difference = face_anger[k] - (int)face.GetBlendShapeWeight(anger_face[k]);
                            face.SetBlendShapeWeight(anger_face[k], (face.GetBlendShapeWeight(anger_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(anger_face[k]) - face_anger[k];
                            face.SetBlendShapeWeight(anger_face[k], (face.GetBlendShapeWeight(anger_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set anger eyelashes muscles
                    for (int j = 0; j < eyelashes_anger.Length; j++)
                    {
                        if (eyelashes_anger[j] > eyelashes.GetBlendShapeWeight(anger_eyelashes[j]))
                        {
                            int difference = eyelashes_anger[j] - (int)eyelashes.GetBlendShapeWeight(anger_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(anger_eyelashes[j], (eyelashes.GetBlendShapeWeight(anger_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(anger_eyelashes[j]) - eyelashes_anger[j];
                            eyelashes.SetBlendShapeWeight(anger_eyelashes[j], (eyelashes.GetBlendShapeWeight(anger_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set anger moustaches
                    if (moustaches != null)
                    {
                        for (int j = 0; j < moustaches_anger.Length; j++)
                        {
                            if (moustaches_anger[j] > moustaches.GetBlendShapeWeight(anger_moustaches[j]))
                            {
                                int difference = moustaches_anger[j] - (int)moustaches.GetBlendShapeWeight(anger_moustaches[j]);
                                moustaches.SetBlendShapeWeight(anger_moustaches[j], (moustaches.GetBlendShapeWeight(anger_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(anger_moustaches[j]) - moustaches_anger[j];
                                moustaches.SetBlendShapeWeight(anger_moustaches[j], (moustaches.GetBlendShapeWeight(anger_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }

                    yield return null;
                }
            }

            duration = 2.0f;
            animation_lock = true;
        }
    }

    IEnumerator Fear(float duration)
    {
        StartCoroutine(tightenEyes());

        //float time = 0f;
        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 5;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        int[] face_sadnessUjoyUsurpriseUangerUdisgust = sadness_face.Union(joy_face).Union(surprise_face).Union(anger_face).Union(disgust_face).ToArray();
        int[] eyelashes_sadnessUjoyUsurpriseUangerUdisgust = sadness_eyelashes.Union(joy_eyelashes).Union(surprise_eyelashes).Union(anger_eyelashes).Union(disgust_eyelashes).ToArray();
        int[] moustaches_sadnessUjoyUsurpriseUangerUdisgust = sadness_moustaches.Union(joy_moustaches).Union(surprise_moustaches).Union(anger_moustaches).Union(disgust_moustaches).ToArray();

        Array.Sort(face_sadnessUjoyUsurpriseUangerUdisgust);
        Array.Sort(eyelashes_sadnessUjoyUsurpriseUangerUdisgust);
        Array.Sort(moustaches_sadnessUjoyUsurpriseUangerUdisgust);

        IEnumerable<int> face_result = face_sadnessUjoyUsurpriseUangerUdisgust.Except(fear_face);
        IEnumerable<int> eyelashes_result = eyelashes_sadnessUjoyUsurpriseUangerUdisgust.Except(fear_eyelashes);
        IEnumerable<int> moustaches_result = moustaches_sadnessUjoyUsurpriseUangerUdisgust.Except(fear_moustaches);

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        while (emotion_id == 5)
        {
            if (animation_lock)
            {
                animation_lock = false;
                float time = 0f;

                float animation_frequency = UnityEngine.Random.Range(0.5f, 1.0f); //OLD 0.0 1.0

                yield return new WaitForSeconds(animation_frequency);

                int BrowsDown_Left = (int)rnd.Next(65, 100); // OLD 90 100
                int BrowsDown_Right = (int)rnd.Next(65, 100); // OLD 90 100
                int BrowsIn_Left = (int)rnd.Next(60, 100); // OLD 60 80
                int BrowsIn_Right = (int)rnd.Next(60, 100); // OLD 60 80
                int BrowsOuterLower_Left = (int)rnd.Next(60, 80); // OLD 70 80
                int BrowsOuterLower_Right = (int)rnd.Next(60, 80); // OLD 70 80
                int BrowsUp_Left = (int)rnd.Next(15, 40); // OLD 15 25
                int BrowsUp_Right = (int)rnd.Next(15, 40); // OLD 15 25
                int EyesWide_Left = 100;
                int EyesWide_Right = EyesWide_Left;
                int Frown_Left; // NEW
                int Frown_Right;
                int JawBackward = (int)rnd.Next(40, 60);
                int Jaw_Down = (int)rnd.Next(0, 30);
                int LowerLipDown_Left; // inv
                int LowerLipDown_Right; // inv
                int Midmouth_Left;
                int Midmouth_Right; // NEW
                int MouthDown = (int)rnd.Next(30, 35);
                int MouthNarrow_Left;
                int MouthNarrow_Right; // NEW
                int NoseScrunch_Left; // inv
                int NoseScrunch_Right; // inv
                int Smile_Left = (int)rnd.Next(0, 10);
                int Smile_Right = (int)rnd.Next(0, 10);
                int UpperLipIn = (int)rnd.Next(70, 100);
                int UpperLipUp_Left = (int)rnd.Next(20, 35);
                int UpperLipUp_Right = (int)rnd.Next(20, 35);

                int category = (int)rnd.Next(0, 15); // da 0 a 14, 15 escluso

                if (category <= 4)
                {
                    Frown_Left = 0; // NEW
                    Frown_Right = (int)rnd.Next(10, 20);
                    LowerLipDown_Left = (int)rnd.Next(30, 50);
                    LowerLipDown_Right = (int)rnd.Next(15, 25);
                    Midmouth_Left = (int)rnd.Next(0, 15);
                    Midmouth_Right = 0; // NEW
                    MouthNarrow_Left = (int)rnd.Next(40, 60);
                    MouthNarrow_Right = 0; // NEW
                    NoseScrunch_Left = (int)rnd.Next(20, 35);
                    NoseScrunch_Right = (int)rnd.Next(10, 15);
                }
                else if (category > 4 && category <10)
                {
                    Frown_Left = (int)rnd.Next(10, 20);
                    Frown_Right = 0;
                    LowerLipDown_Left = (int)rnd.Next(15, 25);
                    LowerLipDown_Right = (int)rnd.Next(30, 50);
                    Midmouth_Left = 0;
                    Midmouth_Right = (int)rnd.Next(0, 15);
                    MouthNarrow_Left = 0;
                    MouthNarrow_Right = (int)rnd.Next(40, 60);
                    NoseScrunch_Left = (int)rnd.Next(10, 15);
                    NoseScrunch_Right = (int)rnd.Next(20, 35);
                }
                else
                {
                    Frown_Left = (int)rnd.Next(10, 20);
                    Frown_Right = (int)rnd.Next(10, 20);
                    LowerLipDown_Left = (int)rnd.Next(15, 50);
                    LowerLipDown_Right = (int)rnd.Next(15, 50);
                    Midmouth_Left = (int)rnd.Next(0, 15);
                    Midmouth_Right = (int)rnd.Next(0, 15);
                    MouthNarrow_Left = (int)rnd.Next(40, 60);
                    MouthNarrow_Right = (int)rnd.Next(40, 60);
                    NoseScrunch_Left = (int)rnd.Next(10, 35);
                    NoseScrunch_Right = (int)rnd.Next(10, 35);
                }

                // Fix for Speaking
                if (speak)
                {
                    //MouthOpen = (int)rnd.Next(0, 10);
                    //MouthUp = (int)rnd.Next(20, 30);
                    MouthNarrow_Left = (int)rnd.Next(0, 10);
                    MouthNarrow_Right = MouthNarrow_Left;
                }

                int[] face_fear = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right,
                                       BrowsUp_Left, BrowsUp_Right, EyesWide_Left, EyesWide_Right, Frown_Left, Frown_Right, JawBackward, Jaw_Down, LowerLipDown_Left,
                                       LowerLipDown_Right, Midmouth_Left, Midmouth_Right, MouthDown, MouthNarrow_Left, MouthNarrow_Right,
                                       NoseScrunch_Left, NoseScrunch_Right, Smile_Left, Smile_Right, UpperLipIn, UpperLipUp_Left, UpperLipUp_Right};

                int[] eyelashes_fear = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right,
                                           BrowsUp_Left, BrowsUp_Right, EyesWide_Left, EyesWide_Right };

                int[] moustaches_fear = new int[] { Frown_Left, Frown_Right, JawBackward, Jaw_Down, LowerLipDown_Left, LowerLipDown_Right, Midmouth_Left,
                                            Midmouth_Right, MouthDown, MouthNarrow_Left, MouthNarrow_Right, NoseScrunch_Left, NoseScrunch_Right,
                                            Smile_Left, Smile_Right, UpperLipIn, UpperLipUp_Left, UpperLipUp_Right};

                while (time <= duration && emotion_id == 5)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    // Reset unused facial muscles
                    foreach (var n in face_result)
                    {
                        face.SetBlendShapeWeight(n, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(n)));
                    }

                    // Reset unused eyelashes muscles
                    foreach (var m in eyelashes_result)
                    {
                        eyelashes.SetBlendShapeWeight(m, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(m)));
                    }

                    // Reset unused moustaches 
                    if (moustaches != null)
                    {
                        foreach (var k in moustaches_result)
                        {
                            moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                        }
                    }

                    // Fix for Speaking
                    if (speak)
                    {
                        face_fear[19] = (int)rnd.Next(0, 10);
                        face_fear[20] = face_fear[19];
                        moustaches_fear[9] = face_fear[19];
                        moustaches_fear[10] = face_fear[20];
                    }

                    // Set fear facial muscles
                    for (int k = 0; k < face_fear.Length; k++)
                    {
                        if (face_fear[k] > face.GetBlendShapeWeight(fear_face[k]))
                        {
                            int difference = face_fear[k] - (int)face.GetBlendShapeWeight(fear_face[k]);
                            face.SetBlendShapeWeight(fear_face[k], (face.GetBlendShapeWeight(fear_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(fear_face[k]) - face_fear[k];
                            face.SetBlendShapeWeight(fear_face[k], (face.GetBlendShapeWeight(fear_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set fear eyelashes muscles
                    for (int j = 0; j < eyelashes_fear.Length; j++)
                    {
                        if (eyelashes_fear[j] > eyelashes.GetBlendShapeWeight(fear_eyelashes[j]))
                        {
                            int difference = eyelashes_fear[j] - (int)eyelashes.GetBlendShapeWeight(fear_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(fear_eyelashes[j], (eyelashes.GetBlendShapeWeight(fear_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(fear_eyelashes[j]) - eyelashes_fear[j];
                            eyelashes.SetBlendShapeWeight(fear_eyelashes[j], (eyelashes.GetBlendShapeWeight(fear_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set fear moustaches
                    if (moustaches != null)
                    {
                        for (int j = 0; j < moustaches_fear.Length; j++)
                        {
                            if (moustaches_fear[j] > moustaches.GetBlendShapeWeight(fear_moustaches[j]))
                            {
                                int difference = moustaches_fear[j] - (int)moustaches.GetBlendShapeWeight(fear_moustaches[j]);
                                moustaches.SetBlendShapeWeight(fear_moustaches[j], (moustaches.GetBlendShapeWeight(fear_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(fear_moustaches[j]) - moustaches_fear[j];
                                moustaches.SetBlendShapeWeight(fear_moustaches[j], (moustaches.GetBlendShapeWeight(fear_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }

                    yield return null;
                }
            }

            duration = 2.5f; // OLD 2.0
            animation_lock = true;
        }
    }

    IEnumerator Disgust(float duration)
    {
        StartCoroutine(tightenEyes());

        //float time = 0f;
        eyes_frequency = 0;
        head_frequency = 0;
        blink_frequency = 3.5f;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        int[] face_sadnessUjoyUsurpriseUangerUfear = sadness_face.Union(joy_face).Union(surprise_face).Union(anger_face).Union(fear_face).ToArray();
        int[] eyelashes_sadnessUjoyUsurpriseUangerUfear = sadness_eyelashes.Union(joy_eyelashes).Union(surprise_eyelashes).Union(anger_eyelashes).Union(fear_eyelashes).ToArray();
        int[] moustaches_sadnessUjoyUsurpriseUangerUfear = sadness_moustaches.Union(joy_moustaches).Union(surprise_moustaches).Union(anger_moustaches).Union(fear_moustaches).ToArray();

        Array.Sort(face_sadnessUjoyUsurpriseUangerUfear);
        Array.Sort(eyelashes_sadnessUjoyUsurpriseUangerUfear);
        Array.Sort(moustaches_sadnessUjoyUsurpriseUangerUfear);

        IEnumerable<int> face_result = face_sadnessUjoyUsurpriseUangerUfear.Except(disgust_face);
        IEnumerable<int> eyelashes_result = eyelashes_sadnessUjoyUsurpriseUangerUfear.Except(disgust_eyelashes);
        IEnumerable<int> moustaches_result = moustaches_sadnessUjoyUsurpriseUangerUfear.Except(disgust_moustaches);

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        while (emotion_id == 6)
        {
            if (animation_lock)
            {
                animation_lock = false;
                float time = 0f;

                float animation_frequency = UnityEngine.Random.Range(0.0f, 1.0f);

                yield return new WaitForSeconds(animation_frequency);

                int Blink_Left = (int)rnd.Next(0, 5);
                int Blink_Right = Blink_Left;
                int BrowsDown_Left = (int)rnd.Next(80, 100); // OLD 90 100
                int BrowsDown_Right = BrowsDown_Left; // OLD 90 100
                int BrowsIn_Left = (int)rnd.Next(20, 45); // OLD 50 75
                int BrowsIn_Right = BrowsIn_Left; // OLD 50 75
                int BrowsOuterLower_Left = (int)rnd.Next(70, 100); // OLD 0 70
                int BrowsOuterLower_Right = BrowsOuterLower_Left; // OLD 0 70
                int BrowsUp_Left = (int)rnd.Next(-20, 0); // OLD 0 20
                int BrowsUp_Right = BrowsUp_Left; // NEW
                int Frown_Left = (int)rnd.Next(100, 140); // OLD 100 150
                int Frown_Right = (int)rnd.Next(100, 140); // OLD 100 150
                int JawForeward = (int)rnd.Next(50, 100); // NEW
                int Jaw_Up = (int)rnd.Next(50, 70); // OLD 20 30
                int LowerLipDown_Left = (int)rnd.Next(10, 20); // OLD 0 10
                int LowerLipDown_Right = (int)rnd.Next(10, 20); // OLD 0 10
                int LowerLipIn = (int)rnd.Next(50, 70); // OLD 80 100
                int LowerLipOut = (int)rnd.Next(80, 100); // NEW
                int MouthNarrow_Left = (int)rnd.Next(-50, -10); // OLD -30 0
                int MouthNarrow_Right = MouthNarrow_Left; // OLD -30 0
                int MouthUp = (int)rnd.Next(80, 120); // OLD 70 100
                int NoseScrunch_Left = (int)rnd.Next(70, 150); // OLD 80 150
                int NoseScrunch_Right = NoseScrunch_Left; // OLD 80 100
                int Squint_Left = (int)rnd.Next(80, 130); // OLD 80 140
                int Squint_Right = (int)rnd.Next(80, 130); // OLD 80 140
                int UpperLipOut = (int)rnd.Next(60, 80); // OLD 80 100
                int UpperLipUp_Left = (int)rnd.Next(0, 30); // OLD 30 50
                int UpperLipUp_Right = UpperLipUp_Left; // OLD 30 50

                // NEW TEMPORAL CODE
                if (name == "Policeman")
                {
                    JawForeward = (int)rnd.Next(0, 50);
                    UpperLipOut = (int)rnd.Next(0, 10);
                }
                //

                // Fix for Speaking
                if (speak)
                {
                    Frown_Left = (int)rnd.Next(70, 80);
                    Frown_Right = (int)rnd.Next(70, 80);
                    JawForeward = (int)rnd.Next(70, 80);
                    MouthNarrow_Left = (int)rnd.Next(-30, -20);
                    MouthNarrow_Right = MouthNarrow_Left;
                    MouthUp = (int)rnd.Next(20, 30);
                    NoseScrunch_Left = (int)rnd.Next(70, 80);
                    NoseScrunch_Right = NoseScrunch_Left;
                    UpperLipOut = (int)rnd.Next(20, 30);
                }

                int[] face_disgust = new int[] { Blink_Left, Blink_Right, BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right,
                                                 BrowsUp_Left, BrowsUp_Right, Frown_Left, Frown_Right, JawForeward, Jaw_Up, LowerLipDown_Left,  LowerLipDown_Right, LowerLipIn,
                                                 LowerLipOut, MouthNarrow_Left, MouthNarrow_Right, MouthUp, NoseScrunch_Left, NoseScrunch_Right, Squint_Left, Squint_Right,
                                                 UpperLipOut, UpperLipUp_Left, UpperLipUp_Right};

                int[] eyelashes_disgust = new int[] { Blink_Left, Blink_Right, BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right,
                                                      BrowsOuterLower_Left, BrowsOuterLower_Right, BrowsUp_Left, BrowsUp_Right, Squint_Left, Squint_Right };

                int[] moustaches_disgust = new int[] { Frown_Left, Frown_Right, JawForeward, Jaw_Up, LowerLipIn, LowerLipOut, MouthNarrow_Left, MouthNarrow_Right, MouthUp, 
                                                       NoseScrunch_Left, NoseScrunch_Right, UpperLipOut, UpperLipUp_Left, UpperLipUp_Right };

                while (time <= duration && emotion_id == 6)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    // Reset unused facial muscles
                    foreach (var n in face_result)
                    {
                        face.SetBlendShapeWeight(n, (end_curve.Evaluate(percent) * face.GetBlendShapeWeight(n)));
                    }

                    // Reset unused eyelashes muscles
                    foreach (var m in eyelashes_result)
                    {
                        eyelashes.SetBlendShapeWeight(m, (end_curve.Evaluate(percent) * eyelashes.GetBlendShapeWeight(m)));
                    }

                    // Reset unused moustaches 
                    if (moustaches != null)
                    {
                        foreach (var k in moustaches_result)
                        {
                            moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                        }
                    }

                    // Fix for Speaking
                    if (speak)
                    { 
                        face_disgust[10] = (int)rnd.Next(70, 80);
                        face_disgust[11] = (int)rnd.Next(70, 80);
                        face_disgust[12] = (int)rnd.Next(70, 80);
                        face_disgust[18] = (int)rnd.Next(-30, -20);
                        face_disgust[19] = face_disgust[18];
                        face_disgust[20] = (int)rnd.Next(10, 20);
                        face_disgust[21] = (int)rnd.Next(70, 80);
                        face_disgust[22] = face_disgust[21];
                        face_disgust[25] = (int)rnd.Next(20, 30);
                        moustaches_disgust[0] = face_disgust[10];
                        moustaches_disgust[1] = face_disgust[11];
                        moustaches_disgust[2] = face_disgust[12];
                        moustaches_disgust[6] = face_disgust[18];
                        moustaches_disgust[7] = face_disgust[19];
                        moustaches_disgust[8] = face_disgust[20];
                        moustaches_disgust[9] = face_disgust[21];
                        moustaches_disgust[10] = face_disgust[22];
                        moustaches_disgust[12] = face_disgust[25];
                    }

                    // Set disgust facial muscles
                    for (int k = 0; k < face_disgust.Length; k++)
                    {
                        if (face_disgust[k] > face.GetBlendShapeWeight(disgust_face[k]))
                        {
                            int difference = face_disgust[k] - (int)face.GetBlendShapeWeight(disgust_face[k]);
                            face.SetBlendShapeWeight(disgust_face[k], (face.GetBlendShapeWeight(disgust_face[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(disgust_face[k]) - face_disgust[k];
                            face.SetBlendShapeWeight(disgust_face[k], (face.GetBlendShapeWeight(disgust_face[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set disgust eyelashes muscles
                    for (int j = 0; j < eyelashes_disgust.Length; j++)
                    {
                        if (eyelashes_disgust[j] > eyelashes.GetBlendShapeWeight(disgust_eyelashes[j]))
                        {
                            int difference = eyelashes_disgust[j] - (int)eyelashes.GetBlendShapeWeight(disgust_eyelashes[j]);
                            eyelashes.SetBlendShapeWeight(disgust_eyelashes[j], (eyelashes.GetBlendShapeWeight(disgust_eyelashes[j]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)eyelashes.GetBlendShapeWeight(disgust_eyelashes[j]) - eyelashes_disgust[j];
                            eyelashes.SetBlendShapeWeight(disgust_eyelashes[j], (eyelashes.GetBlendShapeWeight(disgust_eyelashes[j]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    // Set disgust moustaches
                    if (moustaches != null)
                    {
                        for (int j = 0; j < moustaches_disgust.Length; j++)
                        {
                            if (moustaches_disgust[j] > moustaches.GetBlendShapeWeight(disgust_moustaches[j]))
                            {
                                int difference = moustaches_disgust[j] - (int)moustaches.GetBlendShapeWeight(disgust_moustaches[j]);
                                moustaches.SetBlendShapeWeight(disgust_moustaches[j], (moustaches.GetBlendShapeWeight(disgust_moustaches[j]) + curve.Evaluate(percent) * difference));
                            }
                            else
                            {
                                int difference = (int)moustaches.GetBlendShapeWeight(disgust_moustaches[j]) - moustaches_disgust[j];
                                moustaches.SetBlendShapeWeight(disgust_moustaches[j], (moustaches.GetBlendShapeWeight(disgust_moustaches[j]) - curve.Evaluate(percent) * difference));
                            }
                        }
                    }

                    yield return null;
                }
            }

            duration = 2.0f;
            animation_lock = true;
        }
    }

    public IEnumerator Speaking()
    {
        /*int Jaw_Down = 0;
        int LowerLipDown_Left = 0;
        int LowerLipDown_Right = 0;
        int LowerLipIn = 0;
        int LowerLipOut = 0;
        int Midmouth_Left = 0;
        int Midmouth_Right = 0;
        int Smile_Left = 0;
        int Smile_Right = 0;
        int TongueUp = 0;
        int UpperLipIn = 0;
        int UpperLipUp_Left = 0;
        int UpperLipUp_Right = 0;*/

        /*int[,] lip = new int[7, 13] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                                           { 35, 48, 51, 86, 0, 30, 30, 0, 0, 5, 0, 27, 27 },
                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                           { 45, 48, 51, 86, 0, 0, 0, 15, 15, 30, 0, 27, 27 },
                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                           { 25, 10, 10, 0, 50, 40, 40, 0, 0, 0, 0, 20, 20 },
                                           { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

        int[] lip_face_index = new int[] { 22, 26, 27, 28, 29, 30, 31, 41, 42, 45, 46, 48, 49 };

        for(int j = 0; j < 6; j++)
        {
            for (int i = 0; i < lip.GetLength(0); i++)
            {
                float time = 0f;
                float duration = 0.10f;

                while (time <= duration)
                {
                    time += Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    Debug.Log("Time: " + time + " Duration: " + duration);

                    // Set lip facial muscles
                    for (int k = 0; k < lip_face_index.Length; k++)
                    {
                        if (lip[i, k] > face.GetBlendShapeWeight(lip_face_index[k]))
                        {
                            int difference = lip[i, k] - (int)face.GetBlendShapeWeight(lip_face_index[k]);
                            face.SetBlendShapeWeight(lip_face_index[k], (face.GetBlendShapeWeight(lip_face_index[k]) + curve.Evaluate(percent) * difference));
                        }
                        else
                        {
                            int difference = (int)face.GetBlendShapeWeight(lip_face_index[k]) - lip[i, k];
                            face.SetBlendShapeWeight(lip_face_index[k], (face.GetBlendShapeWeight(lip_face_index[k]) - curve.Evaluate(percent) * difference));
                        }
                    }

                    Debug.Log("Sto nel while..");
                    yield return null;
                }

                yield return null;
            }
        }*/

        speak = true;

        interlocutor.GetComponent<FacialExpressions>().Listening(); // Start Interlocutor Listening

        head_frequency = 0f;
        eyes_frequency = 0f;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        //interlocutor.GetComponent<FacialExpressions>().Listening(); // Start Interlocutor Listening

        GetComponent<Animation>().Play();

        yield return new WaitForSeconds(4.0f);

        GetComponent<Animation>().Stop();

        speak = false;

        head_frequency = 0f;
        eyes_frequency = 0f;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));

        interlocutor.GetComponent<FacialExpressions>().Listening(); // End Interlocutor Listening
    }

    public void Listening()
    {
        listen = !listen;

        //Debug.Log(gameObject.name + " listen");

        head_frequency = 0f;
        eyes_frequency = 0f;

        StopCoroutine(coroutine_Eyes);
        StopCoroutine(coroutine_Head);
        coroutine_Eyes = StartCoroutine(Eyes(0.8f));
        coroutine_Head = StartCoroutine(Head(1.5f));
    }

    // Functions called by Finite State Machine "CoreAffect.cs"

    public void setNeutral()
    {
        emotion_id = 0;
        StartCoroutine(Neutral(0.5f));
    }

    public void setSadness()
    {
        emotion_id = 1;
        StartCoroutine(Sadness(0.5f));
    }

    public void setJoy()
    {
        emotion_id = 2;
        StartCoroutine(Joy(0.5f));
    }

    public void setSurprise()
    {
        emotion_id = 3;
        StartCoroutine(Surprise(0.5f));
    }

    public void setAnger()
    {
        emotion_id = 4;
        StartCoroutine(Anger(0.5f));
    }

    public void setFear()
    {
        emotion_id = 5;
        StartCoroutine(Fear(0.5f));
    }

    public void setDisgust()
    {
        emotion_id = 6;
        StartCoroutine(Disgust(0.5f));
    }

    public void setSleepiness()
    {
        //emotion_id = 7;
        //StartCoroutine(Sleepiness(0.5f));
    }

    public void setCalmness()
    {
        //emotion_id = 8;
        //StartCoroutine(Calmness(0.5f));
    }
}
