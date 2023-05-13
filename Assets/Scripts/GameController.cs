using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button holeButton;
    [SerializeField] private Rigidbody2D piece;
    [SerializeField] private Button playButton;
    [SerializeField] private ParticleSystem particles;

    public bool IsYellow { get; set; }
    private Vector3 initialPos;

    private void Awake()
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

    private IEnumerator DoYellowMove()
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
            text.text = "You Win!";
            particles.Play();
        }
        else
        {
            text.text = "You Lose!";
        }
    }
}
