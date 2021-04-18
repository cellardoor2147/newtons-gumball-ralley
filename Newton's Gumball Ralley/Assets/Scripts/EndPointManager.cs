using UnityEngine;
using Core;

public class EndPointManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otherObjectCollider)
    {
        if (otherObjectCollider.CompareTag("Player"))
        {
            GameStateManager.SetGameState(GameState.LevelCompleted);
        }
    }
}
