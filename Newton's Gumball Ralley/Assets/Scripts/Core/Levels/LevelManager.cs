using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GUI.EditMode;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Core.Levels
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<TextAsset> serializedLevelsData;

        private static List<LevelData> levelsData = new List<LevelData>();
        private static LevelData currentLevelData;
        private static LevelManager instance;
        private static bool currentLevelIsComplete;

        private void Awake()
        {
            SetInstance();
            StartCoroutine(AsyncSetLevelsData());
        }

        private void SetInstance()
        {
            if (instance != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private static IEnumerator AsyncSetLevelsData()
        {
            yield return new WaitUntil(() => instance != null);
            SetLevelsData();
        }

        private static void SetLevelsData()
        {
            foreach (TextAsset serializedLevelData in instance.serializedLevelsData) 
            {
                LevelData levelData =
                    LevelSerializer.DeserializeFromTextAsset(serializedLevelData);
                levelsData.Add(levelData);
            }
            levelsData.Sort(delegate(LevelData level1, LevelData level2)
            {
                int worldComparison = level1.worldIndex.CompareTo(level2.worldIndex);
                bool levelsHaveSameWorld = worldComparison == 0;
                if (levelsHaveSameWorld)
                {
                    return level1.levelIndex.CompareTo(level2.levelIndex);
                }
                return worldComparison;
            });
        }

        public static void LoadLevelWithIndices(int worldIndex, int levelIndex)
        {
            foreach (LevelData levelData in levelsData)
            {
                bool levelDataIsTheLevelToLoad =
                    levelData.worldIndex == worldIndex && levelData.levelIndex == levelIndex;
                if (levelDataIsTheLevelToLoad)
                {
                    LoadLevelWithLevelData(levelData);
                    break;
                }
            }
        }

        public static void LoadNextLevel()
        {
            for (int i = 0; i < levelsData.Count - 1; i++)
            {
                bool levelDataIsCurrentLevelData =
                    levelsData[i].worldIndex == currentLevelData.worldIndex
                    && levelsData[i].levelIndex == currentLevelData.levelIndex;
                if (levelDataIsCurrentLevelData)
                {
                    LoadLevelWithLevelData(levelsData[i + 1]);
                    return;
                }
            }
            // Couldn't find a next level, so load the main menu
            // (TODO: replaced with credits sequence)
            GameStateManager.SetGameState(GameState.MainMenu);
        }

        public static void LoadLevelWithLevelData(LevelData levelData)
        {
            currentLevelData = levelData;
            currentLevelIsComplete = false;
            bool applicationIsNotRunning = Application.isEditor && !Application.isPlaying;
            if (applicationIsNotRunning)
            {
                LevelSerializer.SetSceneWithLevelData(levelData);
            }
            else
            {
                GameStateManager.SetGameState(GameState.Dialogue);
                GameStateManager.StartStaticCoroutine(
                    LevelSerializer.AsyncSetSceneWithLevelData(levelData)
                );
                ScrapManager.ResetRemainingScrap();
                switch (currentLevelData.worldIndex)
                {
                    case 1:
                        GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.InclinePlane));
                        break;
                    case 2:
                        GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.Screw));
                        break;
                    case 3:
                        GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.Lever));
                        break;
                    case 4:
                        GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.Wedge));
                        break;
                    case 5:
                        GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.Wheel));
                        break;
                    case 6:
                        GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.InclinePlane));
                        break;
                }               
                GameStateManager.StartStaticCoroutine(EditModeManager.AsyncToggleButtonsBasedOnCurrentLevel());
                GameStateManager.StartStaticCoroutine(EditModeManager.DisableTabs());
                GameStateManager.StartStaticCoroutine(EditModeManager.ToggleHintButton());
            }

        }

        public static int GetCurrentWorldIndex()
        {
            return currentLevelData.worldIndex;
        }

        public static int GetCurrentLevelIndex()
        {
            return currentLevelData.levelIndex;
        }

        public static string GetCurrentLevelHintText()
        {
            if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
            {
                currentLevelData.hintText = currentLevelData.englishHintText;
            }
            else
            {
                currentLevelData.hintText = currentLevelData.spanishHintText;
            }
            return currentLevelData.hintText;
        }

        public static bool CurrentLevelShouldHaveHint()
        {
            return currentLevelData.shouldHaveHint;
        }

        public static bool CurrentLevelShouldUseTimeConstraint()
        {
            return currentLevelData.starConditions.shouldUseTimeConstraint;
        }

        public static float GetCurrentLevelTimeConstraint()
        {
            return currentLevelData.starConditions.timeConstraint;
        }
        public static bool CurrentLevelShouldUseScrapConstraint()
        {
            return currentLevelData.starConditions.shouldUseScrapConstraint;
        }

        public static float GetCurrentLevelScrapConstraint()
        {
            return currentLevelData.starConditions.scrapConstraint;
        }

        public static float GetCurrentLevelScrapAllotted()
        {
            return currentLevelData.placeableScrapLimit;
        }

        public static int GetLevelNumberOfEarnableStars(int worldIndex, int levelIndex)
        {
            int numberOfEarnableStars = 1;
            LevelData levelData = levelsData.Find(
                currLevelData => currLevelData.worldIndex == worldIndex
                                 && currLevelData.levelIndex == levelIndex
            );
            if (levelData.starConditions.shouldUseTimeConstraint)
            {
                numberOfEarnableStars++;
            }
            if (levelData.starConditions.shouldUseScrapConstraint)
            {
                numberOfEarnableStars++;
            }
            return numberOfEarnableStars;
        }
        
        public static Vector3 GetCurrentLevelGumballMachinePosition()
        {
            return currentLevelData.gumballMachineTransform.position;
        }

        public static Quaternion GetCurrentLevelGumballMachineRotation()
        {
            return currentLevelData.gumballMachineTransform.rotation;
        }

        public static Vector3 GetCurrentLevelGumballMachineScale()
        {
            return currentLevelData.gumballMachineTransform.scale;
        }

        public static bool GetCurrentLevelIsComplete()
        {
            return currentLevelIsComplete;
        }

        public static void SetCurrentLevelIsComplete(bool isComplete)
        {
            currentLevelIsComplete = isComplete;
        }

        public static bool CurrentLevelShouldHideAllScrapAndTimeGUI()
        {
            return GetCurrentWorldIndex() == 1
                && GetCurrentLevelIndex() < 3;
        }

        public static bool CurrentLevelShouldHideStarConstraints()
        {
            return !(CurrentLevelShouldUseScrapConstraint()
                || CurrentLevelShouldUseTimeConstraint());
        }

        public static int GetCurrentLevelNumberOfDesiredBackgroundColumns()
        {
            return currentLevelData.repeatedBackgroundColumns;
        }
        public static int GetCurrentLevelNumberOfDesiredBackgroundRows()
        {
            return currentLevelData.repeatedBackgroundRows;
        }
    }
}
