﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        private static readonly string LEVELS_DATA_KEY = "/LevelsData";

        private static LevelManager instance;

        private List<LevelData> levelsData;
        private int levelsDataIndex;

        private LevelManager() { } // Prevents instantiation outside of this class

        private void Awake()
        {
            SetInstance();
            levelsData = GetLevelsData();
            levelsDataIndex = 0;
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

        private List<LevelData> GetLevelsData()
        {
            List<LevelData> levelsData = new List<LevelData>();
            DirectoryInfo directoryInfo =
                new DirectoryInfo(Application.dataPath + LEVELS_DATA_KEY);
            FileInfo[] filesInfo = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in filesInfo) 
            {
                LevelData levelData = LevelSerializer.Deserialize(
                    Application.dataPath + LEVELS_DATA_KEY + "/" + fileInfo.Name
                );
                levelsData.Add(levelData);
            }
            return levelsData;
        }

        public static void LoadLevel(int worldIndex, int levelIndex)
        {
            for (int i = 0; i < instance.levelsData.Count; i++)
            {
                LevelData levelData = instance.levelsData[i];
                bool levelDataIsTheLevelToLoad =
                    levelData.worldIndex == worldIndex && levelData.levelIndex == levelIndex;
                if (levelDataIsTheLevelToLoad)
                {
                    LoadLevelWithLevelData(levelData);
                    instance.levelsDataIndex = i;
                    break;
                }
            }
        }

        public static void LoadNextLevel()
        {
            LoadLevelByIndex(instance.levelsDataIndex + 1);
        }

        public static void LoadLevelByIndex(int index)
        {
            if (index >= instance.levelsData.Count)
            {
                // TODO: load a credits sequence or whatever ends up happening
                // once the game concludes
                GameStateManager.SetGameState(GameState.MainMenu);
                return;
            }
            LoadLevelWithLevelData(instance.levelsData[index]);
            instance.levelsDataIndex = index;
        }

        private static void LoadLevelWithLevelData(LevelData levelData)
        {
            GameStateManager.SetGameState(GameState.Editing);
            instance.StartCoroutine(LevelSerializer.AsyncSetSceneWithLevelData(levelData));
        }
    }
}