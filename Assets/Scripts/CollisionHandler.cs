using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    void OnCollisionEnter2D(Collision2D _)
    {
        gameController.EndGame();
    }
}