using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{
    private string text;
    private int[] human_status;
    private int[] ai_status;

    public Answer(string text, int[] human_status, int[] ai_status)
    {
        this.text = text;
        this.human_status = human_status;
        this.ai_status = ai_status;
    }

    public string getText()
    {
        return text;
    }

    public int[] getHumanStatus()
    {
        return human_status;
    }

    public int[] getAiStatus()
    {
        return ai_status;
    }

    public void setText(string text)
    {
        this.text = text;
    }

    public void setHumanStatus(int[] human_status)
    {
        this.human_status = human_status;
    }

    public void setAiStatus(int[] ai_status)
    {
        this.ai_status = ai_status;
    }
}