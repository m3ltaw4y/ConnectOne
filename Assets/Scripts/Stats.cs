using System;
using TMPro;
using UnityEngine;

[Serializable]
public class Stats
{
    [SerializeField] TextMeshProUGUI text;
    int wins, losses;
    
    public void OnWin()
    {
        wins++;
    }

    public void OnLose()
    {
        losses++;
    }

    public void LifeTimeStats()
    {
        text.text = $"Wins/Played = {wins}/{wins+losses}\n{(wins/(float)(wins+losses))*100f:0.##}%";
    }
}