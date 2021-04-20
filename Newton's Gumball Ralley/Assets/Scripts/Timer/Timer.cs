using Core;
using Ball;
using UnityEngine;

namespace LevelTimer
{
    public class Timer : MonoBehaviour
    {
        public float CurrentTime { get; private set; } = 0f;
        public static float CompletionTime { get; private set; }
        private bool completionTimeIsSet = false;
        private BallMovement ball;

        private void Awake()
        {
            GameObject ballObject = GameObject.FindGameObjectWithTag("Player");
            if (ballObject)
                ball = ballObject.GetComponent<BallMovement>();
        }

        void Update()
        {
            if (ball != null 
                && ball.HasBeenReleased
                && !GameStateManager.GetGameState().Equals(GameState.LevelCompleted))
            {
                CurrentTime += Time.deltaTime;
            }
            else
            {
                if (GameStateManager.GetGameState().Equals(GameState.LevelCompleted)
                    && !completionTimeIsSet)
                {
                    CompletionTime = CurrentTime;
                    completionTimeIsSet = true;
                }
                CurrentTime = 0f;
            }
        }
    }
}
