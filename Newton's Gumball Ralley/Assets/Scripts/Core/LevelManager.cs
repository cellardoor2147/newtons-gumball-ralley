using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Core
{
    public static class LevelManager
    {
        private static readonly List<LevelData> levelsData = GetLevelsData();

        private static LevelData currentLevelData;

        private static List<LevelData> GetLevelsData()
        {
            List<LevelData> levelsData = new List<LevelData>();
            DirectoryInfo directoryInfo =
                new DirectoryInfo(LevelSerializer.WRITE_DIRECTORY_PATH);
            FileInfo[] filesInfo = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in filesInfo) 
            {
                LevelData levelData = LevelSerializer.Deserialize(
                    LevelSerializer.WRITE_DIRECTORY_PATH + fileInfo.Name
                );
                levelsData.Add(levelData);
            }
            return levelsData;
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
