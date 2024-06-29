using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] GameController gameController;
    void OnCollisionEnter2D(Collision2D _)
    {
        gameController.EndGame();
    }
}