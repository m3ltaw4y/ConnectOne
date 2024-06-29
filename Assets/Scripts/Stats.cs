using System;
using TMPro;
using UnityEngine;

[Serializable]
public class Stats
{
    [SerializeField] TextMeshProUGUI text;
    int wins, losses;

    public void Init()
    {
        wins = PlayerPrefs.GetInt(nameof(wins), 0);
        losses = PlayerPrefs.GetInt(nameof(losses), 0);
    }
    
    public void OnWin()
    {
        wins++;
        PlayerPrefs.SetInt(nameof(wins), wins);
    }

    public void OnLose()
    {
        losses++;
        PlayerPrefs.SetInt(nameof(losses), losses);
    }

    public void LifeTimeStats()
    {
        text.text = $"Wins/Played = {wins}/{wins+losses}\n{(wins/(float)(wins+losses))*100f:0.##}%";
        PlayerPrefs.Save();
    }
}