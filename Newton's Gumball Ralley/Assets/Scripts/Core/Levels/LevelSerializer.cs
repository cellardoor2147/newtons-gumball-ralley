using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Core.PlacedObjects;
using SimpleMachine;
using Hints;

namespace Core.Levels
{
    [System.Serializable]
    public struct SerializableTransform
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public struct SerializablePulleyState
    {
        public PlatformState platformState;
        public PlatformPosition platformPosition;
        public BallRollDirection ballRollDirection;
    }

    [System.Serializable]
    public struct SerializablePreplacedObject
    {
        public string objectName;
        public SerializableTransform transform;
        public SerializablePulleyState pulleyState;
    }

    [System.Serializable]
    public struct SerializableHint
    {
        public string objectName;
        public string text;
        public float characterSize;
        public int fontSize;
        public bool isArrow;
        public SerializableTransform textTransform;
        public float minArrowScale;
        public float maxArrowScale;
        public SerializableTransform arrowTransform;
        public bool outlineIsFlipped;
        public string outlineHintName;
        public int outlineHintOrder;
    }

    [System.Serializable]
    public struct StarConditions
    {
        public bool shouldUseTimeConstraint;
        public float timeConstraint;
        public bool shouldUseScrapConstraint;
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
        public List<SerializableHint> hints;
        public SerializableTransform gumballMachineTransform;
        public float placeableScrapLimit;
        public StarConditions starConditions;
    }

    public static class LevelSerializer
    {
        public static readonly string WRITE_DIRECTORY_PATH =
            Path.Combine(Application.dataPath, "LevelsData");

        private static readonly string ENVIRONMENT_KEY = "Environment";
        private static readonly string PREPLACED_OBJECTS_KEY = "Preplaced Objects";
        private static readonly string HINTS_KEY = "Hints";
        private static readonly string HINT_ARROW_KEY = "Hint Arrow";
        private static readonly string GUMBALL_MACHINE_KEY = "Gumball Machine";
        private static readonly string GAME_SCENE_KEY = "Game";

