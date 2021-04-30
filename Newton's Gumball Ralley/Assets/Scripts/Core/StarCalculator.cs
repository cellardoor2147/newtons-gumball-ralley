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
                if (PlayerGotTimeStar())
                {
                    starsEarned++;
                }
                if (PlayerGotScrapStar())
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