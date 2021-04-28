using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GUI.EditMode;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<TextAsset> serializedLevelsData;

        private static List<LevelData> levelsData = new List<LevelData>();
        private static LevelData currentLevelData;
        private static LevelManager instance;
        
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
            bool applicationIsNotRunning = Application.isEditor && !Application.isPlaying;
            if (applicationIsNotRunning)
            {
                LevelSerializer.SetSceneWithLevelData(levelData);
            }
            else
            {
                GameStateManager.SetGameState(GameState.Editing);
                GameStateManager.StartStaticCoroutine(
                    LevelSerializer.AsyncSetSceneWithLevelData(levelData)
                );
                ScrapManager.ResetRemainingScrap();
                GameStateManager.StartStaticCoroutine(EditModeManager.AsyncSetActiveTab(PlaceableObjectType.InclinePlane));
                GameStateManager.StartStaticCoroutine(EditModeManager.AsyncToggleButtonsBasedOnCurrentLevel());
                GameStateManager.StartStaticCoroutine(EditModeManager.DisableFutureTabs());
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

        public static float GetCurrentLevelTimeConstraint()
        {
            return currentLevelData.starConditions.timeConstraint;
        }

        public static float GetCurrentLevelScrapConstraint()
        {
            return currentLevelData.starConditions.scrapConstraint;
        }

        public static float GetCurrentLevelScrapAllotted()
        {
            return currentLevelData.placeableScrapLimit;
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
    }
}
