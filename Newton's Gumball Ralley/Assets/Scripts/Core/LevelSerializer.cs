using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Background;

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
    public struct LevelData
    {
        public int worldIndex;
        public int levelIndex;
        public string customLevelName;
        public int repeatedBackgroundRows;
        public int repeatedBackgroundColumns;
        public List<SerializableTransform> environmentTransforms;
        public List<SerializablePreplacedObject> preplacedObjects;
        public SerializableTransform slingAnchorTransform;
    }

    public static class LevelSerializer
    {
        private static readonly string BACKGROUND_KEY = "Background";
        private static readonly string ENVIRONMENT_KEY = "Environment";
        private static readonly string PREPLACED_OBJECTS_KEY = "Preplaced Objects";
        private static readonly string SLING_ANCHOR_KEY = "Sling Anchor";
        private static readonly string WRITE_FILE_PATH_PREFIX = "Assets/LevelsData/";
        private static readonly string ENVIRONMENT_BLOCK_KEY = "EnvironmentBlock";

        public static void Serialize(int worldIndex, int levelIndex, string customLevelName)
        {
            LevelData levelData = GetLevelData(worldIndex, levelIndex, customLevelName);
            string serializedLevelData = JsonUtility.ToJson(levelData, true);
            string writeFilePath = WRITE_FILE_PATH_PREFIX;
            if (customLevelName.Equals(""))
            {
                writeFilePath += worldIndex.ToString() + "-" + levelIndex.ToString();
            }
            else
            {
                writeFilePath += customLevelName;
            }
            writeFilePath += ".json";
            using (StreamWriter streamWriter = new StreamWriter(writeFilePath))
            {
                streamWriter.Write(serializedLevelData);
            }
        }

        private static LevelData GetLevelData(int worldIndex, int levelIndex, string customLevelName)
        {
            LevelData levelData = new LevelData();
            levelData.worldIndex = worldIndex;
            levelData.levelIndex = levelIndex;
            levelData.customLevelName = customLevelName;
            List<GameObject> rootGameObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);
            foreach (GameObject gameObject in rootGameObjects)
            {
                if (gameObject.name.Equals(BACKGROUND_KEY))
                {
                    RepeatedBackgroundManager backgroundManager =
                        gameObject.GetComponentInChildren<RepeatedBackgroundManager>();
                    levelData.repeatedBackgroundColumns =
                        backgroundManager.desiredNumberOfColumns;
                    levelData.repeatedBackgroundRows =
                        backgroundManager.desiredNumberOfRows;
                }
                else if (gameObject.name.Equals(ENVIRONMENT_KEY))
                {
                    List<SerializableTransform> environmentTransforms =
                        new List<SerializableTransform>();
                    foreach (Transform environmentObject in gameObject.transform)
                    {
                        environmentTransforms.Add(SerializeTransform(environmentObject.transform));
                    }
                    levelData.environmentTransforms = environmentTransforms;
                }
                else if (gameObject.name.Equals(PREPLACED_OBJECTS_KEY))
                {
                    List<SerializablePreplacedObject> preplacedObjects =
                        new List<SerializablePreplacedObject>();
                    foreach (Transform preplacedObjectTransform in gameObject.transform)
                    {
                        SerializablePreplacedObject preplacedObject =
                            new SerializablePreplacedObject();
                        preplacedObject.objectName = preplacedObjectTransform
                            .GetComponent<PlacedObjectManager>()
                            .metaData.objectName;
                        preplacedObject.transform = SerializeTransform(preplacedObjectTransform);
                        preplacedObjects.Add(preplacedObject);
                    }
                    levelData.preplacedObjects = preplacedObjects;
                }
                else if (gameObject.name.Equals(SLING_ANCHOR_KEY))
                {
                    levelData.slingAnchorTransform = SerializeTransform(gameObject.transform);
                }
            }
            return levelData;
        }

        private static SerializableTransform SerializeTransform(Transform transform)
        {
            SerializableTransform serializedTransform = new SerializableTransform();
            serializedTransform.position = transform.position;
            serializedTransform.rotation = transform.rotation;
            serializedTransform.scale = transform.localScale;
            return serializedTransform;
        }

        public static LevelData Deserialize(string readFilePath)
        {
            string serializedLevelData = "";
            using (StreamReader streamReader = new StreamReader(readFilePath))
            {
                serializedLevelData = streamReader.ReadToEnd();
            }
            LevelData levelData = JsonUtility.FromJson<LevelData>(serializedLevelData);
            SetSceneWithLevelData(levelData);
            return levelData;
        }

        private static void SetSceneWithLevelData(LevelData levelData)
        {
            List<GameObject> rootGameObjects = new List<GameObject>();
            SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);
            foreach (GameObject gameObject in rootGameObjects)
            {
                if (gameObject.name.Equals(BACKGROUND_KEY))
                {
                    RepeatedBackgroundManager backgroundManager =
                        gameObject.GetComponentInChildren<RepeatedBackgroundManager>();
                    backgroundManager.desiredNumberOfColumns = levelData.repeatedBackgroundColumns;
                    backgroundManager.desiredNumberOfRows = levelData.repeatedBackgroundRows;
                }
                else if (gameObject.name.Equals(ENVIRONMENT_KEY))
                {
                    DeleteAllChildren(gameObject);
                    GameObject environmentBlockPrefab =
                        PlacedObjectPrefabDictionary.Get(ENVIRONMENT_BLOCK_KEY);
                    foreach (SerializableTransform transform in levelData.environmentTransforms)
                    {

                        GameObject environmentBlock = Object.Instantiate(
                            environmentBlockPrefab,
                            transform.position,
                            transform.rotation,
                            gameObject.transform
                        );
                        environmentBlock.transform.localScale = transform.scale;
                    }
                }
                else if (gameObject.name.Equals(PREPLACED_OBJECTS_KEY))
                {
                    DeleteAllChildren(gameObject);
                    foreach (
                        SerializablePreplacedObject serializedPreplacedObject
                        in levelData.preplacedObjects
                    )
                    {
                        GameObject preplacedObjectPrefab =
                            PlacedObjectPrefabDictionary.Get(serializedPreplacedObject.objectName);
                        GameObject preplacedObject = Object.Instantiate(
                            preplacedObjectPrefab,
                            serializedPreplacedObject.transform.position,
                            serializedPreplacedObject.transform.rotation,
                            gameObject.transform
                        );
                        preplacedObject.transform.localScale =
                            serializedPreplacedObject.transform.scale;
                    }
                }
                else if (gameObject.name.Equals(SLING_ANCHOR_KEY))
                {
                    gameObject.transform.position = levelData.slingAnchorTransform.position;
                    gameObject.transform.rotation = levelData.slingAnchorTransform.rotation;
                    gameObject.transform.localScale = levelData.slingAnchorTransform.scale;
                }
            }
        }

        private static void DeleteAllChildren(GameObject gameObject)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}
