using System;
using System.Collections;
using UnityEngine;

public class CoreAffect : MonoBehaviour
{
    public string status = ""; // stato emotivo di partenza

    public float pleasant = 0; // -1 = spiacevole, 1 = piacevole, 0 = neutrale
    public float aroused = 0; // -1 = disattivato, 1 = attivato, 0 = neutrale

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

        //fsm = new FSM(neutral);
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

        if (aroused == 0 && pleasant == 0) // punto neutrale
        {
            return true;
        }

        return false;
    }

    public bool isJoy()
    {
        //Debug.Log("isJoy");

        if ((calculateAngle() >= 0 && calculateAngle() < 45) && !(aroused == 0 && pleasant == 0)) // si trova nello spicchio relativo alla gioia?
        {
            return true;
        }

        return false;
    }

    public bool isSurprise()
    {
        //Debug.Log("isSurprise");

        if (calculateAngle() >= 45 && calculateAngle() < 90) // si trova nello spicchio relativo alla sorpresa?
        {
            return true;
        }

        return false;
    }

    public bool isFear()
    {
        //Debug.Log("isFear");

        if (calculateAngle() >= 90 && calculateAngle() < 135) // si trova nello spicchio relativo alla paura?
        {
            return true;
        }

        return false;
    }

    public bool isAnger()
    {
        //Debug.Log("isAnger");

        if (calculateAngle() >= 135 && calculateAngle() < 180) // si trova nello spicchio relativo alla rabbia?
        {
            return true;
        }

        return false;
    }

    public bool isDisgust()
    {
        //Debug.Log("isDisgust");

        if (calculateAngle() >= 180 && calculateAngle() < 225) // si trova nello spicchio relativo al disgusto?
        {
            return true;
        }

        return false;
    }

    public bool isSadness()
    {
        //Debug.Log("isSadness");

        if (calculateAngle() >= 225 && calculateAngle() < 270) // si trova nello spicchio relativo alla tristezza?
        {
            return true;
        }

        return false;
    }

    public bool isSleepiness()
    {
        //Debug.Log("isSleepiness");

        if (calculateAngle() >= 270 && calculateAngle() < 315) // si trova nello spicchio relativo alla sonnolenza?
        {
            return true;
        }

        return false;
    }

    public bool isCalmness()
    {
        //Debug.Log("isCalmness");

        if (calculateAngle() >= 315 && calculateAngle() < 360) // si trova nello spicchio relativo alla calma?
        {
            return true;
        }

        return false;
    }


    // Actions

    public void setNeutral()
    {
        Debug.Log("setNeutral");
        face.setNeutral();
        status = "neutral";
    }

    public void setSadness()
    {
        Debug.Log("setSadness");
        face.setSadness();
        status = "sadness";
    }

    public void setJoy()
    {
        Debug.Log("setJoy");
        face.setJoy();
        status = "joy";
    }

    public void setSurprise()
    {
        Debug.Log("setSurprise");
        face.setSurprise();
        status = "surprise";
    }

    public void setAnger()
    {
        Debug.Log("setAnger");
        face.setAnger();
        status = "anger";
    }

    public void setFear()
    {
        Debug.Log("setFear");
        face.setFear();
        status = "fear";
    }

    public void setDisgust()
    {
        Debug.Log("setDisgust");
        face.setDisgust();
        status = "disgust";
    }

    public void setSleepiness()
    {
        Debug.Log("setSleepiness");
        face.setSleepiness();
        status = "sleepiness";
    }

    public void setCalmness()
    {
        Debug.Log("setCalmness");
        face.setCalmness();
        status = "calmness";
    }

    public double calculateAngle()
    {
        double radiant = Math.Atan2(aroused - 0, pleasant - 0);
        double angle = radiant * (180 / Math.PI);

        if (angle < 0.0)
        {
            angle += 360.0;
        }

        return angle;
    }

    private void setInitialStatus(string status)
    {
        status.ToLower();

        if(status == "" || status == "neutral")
        {
            pleasant = 0;
            aroused = 0;
            fsm = new FSM(neutral);
        }
        else if (status == "sadness")
        {
            pleasant = -0.35f;
            aroused = -0.85f;
            fsm = new FSM(sadness);
        }
        else if (status == "joy")
        {
            pleasant = 0.85f;
            aroused = 0.35f;
            fsm = new FSM(joy);
        }
        else if (status == "surprise")
        {
            pleasant = 0.35f;
            aroused = 0.85f;
            fsm = new FSM(surprise);
        }
        else if (status == "anger")
        {
            pleasant = -0.85f;
            aroused = 0.35f;
            fsm = new FSM(anger);
        }
        else if (status == "fear")
        {
            pleasant = -0.35f;
            aroused = 0.85f;
            fsm = new FSM(fear);
        }
        else if (status == "disgust")
        {
            pleasant = -0.85f;
            aroused = -0.35f;
            fsm = new FSM(disgust);
        }
        else if (status == "sleepiness")
        {
            pleasant = 0.35f;
            aroused = -0.85f;
            fsm = new FSM(sleepiness);
        }
        else if (status == "calmness")
        {
            pleasant = 0.85f;
            aroused = -0.35f;
            fsm = new FSM(calmness);
        }
        else
        {
            Debug.Log("Hai inserito uno status errato! E' stato impostato quello neutrale!");
            fsm = new FSM(neutral);
        }
    }

    public float getPleasant()
    {
        return pleasant;
    }

    public void setPleasant(float value)
    {
        pleasant += value;
    }

    public float getAroused()
    {
        return aroused;
    }

    public void setAroused(float value)
    {
        aroused += value;
    }
}
