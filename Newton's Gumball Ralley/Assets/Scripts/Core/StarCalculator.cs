using LevelTimer;
using GUI.EditMode;
using UnityEngine;
using Core.Levels;

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
                if (PlayerGotTimeStar() && LevelManager.CurrentLevelShouldUseTimeConstraint())
                {
                    starsEarned++;
                }
                if (PlayerGotScrapStar() && LevelManager.CurrentLevelShouldUseScrapConstraint())
                {
                    starsEarned++;
                }
            }

            return starsEarned;
        }

        public static bool PlayerGotTimeStar()
        {
            return Timer.CompletionTime < LevelManager.GetCurrentLevelTimeConstraint();
        }

        public static bool PlayerGotScrapStar()
        {
            return ScrapManager.ScrapRemaining >= LevelManager.GetCurrentLevelScrapConstraint();
        }
    }
}