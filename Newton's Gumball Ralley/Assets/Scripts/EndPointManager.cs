using UnityEngine;
using Core;
using LevelTimer;

public class EndPointManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otherObjectCollider)
    {
        if (otherObjectCollider.CompareTag("Player")
            && GameStateManager.GetGameState().Equals(GameState.Playing))
        {
            Timer.StopTimer();
            GameStateManager.SetGameState(GameState.LevelCompleted);
        }
    }
}
