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
    private LookAtConstraint left_lookAtConstraint;
    private LookAtConstraint right_lookAtConstraint;

    [SerializeField]
    private AnimationCurve animationCurve;

    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private AnimationCurve end_curve;

    private bool blinking = false;
    private float blink_frequency = 3.5f;
    private int emotion_id = 0; // 0 = Neutral Face
    private bool eyes_movement = false;
    private float eyes_frequency = 2f;
    private bool cry = false;

    private int[] sadness_face = new int[] { 0, 1, 4, 5, 6, 7, 10, 11, 14, 15, 17, 26, 27, 28, 32 };
    private int[] sadness_eyelashes = new int[] { 0, 1, 4, 5, 6, 7 };
    private int[] sadness_moustaches = new int[] { 5, 6, 9, 10, 12, 21, 22, 23, 27 };

    private int[] joy_face = new int[] { 6, 7, 10, 11, 35, 39, 40, 41, 42, 43, 44, 48, 49 };
    private int[] joy_eyelashes = new int[] { 6, 7, 43, 44 };
    private int[] joy_moustaches = new int[] { 5, 6, 30, 34, 35, 36, 37, 40, 41 };

    private int[] surprise_face = new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 22, 29, 33, 34, 35 };
    private int[] surprise_eyelashes = new int[] { 6, 7, 8, 9, 12, 13 };
    private int[] surprise_moustaches = new int[] { 5, 6, 9, 10, 17, 24, 28, 29, 30 };

    private int[] anger_face = new int[] { 2, 3, 10, 11, 14, 15, 25, 28, 36, 39, 40, 46 };
    private int[] anger_eyelashes = new int[] { 2, 3 };
    private int[] anger_moustaches = new int[] { 5, 6, 9, 10, 20, 23, 31, 34, 35, 38 };

    private int[] fear_face = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 12, 13, 15, 16, 26, 27, 30, 32, 33, 39, 40, 41, 42, 46, 48, 49 };
    private int[] fear_eyelashes = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 12, 13 };
    private int[] fear_moustaches = new int[] { 10, 11, 21, 22, 25, 27, 28, 34, 35, 36, 37, 38, 40, 41 };

    private int[] disgust_face = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 14, 15, 25, 26, 27, 33, 34, 36, 39, 40, 43, 44, 47, 48, 49};
    private int[] disgust_eyelashes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 43, 44 };
    private int[] disgust_moustaches = new int[] { 9, 10, 20, 21, 22, 28, 29, 31, 34, 35, 39, 40, 41 };

    void Start()
    {
        left_lookAtConstraint = left_eye.GetComponent<LookAtConstraint>();
        right_lookAtConstraint = right_eye.GetComponent<LookAtConstraint>();

        // Active Standard Blinking
        blinking = true;
        StartCoroutine(Blink(0.3f));

        // Active Standard Eyes Movement
        eyes_movement = true;
        StartCoroutine(Eyes(0.8f)); //OLD 1.5
    }

    // Update is called once per frame
    void Update()
    {
        // 1.Neutral
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            emotion_id = 0;
            StartCoroutine(Neutral(0.5f));
        }

        // 1.Sadness
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            emotion_id = 1;
            StartCoroutine(Sadness(0.5f));
        }

        // 2.Joy
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            emotion_id = 2;
            StartCoroutine(Joy(0.5f));
        }

        // 3.Surprise
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            emotion_id = 3;
            StartCoroutine(Surprise(0.5f));
        }

        // 4.Anger
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            emotion_id = 4;
            StartCoroutine(Anger(0.5f));
        }

        // 5.Fear
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            emotion_id = 5;
            StartCoroutine(Fear(0.5f));
        }

        // 6.Disgust
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            emotion_id = 6;
            StartCoroutine(Disgust(0.5f));
        }
    }

    IEnumerator Eyes(float duration)
    {
        while (eyes_movement)
        {
            yield return new WaitForSeconds(eyes_frequency);

            float time = 0f;

            System.Random rnd = new System.Random();
            eyes_frequency = UnityEngine.Random.Range(0.0f, 7.0f); ; // OLD 0.2 5.0
            float up_down = UnityEngine.Random.Range(-5.0f, 15.0f); // OLD -5 15 // OLD -5 8 // OLD -10 10
            float left_right = UnityEngine.Random.Range(-20.0f, 20.0f); // OLD -25 25

            if(emotion_id == 0)
            {
                while (time <= duration)
                {
                    time = time + Time.deltaTime;
                    float percent = Mathf.Clamp01(time / duration);

                    if (up_down > left_lookAtConstraint.rotationAtRest.x)
                    {
                        float difference = up_down - left_lookAtConstraint.rotationAtRest.x;
                        left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x + curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                    }
                    else
                    {
                        float difference = left_lookAtConstraint.rotationAtRest.x - up_down;
                        left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                        right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x - curve.Evaluate(percent) * difference, left_lookAtConstraint.rotationAtRest.y, 0);
                    }

                    if (left_right > left_lookAtConstraint.rotationAtRest.y)
                    {
                        float difference = left_right - left_lookAtConstraint.rotationAtRest.y;
                        left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                        right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y + curve.Evaluate(percent) * difference, 0);
                    }
                    else
                    {
                        float difference = left_lookAtConstraint.rotationAtRest.y - left_right;
                        left_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                        right_lookAtConstraint.rotationAtRest = new Vector3(left_lookAtConstraint.rotationAtRest.x, left_lookAtConstraint.rotationAtRest.y - curve.Evaluate(percent) * difference, 0);
                    }

                    yield return null;
                }
            } else
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
    }

    IEnumerator Blink(float duration)
    {
        while (blinking)
        {
            yield return new WaitForSeconds(blink_frequency);

            float time = 0f;

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
                System.Random rnd = new System.Random();
                int Blink = (int)rnd.Next(20, 30);

                face.SetBlendShapeWeight(0, Blink); // Blink_Left
                face.SetBlendShapeWeight(1, Blink); // Blink_Right
                eyelashes.SetBlendShapeWeight(0, Blink); // Blink_Left
                eyelashes.SetBlendShapeWeight(1, Blink); // Blink_Right
            } else if (emotion_id == 2) // joy
            {
                System.Random rnd = new System.Random();
                int Squint = (int)rnd.Next(70, 80);

                face.SetBlendShapeWeight(43, Squint); // Squint_Left
                face.SetBlendShapeWeight(44, Squint); // Squint_Right
                eyelashes.SetBlendShapeWeight(43, Squint); // Squint_Left
                eyelashes.SetBlendShapeWeight(44, Squint); // Squint_Right
            } else if (emotion_id == 3) // surprise
            {
                System.Random rnd = new System.Random();
                int EyesWide = (int)rnd.Next(120, 150);

                face.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                face.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
                eyelashes.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                eyelashes.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
            } else if (emotion_id == 5) // fear
            {
                int EyesWide = 100;

                eyelashes.SetBlendShapeWeight(12, EyesWide); // EyesWide_Left
                eyelashes.SetBlendShapeWeight(13, EyesWide); // EyesWide_Right
            } else if (emotion_id == 6) // disgust
            {
                System.Random rnd = new System.Random();
                int Blink = (int)rnd.Next(0, 5);
                int Squint_Left = (int)rnd.Next(110, 140);
                int Squint_Right = (int)rnd.Next(110, 140);

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
        tightenEyes();

        System.Random rnd = new System.Random();
        float time_to_cry = UnityEngine.Random.Range(1f, 2f); // OLD 0 0.5

        cry = false;
        StartCoroutine(Crying(time_to_cry)); // Set Crying OFF

        blink_frequency = 3.5f;

        float time = 0f;

        while (time <= duration)
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

            if(moustaches_mesh != null)
            {
                for (int k = 0; k < moustaches_mesh.blendShapeCount; k++)
                {
                    moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                }
            }

            yield return null;
        }
    }

    IEnumerator Sadness(float duration)
    {
        tightenEyes();

        float time = 0f;
        blink_frequency = 2;

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

        while (time <= duration)
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
            if(moustaches != null)
            {
                foreach (var k in moustaches_result)
                {
                    moustaches.SetBlendShapeWeight(k, (end_curve.Evaluate(percent) * moustaches.GetBlendShapeWeight(k)));
                }
            }

            // Set sadness facial muscles
            for (int k=0; k<face_sadness.Length; k++)
            {
                if (face_sadness[k] > face.GetBlendShapeWeight(sadness_face[k]))
                {
                    int difference = face_sadness[k] - (int)face.GetBlendShapeWeight(sadness_face[k]);
                    face.SetBlendShapeWeight(sadness_face[k], (face.GetBlendShapeWeight(sadness_face[k]) + curve.Evaluate(percent) * difference));
                }
                else
                {
                    int difference = (int)face.GetBlendShapeWeight(sadness_face[k]) - face_sadness[k];
                    face.SetBlendShapeWeight(sadness_face[k], (face.GetBlendShapeWeight(sadness_face[k]) - end_curve.Evaluate(percent) * difference));
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
                    eyelashes.SetBlendShapeWeight(sadness_eyelashes[j], (eyelashes.GetBlendShapeWeight(sadness_eyelashes[j]) - end_curve.Evaluate(percent) * difference));
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
                        moustaches.SetBlendShapeWeight(sadness_moustaches[j], (moustaches.GetBlendShapeWeight(sadness_moustaches[j]) - end_curve.Evaluate(percent) * difference));
                    }
                }
            }

            yield return null;
        }
    }

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
        tightenEyes();

        float time = 0f;
        blink_frequency = 3.5f;

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

        int BrowsOuterLower_Left = (int)rnd.Next(70, 100);
        int BrowsOuterLower_Right = (int)rnd.Next(70, 100);
        int CheekPuff_Left = (int)rnd.Next(0, 20);
        int CheekPuff_Right = (int)rnd.Next(0, 20);
        int Mouth_Open = (int)rnd.Next(0, 25); // OLD 40
        int NoseScrunch_Left = (int)rnd.Next(0, 40);
        int NoseScrunch_Right = (int)rnd.Next(0, 40);
        int Smile_Left = (int)rnd.Next(70, 120); // OLD 50-100
        int Smile_Right = (int)rnd.Next(70, 120); // OLD 50-100
        int Squint_Left = (int)rnd.Next(70, 80);
        int Squint_Right = Squint_Left;
        int UpperLipUp_Left = (int)rnd.Next(0, 40); // OLD 30
        int UpperLipUp_Right = (int)rnd.Next(0, 40); // OLD 30

        int[] face_joy = new int[] { BrowsOuterLower_Left, BrowsOuterLower_Right, CheekPuff_Left, CheekPuff_Right, Mouth_Open,
                                     NoseScrunch_Left, NoseScrunch_Right, Smile_Left, Smile_Right, Squint_Left, Squint_Right,
                                     UpperLipUp_Left, UpperLipUp_Right};

        int[] eyelashes_joy = new int[] { BrowsOuterLower_Left, BrowsOuterLower_Right, Squint_Left, Squint_Right };

        int[] moustaches_joy = new int[] { CheekPuff_Left, CheekPuff_Right, Mouth_Open, NoseScrunch_Left, NoseScrunch_Right, Smile_Left,
                                           Smile_Right, UpperLipUp_Left, UpperLipUp_Right };

        while (time <= duration)
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
                    face.SetBlendShapeWeight(joy_face[k], (face.GetBlendShapeWeight(joy_face[k]) - end_curve.Evaluate(percent) * difference));
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
                    eyelashes.SetBlendShapeWeight(joy_eyelashes[j], (eyelashes.GetBlendShapeWeight(joy_eyelashes[j]) - end_curve.Evaluate(percent) * difference));
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
                        moustaches.SetBlendShapeWeight(joy_moustaches[j], (moustaches.GetBlendShapeWeight(joy_moustaches[j]) - end_curve.Evaluate(percent) * difference));
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator Surprise(float duration)
    {
        float time = 0f;
        blink_frequency = 6;

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

        int BrowsOuterLower_Left = (int)rnd.Next(40, 80);
        int BrowsOuterLower_Right = (int)rnd.Next(40, 80);
        int BrowsUp_Left = (int)rnd.Next(40, 60);
        int BrowsUp_Right = (int)rnd.Next(40, 60);
        int CheekPuff_Left = (int)rnd.Next(0, 20);
        int CheekPuff_Right = (int)rnd.Next(0, 20);
        int EyesWide_Left = (int)rnd.Next(120, 150);
        int EyesWide_Right = EyesWide_Left;
        int Frown_Left = (int)rnd.Next(30, 60);
        int Frown_Right = (int)rnd.Next(30, 60);
        int Jaw_Down = (int)rnd.Next(50, 80);
        int LowerLipOut = (int)rnd.Next(40, 60);
        int MouthNarrow_Left = (int)rnd.Next(20, 40);
        int MouthNarrow_Right = (int)rnd.Next(20, 40);
        int Mouth_Open = (int)rnd.Next(30, 40);

        int[] face_surprise = new int[] { BrowsOuterLower_Left, BrowsOuterLower_Right, BrowsUp_Left, BrowsUp_Right, CheekPuff_Left,
                                          CheekPuff_Right, EyesWide_Left, EyesWide_Right, Frown_Left, Frown_Right, Jaw_Down, LowerLipOut,
                                          MouthNarrow_Left, MouthNarrow_Right, Mouth_Open};

        int[] eyelashes_surprise = new int[] { BrowsOuterLower_Left, BrowsOuterLower_Right, BrowsUp_Left, BrowsUp_Right, EyesWide_Left, EyesWide_Right };

        int[] moustaches_surprise = new int[] { CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, Jaw_Down, LowerLipOut, MouthNarrow_Left,
                                                MouthNarrow_Right, Mouth_Open };

        while (time <= duration)
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
                    face.SetBlendShapeWeight(surprise_face[k], (face.GetBlendShapeWeight(surprise_face[k]) - end_curve.Evaluate(percent) * difference));
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
                    eyelashes.SetBlendShapeWeight(surprise_eyelashes[j], (eyelashes.GetBlendShapeWeight(surprise_eyelashes[j]) - end_curve.Evaluate(percent) * difference));
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
                        moustaches.SetBlendShapeWeight(surprise_moustaches[j], (moustaches.GetBlendShapeWeight(surprise_moustaches[j]) - end_curve.Evaluate(percent) * difference));
                    }
                }
            }

            yield return null;
        }

        widensEyes();

    }

    public void widensEyes()
    {
        left_eye.transform.localScale = new Vector3(1.05f, 1.1f, 1.1f);
        right_eye.transform.localScale = new Vector3(1.05f, 1.1f, 1.1f);
    }

    public void tightenEyes()
    {
        left_eye.transform.localScale = new Vector3(1, 1, 1);
        right_eye.transform.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator Anger(float duration)
    {
        tightenEyes();

        float time = 0f;
        blink_frequency = 5;

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

        int BrowsDown_Left = (int)rnd.Next(80, 100);
        int BrowsDown_Right = (int)rnd.Next(80, 100);
        int CheekPuff_Left = (int)rnd.Next(20, 30);
        int CheekPuff_Right = (int)rnd.Next(20, 30);
        int Frown_Left = (int)rnd.Next(20, 35);
        int Frown_Right = (int)rnd.Next(20, 35);
        int Jaw_Up = (int)rnd.Next(20, 50);
        int LowerLipIn = (int)rnd.Next(70, 100);
        int MouthUp = (int)rnd.Next(80, 100);
        int NoseScrunch_Left = (int)rnd.Next(80, 100);
        int NoseScrunch_Right = (int)rnd.Next(80, 100);
        int UpperLipIn = (int)rnd.Next(80, 100);

        int[] face_anger = new int[] { BrowsDown_Left, BrowsDown_Right, CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, Jaw_Up,
                                          LowerLipIn, MouthUp, NoseScrunch_Left, NoseScrunch_Right, UpperLipIn};

        int[] eyelashes_anger = new int[] { BrowsDown_Left, BrowsDown_Right };

        int[] moustaches_anger = new int[] { CheekPuff_Left, CheekPuff_Right, Frown_Left, Frown_Right, Jaw_Up, LowerLipIn, MouthUp, NoseScrunch_Left,
                                             NoseScrunch_Right, UpperLipIn };

        while (time <= duration)
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
                    face.SetBlendShapeWeight(anger_face[k], (face.GetBlendShapeWeight(anger_face[k]) - end_curve.Evaluate(percent) * difference));
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
                    eyelashes.SetBlendShapeWeight(anger_eyelashes[j], (eyelashes.GetBlendShapeWeight(anger_eyelashes[j]) - end_curve.Evaluate(percent) * difference));
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
                        moustaches.SetBlendShapeWeight(anger_moustaches[j], (moustaches.GetBlendShapeWeight(anger_moustaches[j]) - end_curve.Evaluate(percent) * difference));
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator Fear(float duration)
    {
        tightenEyes();

        float time = 0f;
        blink_frequency = 5;

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

        int BrowsDown_Left = (int)rnd.Next(90, 100);
        int BrowsDown_Right = (int)rnd.Next(90, 100);
        int BrowsIn_Left = (int)rnd.Next(60, 80);
        int BrowsIn_Right = (int)rnd.Next(60, 80);
        int BrowsOuterLower_Left = (int)rnd.Next(70, 80);
        int BrowsOuterLower_Right = (int)rnd.Next(70, 80);
        int BrowsUp_Left = (int)rnd.Next(15, 25);
        int BrowsUp_Right = (int)rnd.Next(15, 25);
        int EyesWide_Left = 100;
        int EyesWide_Right = EyesWide_Left;
        int Frown_Right = (int)rnd.Next(10, 20);
        int JawBackward = (int)rnd.Next(40, 60);
        int LowerLipDown_Left = (int)rnd.Next(30, 50);
        int LowerLipDown_Right = (int)rnd.Next(15, 25);
        int Midmouth_Left = (int)rnd.Next(0, 15);
        int MouthDown = (int)rnd.Next(30, 35);
        int MouthNarrow_Left = (int)rnd.Next(40, 60);
        int NoseScrunch_Left = (int)rnd.Next(20, 35);
        int NoseScrunch_Right = (int)rnd.Next(10, 15);
        int Smile_Left = (int)rnd.Next(0, 10);
        int Smile_Right = (int)rnd.Next(0, 10);
        int UpperLipIn = (int)rnd.Next(70, 100);
        int UpperLipUp_Left = (int)rnd.Next(20, 35);
        int UpperLipUp_Right = (int)rnd.Next(20, 35);

        int[] face_fear = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right,
                                       BrowsUp_Left, BrowsUp_Right, EyesWide_Left, EyesWide_Right, Frown_Right, JawBackward, LowerLipDown_Left,
                                       LowerLipDown_Right, Midmouth_Left, MouthDown, MouthNarrow_Left, NoseScrunch_Left, NoseScrunch_Right,
                                       Smile_Left, Smile_Right, UpperLipIn, UpperLipUp_Left, UpperLipUp_Right};

        int[] eyelashes_fear = new int[] { BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left, BrowsOuterLower_Right,
                                           BrowsUp_Left, BrowsUp_Right, EyesWide_Left, EyesWide_Right };

        int[] moustaches_fear = new int[] { Frown_Right, JawBackward, LowerLipDown_Left, LowerLipDown_Right, Midmouth_Left, MouthDown,
                                            MouthNarrow_Left, NoseScrunch_Left, NoseScrunch_Right, Smile_Left, Smile_Right, UpperLipIn,
                                            UpperLipUp_Left, UpperLipUp_Right};

        while (time <= duration)
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
                    face.SetBlendShapeWeight(fear_face[k], (face.GetBlendShapeWeight(fear_face[k]) - end_curve.Evaluate(percent) * difference));
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
                    eyelashes.SetBlendShapeWeight(fear_eyelashes[j], (eyelashes.GetBlendShapeWeight(fear_eyelashes[j]) - end_curve.Evaluate(percent) * difference));
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
                        moustaches.SetBlendShapeWeight(fear_moustaches[j], (moustaches.GetBlendShapeWeight(fear_moustaches[j]) - end_curve.Evaluate(percent) * difference));
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator Disgust(float duration)
    {
        tightenEyes();

        float time = 0f;
        blink_frequency = 3.5f;

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

        int Blink_Left = (int)rnd.Next(0, 5);
        int Blink_Right = Blink_Left;
        int BrowsDown_Left = (int)rnd.Next(90, 100);
        int BrowsDown_Right = (int)rnd.Next(90, 100);
        int BrowsIn_Left = (int)rnd.Next(50, 75);
        int BrowsIn_Right = (int)rnd.Next(50, 75);
        int BrowsOuterLower_Left = (int)rnd.Next(0, 30);
        int BrowsOuterLower_Right = (int)rnd.Next(0, 30);
        int Frown_Left = (int)rnd.Next(0, 20);
        int Frown_Right = Frown_Left;
        int Jaw_Up = (int)rnd.Next(20, 30);
        int LowerLipDown_Left = (int)rnd.Next(0, 10);
        int LowerLipDown_Right = (int)rnd.Next(0, 10);
        int MouthNarrow_Left = (int)rnd.Next(30, 35);
        int MouthNarrow_Right = (int)rnd.Next(30, 35);
        int MouthUp = (int)rnd.Next(95, 100);
        int NoseScrunch_Left = (int)rnd.Next(100, 120);
        int NoseScrunch_Right = (int)rnd.Next(100, 120);
        int Squint_Left = (int)rnd.Next(110, 140);
        int Squint_Right = (int)rnd.Next(110, 140);
        int UpperLipOut = (int)rnd.Next(80, 90);
        int UpperLipUp_Left = (int)rnd.Next(30, 45);
        int UpperLipUp_Right = (int)rnd.Next(30, 45);

        int[] face_disgust = new int[] { Blink_Left, Blink_Right, BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right, BrowsOuterLower_Left,
                                      BrowsOuterLower_Right, Frown_Left, Frown_Right, Jaw_Up, LowerLipDown_Left, LowerLipDown_Right, MouthNarrow_Left,
                                      MouthNarrow_Right, MouthUp, NoseScrunch_Left, NoseScrunch_Right, Squint_Left, Squint_Right, UpperLipOut,
                                      UpperLipUp_Left, UpperLipUp_Right};

        int[] eyelashes_disgust = new int[] { Blink_Left, Blink_Right, BrowsDown_Left, BrowsDown_Right, BrowsIn_Left, BrowsIn_Right,
                                              BrowsOuterLower_Left, BrowsOuterLower_Right, Squint_Left, Squint_Right };

        int[] moustaches_disgust = new int[] { Frown_Left, Frown_Right, Jaw_Up, LowerLipDown_Left, LowerLipDown_Right, MouthNarrow_Left,
                                               MouthNarrow_Right, MouthUp, NoseScrunch_Left, NoseScrunch_Right, UpperLipOut, UpperLipUp_Left,
                                               UpperLipUp_Right };

        while (time <= duration)
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
                    face.SetBlendShapeWeight(disgust_face[k], (face.GetBlendShapeWeight(disgust_face[k]) - end_curve.Evaluate(percent) * difference));
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
                    eyelashes.SetBlendShapeWeight(disgust_eyelashes[j], (eyelashes.GetBlendShapeWeight(disgust_eyelashes[j]) - end_curve.Evaluate(percent) * difference));
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
                        moustaches.SetBlendShapeWeight(disgust_moustaches[j], (moustaches.GetBlendShapeWeight(disgust_moustaches[j]) - end_curve.Evaluate(percent) * difference));
                    }
                }
            }

            yield return null;
        }
    }
}
