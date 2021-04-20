using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Core
{
    public static class PlayerProgressManager
    {
        public static readonly string WRITE_DIRECTORY_PATH =
            Application.persistentDataPath + '/';

        [System.Serializable]
        private struct LevelProgress
        {
            public int worldIndex;
            public int levelIndex;
            public int bestStarsEarned;
        }

        private static List<LevelProgress> levelProgresses = GetLevelProgresses();

        private static void Serialize(int worldIndex, int levelIndex)
        {
            LevelProgress levelProgress = GetLevelProgress(worldIndex, levelIndex);
            string serializedLevelProgress = JsonUtility.ToJson(levelProgress, true);
            string writeFilePath = WRITE_DIRECTORY_PATH;
            writeFilePath += worldIndex.ToString() + "-" + levelIndex.ToString() + ".json";
            Directory.CreateDirectory(WRITE_DIRECTORY_PATH);
            using (StreamWriter streamWriter = new StreamWriter(writeFilePath))
            {
                streamWriter.Write(serializedLevelProgress);
            }
        }

        private static LevelProgress Deserialize(string readFilePath)
        {
            string serializedLevelProgress = "";
            using (StreamReader streamReader = new StreamReader(readFilePath))
            {
                serializedLevelProgress = streamReader.ReadToEnd();
            }
            LevelProgress levelProgress = JsonUtility.FromJson<LevelProgress>(serializedLevelProgress);
            return levelProgress;
        }

        private static LevelProgress GetLevelProgress(int worldIndex, int levelIndex)
        {
            for (int i = 0; i < levelProgresses.Count; i++)
            {
                bool foundDesiredLevelProgress =
                    levelProgresses[i].worldIndex == worldIndex
                    && levelProgresses[i].levelIndex == levelIndex;
                if (foundDesiredLevelProgress)
                {
                    return levelProgresses[i];
                }
            }

            return new LevelProgress();
        }

        private static List<LevelProgress> GetLevelProgresses()
        {
            List<LevelProgress> levelProgresses = new List<LevelProgress>();
            DirectoryInfo directoryInfo = new DirectoryInfo(WRITE_DIRECTORY_PATH);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in fileInfos)
            {
                LevelProgress levelProgress = Deserialize(WRITE_DIRECTORY_PATH + fileInfo.Name);
                levelProgresses.Add(levelProgress);
            }

            return levelProgresses;
        }

        public static void RecordProgressForCurrentLevel()
        {
            int currentWorldIndex = LevelManager.GetCurrentWorldIndex();
            int currentLevelIndex = LevelManager.GetCurrentLevelIndex();
            int starsEarned = StarCalculator.GetStarsEarned();    

            for (int i = 0; i < levelProgresses.Count; i++)
            {
                if (levelProgresses[i].worldIndex == currentWorldIndex 
                    && levelProgresses[i].levelIndex == currentLevelIndex)
                {
                    if (starsEarned > levelProgresses[i].bestStarsEarned)
                    {
                        LevelProgress newLevelProgress = new LevelProgress();
                        newLevelProgress.worldIndex = currentWorldIndex;
                        newLevelProgress.levelIndex = currentLevelIndex;
                        newLevelProgress.bestStarsEarned = starsEarned;
                        levelProgresses[i] = newLevelProgress;
                        Serialize(currentWorldIndex, currentLevelIndex);
                        return;
                    }
                    return;
                }
            }
            LevelProgress levelProgress = new LevelProgress();
            levelProgress.worldIndex = currentWorldIndex;
            levelProgress.levelIndex = currentLevelIndex;
            levelProgress.bestStarsEarned = starsEarned;
            levelProgresses.Add(levelProgress);
            Serialize(currentWorldIndex, currentLevelIndex);
        }
    }
}