        public static void Serialize(
            int worldIndex,
            int levelIndex,
            string customLevelName,
            bool shouldUseTimeConstraint,
            float timeConstraint,
            bool shouldUseScrapConstraint,
            float scrapConstraint,
            int repeatedBackgroundColumns,
            int repeatedBackgroundRows,
            float placeableScrapLimit
        )
        {
            LevelData levelData = GetLevelData(
                worldIndex,
                levelIndex,
                customLevelName
            );
            levelData.repeatedBackgroundColumns = repeatedBackgroundColumns;
            levelData.repeatedBackgroundRows = repeatedBackgroundRows;
            levelData.starConditions.shouldUseTimeConstraint = shouldUseTimeConstraint;
            levelData.starConditions.timeConstraint = timeConstraint;
            levelData.starConditions.shouldUseScrapConstraint = shouldUseScrapConstraint;
            levelData.starConditions.scrapConstraint = scrapConstraint;
            levelData.placeableScrapLimit = placeableScrapLimit;
            string serializedLevelData = JsonUtility.ToJson(levelData, true);
            string writeFilePath = WRITE_DIRECTORY_PATH;
            if (customLevelName.Equals(""))
            {
                writeFilePath = Path.Combine(
                    writeFilePath,
                    worldIndex.ToString() + "-" + levelIndex.ToString()
                );
            }
            else
            {
                writeFilePath = Path.Combine(writeFilePath, customLevelName);
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
                else if (gameObject.name.Equals(HINTS_KEY))
                {
                    List<SerializableHint> hints = GetHints(gameObject);
                    levelData.hints = hints;
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
                preplacedObject.pulleyState = GetPulleyState(objectTransform);
                preplacedObjects.Add(preplacedObject);
            }

            return preplacedObjects;
        }

        private static List<SerializableHint> GetHints(GameObject hintsContainer)
        {
            List<SerializableHint> hints = new List<SerializableHint>();
            foreach (Transform hintTransform in hintsContainer.transform)
            {
                SerializableHint hint = new SerializableHint();
                TextMesh textMesh = hintTransform.GetComponent<TextMesh>();
                bool isArrow = hintTransform.GetComponent<TextMesh>() != null;
                hint.isArrow = isArrow;
                hint.objectName = hintTransform.GetComponent<PlacedObjectManager>().metaData.objectName;
                hint.textTransform = SerializeTransform(hintTransform);
                if (isArrow)
                {
                    hint.text = textMesh.text;
                    hint.characterSize = textMesh.characterSize;
                    hint.fontSize = textMesh.fontSize;
                    Transform hintArrowTransform = hintTransform.Find(HINT_ARROW_KEY).transform;
                    HintArrowEffectsManager hintEffectsManager =
                        hintArrowTransform.GetComponent<HintArrowEffectsManager>();
                    hint.minArrowScale = hintEffectsManager.minScale;
                    hint.maxArrowScale = hintEffectsManager.maxScale;
                    hint.arrowTransform = SerializeTransform(hintArrowTransform);
                }
                else
                {
                    SpriteRenderer spriteRenderer = hintTransform.GetComponent<SpriteRenderer>();
                    hint.outlineIsFlipped = spriteRenderer.flipX; 
                    hint.outlineHintName = spriteRenderer.sprite.name;
                    hint.outlineHintOrder = hintTransform.GetComponent<OutlineHintOrderTracker>().order;
                }
                hints.Add(hint);
            }
            return hints;
        }

        private static SerializableTransform SerializeTransform(Transform transform)
        {
            SerializableTransform serializedTransform = new SerializableTransform();
            serializedTransform.position = transform.position;
            serializedTransform.rotation = transform.rotation;
            serializedTransform.scale = transform.localScale;
            return serializedTransform;
        }

        private static SerializablePulleyState GetPulleyState(Transform transform)
        {
            SerializablePulleyState serializablePulleyState = new SerializablePulleyState();
            if (transform.GetComponent<PulleyBehavior>() != null)
            {
                serializablePulleyState.platformState = transform.GetComponent<PulleyBehavior>().platformState;
                serializablePulleyState.platformPosition = transform.GetComponent<PulleyBehavior>().platformPosition;
                serializablePulleyState.ballRollDirection = transform.GetComponent<PulleyBehavior>().ballRollDirection;
            }
            return serializablePulleyState;
        }

        private static void SetPulleyState(
            GameObject preplacedObject, 
            SerializablePreplacedObject serializedPreplacedObject)
        {
            if (preplacedObject.GetComponent<PulleyBehavior>() != null) 
            {
                PulleyBehavior pulleyBehavior = preplacedObject.GetComponent<PulleyBehavior>();
                pulleyBehavior.platformState = serializedPreplacedObject.pulleyState.platformState;
                pulleyBehavior.platformPosition = serializedPreplacedObject.pulleyState.platformPosition;
                
                pulleyBehavior.SwitchPlatformState(serializedPreplacedObject.pulleyState.platformState);
                pulleyBehavior.SwitchPlatformPosition(serializedPreplacedObject.pulleyState.platformPosition);
                pulleyBehavior.ballRollDirection = serializedPreplacedObject.pulleyState.ballRollDirection;
                pulleyBehavior.SetOrginalPosition();
                pulleyBehavior.ResetPlatformHeight();

                
            }
            return;
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
                else if (gameObject.name.Equals(HINTS_KEY))
                {
                    PopulateHints(gameObject, levelData.hints);
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
                SetPulleyState(preplacedObject, serializedPreplacedObject);
            }
        }

        private static void PopulateHints(GameObject hintsContainer, List<SerializableHint> hints)
        {
            DeleteAllChildren(hintsContainer);
            foreach (SerializableHint serializedHint in hints)
            {
                GameObject hintPrefab = PlacedObjectPrefabDictionary.Get(serializedHint.objectName);
                GameObject hintObject = Object.Instantiate(
                    hintPrefab,
                    serializedHint.textTransform.position,
                    serializedHint.textTransform.rotation,
                    hintsContainer.transform
                );
                hintObject.transform.localScale = serializedHint.textTransform.scale;
                if (serializedHint.isArrow)
                {
                    TextMesh textMesh = hintObject.GetComponent<TextMesh>();
                    textMesh.text = serializedHint.text;
                    textMesh.characterSize = serializedHint.characterSize;
                    textMesh.fontSize = serializedHint.fontSize;
                    Transform arrowTransform =
                        hintObject.transform.Find(HINT_ARROW_KEY).transform;
                    arrowTransform.position = serializedHint.arrowTransform.position;
                    arrowTransform.rotation = serializedHint.arrowTransform.rotation;
                    arrowTransform.localScale = serializedHint.arrowTransform.scale;
                    HintArrowEffectsManager hintArrowEffectsManager =
                        arrowTransform.GetComponent<HintArrowEffectsManager>();
                    hintArrowEffectsManager.SetMinScaleVector(serializedHint.minArrowScale);
                    hintArrowEffectsManager.SetMaxScaleVector(serializedHint.maxArrowScale);
                }
                else
                {
                    SpriteRenderer spiteRenderer = hintObject.GetComponent<SpriteRenderer>();
                    spiteRenderer.sprite =
                        OutlineHintSpriteDictionary.GetSpriteByName(serializedHint.outlineHintName);
                    spiteRenderer.flipX = serializedHint.outlineIsFlipped;
                    OutlineHintOrderTracker orderTracker =
                        hintObject.GetComponent<OutlineHintOrderTracker>();
                    orderTracker.order = serializedHint.outlineHintOrder;
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
