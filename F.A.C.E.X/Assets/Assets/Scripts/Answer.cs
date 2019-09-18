using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{
    private string text;
    private string human_status;
    private string ai_status;

    public Answer(string text, string human_status, string ai_status)
    {
        this.text = text;
        this.human_status = human_status;
        this.ai_status = ai_status;
    }

    public string getText()
    {
        return text;
    }

    public string getHumanStatus()
    {
        return human_status;
    }

    public string getAiStatus()
    {
        return ai_status;
    }

    public void setText(string text)
    {
        this.text = text;
    }

    public void setHumanStatus(string human_status)
    {
        this.human_status = human_status;
    }

    public void setAiStatus(string ai_status)
    {
        this.ai_status = ai_status;
    }
}