using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Background;
using System.Collections;

namespace Core
{
    [System.Serializable]
    public struct SerializableTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public struct SerializablePreplacedObject
    {
        public string objectName;
        public SerializableTransform transform;
    }

    [System.Serializable]
    public struct StarConditions
    {
        public float timeConstraint;
        public float scrapConstraint;
    }

    [System.Serializable]
    public struct LevelData
    {
        public int worldIndex;
        public int levelIndex;
        public string customLevelName;
        public int repeatedBackgroundRows;
        public int repeatedBackgroundColumns;
        public List<SerializablePreplacedObject> environmentObjects;
        public List<SerializablePreplacedObject> preplacedObjects;
        public SerializableTransform gumballMachineTransform;
        public StarConditions starConditions;
    }

    public static class LevelSerializer
    {
        public static readonly string WRITE_DIRECTORY_PATH =
            Application.dataPath + "/LevelsData/";

        private static readonly string MANAGERS_KEY = "Managers";
        private static readonly string BACKGROUND_MANAGER_KEY = "Background Manager";
        private static readonly string ENVIRONMENT_KEY = "Environment";
        private static readonly string PREPLACED_OBJECTS_KEY = "Preplaced Objects";
        private static readonly string GUMBALL_MACHINE_KEY = "Gumball Machine";
        private static readonly string GAME_SCENE_KEY = "Game";

        public static void Serialize(
            int worldIndex,
            int levelIndex,
            string customLevelName, 
            float timeConstraint,
            float scrapConstraint,
            int repeatedBackgroundColumns,
            int repeatedBackgroundRows
        )
        {
            LevelData levelData = GetLevelData(
                worldIndex,
                levelIndex,
                customLevelName
            );
            levelData.repeatedBackgroundColumns = repeatedBackgroundColumns;
            levelData.repeatedBackgroundRows = repeatedBackgroundRows;
            levelData.starConditions.timeConstraint = timeConstraint;
            levelData.starConditions.scrapConstraint = scrapConstraint;
            string serializedLevelData = JsonUtility.ToJson(levelData, true);
            string writeFilePath = WRITE_DIRECTORY_PATH;
            if (customLevelName.Equals(""))
            {
                writeFilePath += worldIndex.ToString() + "-" + levelIndex.ToString();
            }
            else
            {
                writeFilePath += customLevelName;
            }
            writeFilePath += ".json";
            Directory.CreateDirectory(WRITE_DIRECTORY_PATH);
            using (StreamWriter streamWriter = new StreamWriter(writeFilePath))
            {
                streamWriter.Write(serializedLevelData);
            }
        }

        private static LevelData GetLevelData(
            int worldIndex,
            int levelIndex,
            string customLevelName
        )
        {
            LevelData levelData = new LevelData();
            levelData.worldIndex = worldIndex;
            levelData.levelIndex = levelIndex;
            levelData.customLevelName = customLevelName;
            List<GameObject> rootGameObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);
            foreach (GameObject gameObject in rootGameObjects)
            {
                if (gameObject.name.Equals(ENVIRONMENT_KEY))
                {
                    List<SerializablePreplacedObject> environmentObjects = GetObjectsInContainer(gameObject);
                    levelData.environmentObjects = environmentObjects;
                }
                else if (gameObject.name.Equals(PREPLACED_OBJECTS_KEY))
                {
                    List<SerializablePreplacedObject> preplacedObjects = GetObjectsInContainer(gameObject);
                    levelData.preplacedObjects = preplacedObjects;
                }
                else if (gameObject.name.Equals(GUMBALL_MACHINE_KEY))
                {
                    levelData.gumballMachineTransform = SerializeTransform(gameObject.transform);
                }
            }
            return levelData;
        }

        private static List<SerializablePreplacedObject> GetObjectsInContainer(GameObject container)
        {
            List<SerializablePreplacedObject> preplacedObjects = new List<SerializablePreplacedObject>();
            foreach (Transform objectTransform in container.transform)
            {
                SerializablePreplacedObject preplacedObject = new SerializablePreplacedObject();
                preplacedObject.objectName = objectTransform
                    .GetComponent<PlacedObjectManager>()
                    .metaData.objectName;
                preplacedObject.transform = SerializeTransform(objectTransform);
                preplacedObjects.Add(preplacedObject);
            }

            return preplacedObjects;
        }

        private static SerializableTransform SerializeTransform(Transform transform)
        {
            SerializableTransform serializedTransform = new SerializableTransform();
            serializedTransform.position = transform.position;
            serializedTransform.rotation = transform.rotation;
            serializedTransform.scale = transform.localScale;
            return serializedTransform;
        }

        public static LevelData DeserializeFromReadFilePath(string readFilePath)
        {
            string serializedLevelData = "";
            using (StreamReader streamReader = new StreamReader(readFilePath))
            {
                serializedLevelData = streamReader.ReadToEnd();
            }
            return JsonUtility.FromJson<LevelData>(serializedLevelData);
        }

        public static LevelData DeserializeFromTextAsset(TextAsset textAsset)
        {
            return JsonUtility.FromJson<LevelData>(textAsset.text);
        }

        public static void SetSceneWithLevelData(LevelData levelData)
        {
            if (Application.isPlaying)
            {
                RepeatedBackgroundManager.SetDesiredNumberOfColumnsAndRows(
                    levelData.repeatedBackgroundColumns,
                    levelData.repeatedBackgroundRows
                );
            }
            List<GameObject> rootGameObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);
            foreach (GameObject gameObject in rootGameObjects)
            {
                if (gameObject.name.Equals(ENVIRONMENT_KEY))
                {
                    PopulateContainer(gameObject, levelData.environmentObjects);
                }
                else if (gameObject.name.Equals(PREPLACED_OBJECTS_KEY))
                {
                    PopulateContainer(gameObject, levelData.preplacedObjects);
                }
                else if (gameObject.name.Equals(GUMBALL_MACHINE_KEY))
                {
                    gameObject.transform.position = levelData.gumballMachineTransform.position;
                    gameObject.transform.rotation = levelData.gumballMachineTransform.rotation;
                    gameObject.transform.localScale = levelData.gumballMachineTransform.scale;
                }
            }
        }

        private static void PopulateContainer(GameObject container, List<SerializablePreplacedObject> placedObjects)
        {
            DeleteAllChildren(container);
            foreach (SerializablePreplacedObject serializedPreplacedObject in placedObjects)
            {
                GameObject preplacedObjectPrefab =
                    PlacedObjectPrefabDictionary.Get(serializedPreplacedObject.objectName);
                GameObject preplacedObject = Object.Instantiate(
                    preplacedObjectPrefab,
                    serializedPreplacedObject.transform.position,
                    serializedPreplacedObject.transform.rotation,
                    container.transform
                );
                preplacedObject.transform.localScale =
                    serializedPreplacedObject.transform.scale;
            }
        }

        private static void DeleteAllChildren(GameObject gameObject)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public static IEnumerator AsyncSetSceneWithLevelData(LevelData levelData)
        {
            yield return new WaitUntil(() => GameSceneIsLoaded());
            SetSceneWithLevelData(levelData);
        }

        private static bool GameSceneIsLoaded()
        {
            return SceneManager.GetActiveScene().name.Equals(GAME_SCENE_KEY) &&
                SceneManager.GetActiveScene().isLoaded;
        }
    }
}
