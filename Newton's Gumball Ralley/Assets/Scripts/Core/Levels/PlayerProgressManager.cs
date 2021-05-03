using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Levels
{
    public static class PlayerProgressManager
    {
        [System.Serializable]
        private struct LevelProgress
        {
            public int worldIndex;
            public int levelIndex;
            public int bestStarsEarned;
        }

        private static List<LevelProgress> levelProgresses = GetLevelProgresses();

        private static void Serialize(LevelProgress levelProgress)
        {
            int worldIndex = levelProgress.worldIndex;
            int levelIndex = levelProgress.levelIndex;
            string serializedLevelProgress = JsonUtility.ToJson(levelProgress, true);
            string writeFilePath = Application.persistentDataPath;
            writeFilePath = Path.Combine(
                writeFilePath,
                worldIndex.ToString() + "-" + levelIndex.ToString() + ".json"
            );
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
            if (Application.platform.Equals(RuntimePlatform.WebGLPlayer))
            {
                for (int worldIndex = 1; worldIndex <= 6; worldIndex++)
                {
                    for (int levelIndex = 1; levelIndex <= 6; levelIndex++)
                    {
                        LevelProgress levelProgress = new LevelProgress();
                        levelProgress.worldIndex = worldIndex;
                        levelProgress.levelIndex = levelIndex;
                        levelProgresses.Add(levelProgress);
                    }
                }
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*.json", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileInfos)
                {
                    LevelProgress levelProgress =
                        Deserialize(Path.Combine(Application.persistentDataPath, fileInfo.Name));
                    levelProgresses.Add(levelProgress);
                }
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
                        if (!Application.platform.Equals(RuntimePlatform.WebGLPlayer))
                        {
                            Serialize(levelProgresses[i]);
                        }
                        return;
                    }
                    return;
                }
            }
            if (!Application.platform.Equals(RuntimePlatform.WebGLPlayer))
            {
                LevelProgress levelProgress = new LevelProgress();
                levelProgress.worldIndex = currentWorldIndex;
                levelProgress.levelIndex = currentLevelIndex;
                levelProgress.bestStarsEarned = starsEarned;
                levelProgresses.Add(levelProgress);
                Serialize(levelProgress);
            }
        }

        public static int GetBestStarsEarnedForLevel(int worldIndex, int levelIndex)
        {
            return GetLevelProgress(worldIndex, levelIndex).bestStarsEarned;
        }
    }
}