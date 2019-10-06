using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CoreAffect : MonoBehaviour
{
    public string status = ""; // stato emotivo di partenza

    private int actual_status = -1;

    // EMOZIONI DI BASE: questi valori sono inclusi nell'intervallo (0, 10), dove 0 = niente, 10 = molto
    [Range(0, 10)] public int Neutral = 0;
    [Range(0, 10)] public int Sadness = 0;
    [Range(0, 10)] public int Joy = 0;
    [Range(0, 10)] public int Surprise = 0;
    [Range(0, 10)] public int Anger = 0;
    [Range(0, 10)] public int Fear = 0;
    [Range(0, 10)] public int Disgust = 0;
    [Range(0, 10)] public int Sleepiness = 0;
    [Range(0, 10)] public int Calmness = 0;

    private float reactionTime = 1f; // tempo di reazione del character

    private FSM fsm;
    private FacialExpressions face;
    private FSMState neutral;
    private FSMState sadness;
    private FSMState joy;
    private FSMState surprise;
    private FSMState anger;
    private FSMState fear;
    private FSMState disgust;
    private FSMState sleepiness;
    private FSMState calmness;

    void Start()
    {
        // Define states and link actions when enter/exit/stay

        neutral = new FSMState();
        neutral.enterActions = new FSMAction[] { setNeutral };
        neutral.stayActions = new FSMAction[] { };
        neutral.exitActions = new FSMAction[] { };

        sadness = new FSMState();
        sadness.enterActions = new FSMAction[] { setSadness };
        sadness.stayActions = new FSMAction[] { };
        sadness.exitActions = new FSMAction[] { };

        joy = new FSMState();
        joy.enterActions = new FSMAction[] { setJoy };
        joy.stayActions = new FSMAction[] { };
        joy.exitActions = new FSMAction[] { };

        surprise = new FSMState();
        surprise.enterActions = new FSMAction[] { setSurprise };
        surprise.stayActions = new FSMAction[] { };
        surprise.exitActions = new FSMAction[] { };

        anger = new FSMState();
        anger.enterActions = new FSMAction[] { setAnger };
        anger.stayActions = new FSMAction[] { };
        anger.exitActions = new FSMAction[] { };

        fear = new FSMState();
        fear.enterActions = new FSMAction[] { setFear };
        fear.stayActions = new FSMAction[] { };
        fear.exitActions = new FSMAction[] { };

        disgust = new FSMState();
        disgust.enterActions = new FSMAction[] { setDisgust };
        disgust.stayActions = new FSMAction[] { };
        disgust.exitActions = new FSMAction[] { };

        sleepiness = new FSMState();
        sleepiness.enterActions = new FSMAction[] { setSleepiness };
        sleepiness.stayActions = new FSMAction[] { };
        sleepiness.exitActions = new FSMAction[] { };

        calmness = new FSMState();
        calmness.enterActions = new FSMAction[] { setCalmness };
        calmness.stayActions = new FSMAction[] { };
        calmness.exitActions = new FSMAction[] { };


        // Define transitions

        FSMTransition t0 = new FSMTransition(isNeutral);
        FSMTransition t1 = new FSMTransition(isSadness);
        FSMTransition t2 = new FSMTransition(isJoy);
        FSMTransition t3 = new FSMTransition(isSurprise);
        FSMTransition t4 = new FSMTransition(isAnger);
        FSMTransition t5 = new FSMTransition(isFear);
        FSMTransition t6 = new FSMTransition(isDisgust);
        FSMTransition t7 = new FSMTransition(isSleepiness);
        FSMTransition t8 = new FSMTransition(isCalmness);


        // Link states with transitions

        sadness.AddTransition(t0, neutral);
        joy.AddTransition(t0, neutral);
        surprise.AddTransition(t0, neutral);
        anger.AddTransition(t0, neutral);
        fear.AddTransition(t0, neutral);
        disgust.AddTransition(t0, neutral);
        sleepiness.AddTransition(t0, neutral);
        calmness.AddTransition(t0, neutral);

        neutral.AddTransition(t1, sadness);
        joy.AddTransition(t1, sadness);
        surprise.AddTransition(t1, sadness);
        anger.AddTransition(t1, sadness);
        fear.AddTransition(t1, sadness);
        disgust.AddTransition(t1, sadness);
        sleepiness.AddTransition(t1, sadness);
        calmness.AddTransition(t1, sadness);

        neutral.AddTransition(t2, joy);
        sadness.AddTransition(t2, joy);
        surprise.AddTransition(t2, joy);
        anger.AddTransition(t2, joy);
        fear.AddTransition(t2, joy);
        disgust.AddTransition(t2, joy);
        sleepiness.AddTransition(t2, joy);
        calmness.AddTransition(t2, joy);

        neutral.AddTransition(t3, surprise);
        sadness.AddTransition(t3, surprise);
        joy.AddTransition(t3, surprise);
        anger.AddTransition(t3, surprise);
        fear.AddTransition(t3, surprise);
        disgust.AddTransition(t3, surprise);
        sleepiness.AddTransition(t3, surprise);
        calmness.AddTransition(t3, surprise);

        neutral.AddTransition(t4, anger);
        sadness.AddTransition(t4, anger);
        joy.AddTransition(t4, anger);
        surprise.AddTransition(t4, anger);
        fear.AddTransition(t4, anger);
        disgust.AddTransition(t4, anger);
        sleepiness.AddTransition(t4, anger);
        calmness.AddTransition(t4, anger);

        neutral.AddTransition(t5, fear);
        sadness.AddTransition(t5, fear);
        joy.AddTransition(t5, fear);
        surprise.AddTransition(t5, fear);
        anger.AddTransition(t5, fear);
        disgust.AddTransition(t5, fear);
        sleepiness.AddTransition(t5, fear);
        calmness.AddTransition(t5, fear);

        neutral.AddTransition(t6, disgust);
        sadness.AddTransition(t6, disgust);
        joy.AddTransition(t6, disgust);
        surprise.AddTransition(t6, disgust);
        anger.AddTransition(t6, disgust);
        fear.AddTransition(t6, disgust);
        sleepiness.AddTransition(t6, disgust);
        calmness.AddTransition(t6, disgust);

        neutral.AddTransition(t7, sleepiness);
        sadness.AddTransition(t7, sleepiness);
        joy.AddTransition(t7, sleepiness);
        surprise.AddTransition(t7, sleepiness);
        anger.AddTransition(t7, sleepiness);
        disgust.AddTransition(t7, sleepiness);
        fear.AddTransition(t7, sleepiness);
        calmness.AddTransition(t7, sleepiness);

        neutral.AddTransition(t8, calmness);
        sadness.AddTransition(t8, calmness);
        joy.AddTransition(t8, calmness);
        surprise.AddTransition(t8, calmness);
        anger.AddTransition(t8, calmness);
        disgust.AddTransition(t8, calmness);
        fear.AddTransition(t8, calmness);
        sleepiness.AddTransition(t8, calmness);


        // Capture Facial Expressions Script

        face = GetComponent<FacialExpressions>();


        // Setup a FSA at initial state

        setInitialStatus(status);


        // Start monitoring

        StartCoroutine(Status());
    }


    // Periodic update, run forever
    public IEnumerator Status()
    {
        while (true)
        {
            fsm.Update();
            yield return new WaitForSeconds(reactionTime);
        }
    }


    // Conditions

    public bool isNeutral()
    {
        //Debug.Log("isNeutral");

        if (dominantExpression() == 0)
        {
            return true;
        }

        return false;
    }

    public bool isSadness()
    {
        //Debug.Log("isSadness");

        if (dominantExpression() == 1)
        {
            return true;
        }

        return false;
    }

    public bool isJoy()
    {
        //Debug.Log("isJoy");

        if (dominantExpression() == 2)
        {
            return true;
        }

        return false;
    }

    public bool isSurprise()
    {
        //Debug.Log("isSurprise");

        if (dominantExpression() == 3)
        {
            return true;
        }

        return false;
    }

    public bool isAnger()
    {
        //Debug.Log("isAnger");

        if (dominantExpression() == 4)
        {
            return true;
        }

        return false;
    }

    public bool isFear()
    {
        //Debug.Log("isFear");

        if (dominantExpression() == 5)
        {
            return true;
        }

        return false;
    }

    public bool isDisgust()
    {
        //Debug.Log("isDisgust");

        if (dominantExpression() == 6)
        {
            return true;
        }

        return false;
    }

    public bool isSleepiness()
    {
        //Debug.Log("isSleepiness");

        if (dominantExpression() == 7)
        {
            return true;
        }

        return false;
    }

    public bool isCalmness()
    {
        //Debug.Log("isCalmness");

        if (dominantExpression() == 8)
        {
            return true;
        }

        return false;
    }


    // Actions

    public void setNeutral()
    {
        Debug.Log(gameObject.name + ": setNeutral");
        face.setNeutral();
        status = "neutral";
        actual_status = 0;
    }

    public void setSadness()
    {
        Debug.Log(gameObject.name + ": setSadness");
        face.setSadness();
        status = "sadness";
        actual_status = 1;
    }

    public void setJoy()
    {
        Debug.Log(gameObject.name + ": setJoy");
        face.setJoy();
        status = "joy";
        actual_status = 2;
    }

    public void setSurprise()
    {
        Debug.Log(gameObject.name + ": setSurprise");
        face.setSurprise();
        status = "surprise";
        actual_status = 3;
    }

    public void setAnger()
    {
        Debug.Log(gameObject.name + ": setAnger");
        face.setAnger();
        status = "anger";
        actual_status = 4;
    }

    public void setFear()
    {
        Debug.Log(gameObject.name + ": setFear");
        face.setFear();
        status = "fear";
        actual_status = 5;
    }

    public void setDisgust()
    {
        Debug.Log(gameObject.name + ": setDisgust");
        face.setDisgust();
        status = "disgust";
        actual_status = 6;
    }

    public void setSleepiness()
    {
        Debug.Log(gameObject.name + ": setSleepiness");
        face.setSleepiness();
        status = "sleepiness";
        actual_status = 7;
    }

    public void setCalmness()
    {
        Debug.Log(gameObject.name + ": setCalmness");
        face.setCalmness();
        status = "calmness";
        actual_status = 8;
    }


    // Other functions

    private void setInitialStatus(string status)
    {
        status.ToLower();

        if(status == "" || status == "neutral")
        {
            Neutral += 1;
            fsm = new FSM(neutral);
        }
        else if (status == "sadness")
        {
            Sadness += 1;
            fsm = new FSM(sadness);
        }
        else if (status == "joy")
        {
            Joy += 1;
            fsm = new FSM(joy);
        }
        else if (status == "surprise")
        {
            Surprise += 1;
            fsm = new FSM(surprise);
        }
        else if (status == "anger")
        {
            Anger += 1;
            fsm = new FSM(anger);
        }
        else if (status == "fear")
        {
            Fear += 1;
            fsm = new FSM(fear);
        }
        else if (status == "disgust")
        {
            Disgust += 1;
            fsm = new FSM(disgust);
        }
        else if (status == "sleepiness")
        {
            Sleepiness += 1;
            fsm = new FSM(sleepiness);
        }
        else if (status == "calmness")
        {
            Calmness += 1;
            fsm = new FSM(calmness);
        }
        else
        {
            Debug.Log("Hai inserito uno status errato! E' stato impostato quello neutrale!");
            Neutral += 1;
            fsm = new FSM(neutral);
        }
    }

    public int[] getEmotions()
    {
        return new int[] { Neutral, Sadness, Joy, Surprise, Anger, Fear, Disgust, Sleepiness, Calmness };
    }

    public void setEmotions(int[] emotions)
    {
        Neutral = normalize(Neutral + emotions[0]);
        Sadness = normalize(Sadness + emotions[1]);
        Joy = normalize(Joy + emotions[2]);
        Surprise = normalize(Surprise + emotions[3]);
        Anger = normalize(Anger + emotions[4]);
        Fear = normalize(Fear + emotions[5]);
        Disgust = normalize(Disgust + emotions[6]);
        Sleepiness = normalize(Sleepiness + emotions[7]);
        Calmness = normalize(Calmness + emotions[8]);
    }

    private int normalize(int value)
    {
        if(value > 10)
        {
            value = 10;
        }
        else if (value < 0)
        {
            value = 0;
        }

        return (value);
    }

    private int dominantExpression()
    {
        int[] Emotions = new int[] { Neutral, Sadness, Joy, Surprise, Anger, Fear, Disgust, Sleepiness, Calmness };
        List<int> dominant_index = new List<int>();
        int maxValue;

        maxValue = Emotions.Max();

        for(int i = 0; i < Emotions.Length; i++)
        {
            if(Emotions[i] == maxValue)
            {
                dominant_index.Add(i);
            } 
        }

        if (!dominant_index.Contains(actual_status))
        {
            System.Random rnd = new System.Random();
            int choice = rnd.Next(0, dominant_index.Count);

            return dominant_index[choice];
        }
        else
        {
            return actual_status;
        }
    }
}
