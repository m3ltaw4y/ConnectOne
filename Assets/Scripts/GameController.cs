using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button holeButton;
    [SerializeField] Rigidbody2D piece;
    [SerializeField] Button playButton;
    [SerializeField] ParticleSystem particles;
    [SerializeField] Stats stats;
 
    public bool IsYellow { get; set; }
    Vector3 initialPos;

    void Awake()
    {
        initialPos = piece.transform.position;
    }

    public void OnClick()
    {
        particles.Stop();
        piece.transform.position = initialPos; 
        IsYellow = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        if (IsYellow)
        {
            text.text = "You are Yellow.\nYour turn";
            holeButton.gameObject.SetActive(true);
        }
        else
        {
            text.text = "You are Red.\nYellow's turn";
            StartCoroutine(DoYellowMove());
        }
    }

    IEnumerator DoYellowMove()
    {
        yield return new WaitForSeconds(2);
        piece.simulated = true;
    }

    public void EndGame()
    {
        playButton.gameObject.SetActive(true);
        piece.simulated = false;
        if (IsYellow)
        {            
            particles.Play();
            stats.OnWin();
            text.text = "You Win!";
        }
        else
        {
            stats.OnLose();
            text.text = "You Lose!";
        }
        stats.LifeTimeStats();
    }
}
