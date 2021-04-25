using LevelTimer;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class StarCalculator : MonoBehaviour
    {
        public static float AmountOfScrapUsed { get; private set; } = 0;

        private void Update()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                AmountOfScrapUsed = GetAmountOfScrapUsed();
            }
        }

        public static int GetStarsEarned()
        {
            int starsEarned = 0;
            if (GameStateManager.GetGameState().Equals(GameState.LevelCompleted))
            {
                starsEarned = 1;
                if (Timer.CompletionTime < LevelManager.GetCurrentLevelTimeConstraint()
                    || AmountOfScrapUsed <= LevelManager.GetCurrentLevelScrapConstraint())
                {
                    starsEarned = 2;
                }
                if (Timer.CompletionTime < LevelManager.GetCurrentLevelTimeConstraint()
                    && AmountOfScrapUsed <= LevelManager.GetCurrentLevelScrapConstraint())
                {
                    starsEarned = 3;
                }
            }

            return starsEarned;
        }

        private float GetAmountOfScrapUsed()
        {
            float amountOfScrapUsed = 0;
            List<PlacedObjectManager> placedObjects = 
                GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList();
            foreach (PlacedObjectManager placedObject in placedObjects)
            {
                if (placedObject.metaData.isSimpleMachine)
                    amountOfScrapUsed += placedObject.metaData.amountOfScrap;
            }
            return amountOfScrapUsed;
        }
    }
}