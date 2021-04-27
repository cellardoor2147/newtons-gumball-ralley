using LevelTimer;
using GUI.EditMode;
using UnityEngine;

namespace Core
{
    public class StarCalculator : MonoBehaviour
    {
        public static int GetStarsEarned()
        {
            int starsEarned = 0;
            if (GameStateManager.GetGameState().Equals(GameState.LevelCompleted))
            {
                starsEarned = 1;
                if (Timer.CompletionTime < LevelManager.GetCurrentLevelTimeConstraint()
                    || ScrapManager.ScrapRemaining >= LevelManager.GetCurrentLevelScrapConstraint())
                {
                    starsEarned = 2;
                }
                if (Timer.CompletionTime < LevelManager.GetCurrentLevelTimeConstraint()
                    && ScrapManager.ScrapRemaining >= LevelManager.GetCurrentLevelScrapConstraint())
                {
                    starsEarned = 3;
                }
            }

            return starsEarned;
        }
    }
}