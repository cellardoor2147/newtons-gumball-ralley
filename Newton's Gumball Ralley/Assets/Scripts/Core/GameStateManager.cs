using UnityEngine;
using UnityEngine.SceneManagement;
using GUI;
using GUI.EditMode;
using SimpleMachine;
using Ball;
using Destructible2D;
using DestructibleObject;
using System.Linq;
using System.Collections.Generic;
using Audio;
using System.Collections;
using Background;
using Core.PlacedObjects;
using Core.Levels;

namespace Core
{
    public enum GameState
    {
        OpeningCutscene = 0,
        MainMenu = 1,
        Dialogue = 2,
        Playing = 3,
        Editing = 4,
        Paused = 5,
        LevelCompleted = 6,
        GameOver = 7
    }

    public class GameStateManager : MonoBehaviour
    {
        public static readonly string PLACED_OBJECTS_KEY = "Placed Objects";
        public static readonly string PREPLACED_OBJECTS_KEY = "Preplaced Objects";
        public static readonly string ENVIRONMENT_KEY = "Environment";

        private readonly static string GUMBALL_MACHINE_KEY = "Gumball Machine";
        private readonly static string MAIN_MENU_SCENE_KEY = "Main Menu";
        private readonly static string GAME_SCENE_KEY = "Game";

        private static GameStateManager instance;

        [SerializeField] SoundMetaData CutsceneMusicSound;
        [SerializeField] SoundMetaData MenuMusicSound;
        [SerializeField] SoundMetaData Level1MusicSound;
        [SerializeField] SoundMetaData Level2MusicSound;
        [SerializeField] SoundMetaData DialogueMusicSound;
        [SerializeField] SoundMetaData LevelCompleteSound;
        [SerializeField] SoundMetaData BallRollingSound;
        
        [SerializeField] PlacedObjectMetaData gearBackgroundMetaData;
        [SerializeField] PlacedObjectMetaData axleMetaData;
        [SerializeField] PlacedObjectMetaData gear1MetaData;
        [SerializeField] PlacedObjectMetaData screwMetaData;
        [SerializeField] PlacedObjectMetaData wheelMetaData;

        private GameState previousGameState;
        private GameState gameState;
        private Vector2 defaultGravity;

        private GameStateManager() {} // Prevents instantiation outside of this class

        private void Awake()
        {
            SetInstance();
            defaultGravity = Physics2D.gravity;
            PreventManagersContainerFromBeingDestroyedOnLoad();
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

        private void PreventManagersContainerFromBeingDestroyedOnLoad()
        {
            DontDestroyOnLoad(transform.parent.gameObject);
        }

        private void Start()
        {
            if (AudioManager.instance == null)
            {
                Debug.LogError("No audiomanager found");
            }
            if (SceneManager.GetActiveScene().name.Equals(MAIN_MENU_SCENE_KEY))
            {
                SetGameState(GameState.OpeningCutscene);
            }
            else
            {
                SetGameState(GameState.Editing);
            }
        }

        public static GameState GetPreviousGameState()
        {
            return instance.previousGameState;
        }

        public static GameState GetGameState()
        {
            return instance.gameState;
        }

        public static void SetGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.OpeningCutscene:
                    Time.timeScale = 1.0f;
                    LoadScene(MAIN_MENU_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.Cutscene));
                    RepeatedBackgroundManager.SetDesiredNumberOfColumnsAndRows(5, 5);
                    break;
                case GameState.MainMenu:
                    Time.timeScale = 1.0f;
                    LoadScene(MAIN_MENU_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.MainMenu));
                    RepeatedBackgroundManager.SetDesiredNumberOfColumnsAndRows(5, 5);
                    StopAllMusic();
                    AudioManager.instance.PlaySound(instance.MenuMusicSound.name);
                    break;
                case GameState.Dialogue:
                    Time.timeScale = 0.0f;
                    AudioManager.instance.StopSound(instance.BallRollingSound.name);
                    StopAllMusic();
                    if (!AudioManager.instance.isPlaying(instance.DialogueMusicSound.name))
                    {
                        AudioManager.instance.PlaySound(instance.DialogueMusicSound.name);
                    }
                    LoadScene(GAME_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.Dialogue));
                    break;
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    AudioManager.instance.StopSound(instance.MenuMusicSound.name);
                    AudioManager.instance.StopSound(instance.DialogueMusicSound.name);  
                    if (!AudioManager.instance.isPlaying(instance.Level2MusicSound.name)) {
                        AudioManager.instance.PlaySound(instance.Level2MusicSound.name);
                    } 
                    LoadScene(GAME_SCENE_KEY);
                    if (instance.gameState != GameState.Paused
                        && instance.gameState != GameState.GameOver)
                    {
                        instance.StartCoroutine(ResetSceneForPlayMode());
                    }
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.PlayMode));
                    break;
                case GameState.Editing:
                    Time.timeScale = 1.0f;
                    LoadScene(GAME_SCENE_KEY);              
                    AudioManager.instance.StopSound(instance.MenuMusicSound.name);
                    AudioManager.instance.StopSound(instance.DialogueMusicSound.name);
                    if (!AudioManager.instance.isPlaying(instance.Level2MusicSound.name)) {
                        AudioManager.instance.PlaySound(instance.Level2MusicSound.name);
                    }
                    if (instance.gameState != GameState.Paused)
                    {
                        instance.StartCoroutine(ResetSceneForEditMode());
                    }
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.EditMode));
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    LoadScene(GAME_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.SettingsMenu));
                    AudioManager.instance.PauseSound(instance.Level2MusicSound.name);
                    break;
                case GameState.LevelCompleted:
                    Time.timeScale = 1.0f;
                    LoadScene(GAME_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.LevelCompletedPopup));
                    StopAllMusic();
                    if (!AudioManager.instance.isPlaying(instance.LevelCompleteSound.name))
                    {
                        AudioManager.instance.PlaySound(instance.LevelCompleteSound.name);
                    }
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1.0f;
                    LoadScene(GAME_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.GameOverPopup));
                    break;
                default:
                    Debug.Log($"Tried setting invalid game state: {gameState}");
                    break;
            }
            instance.previousGameState = instance.gameState;
            instance.gameState = gameState;
        }

        private static void LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.Equals(sceneName))
            {
                return;
            }
            SceneManager.LoadScene(sceneName);
        }

        public static IEnumerator ResetLevel()
        {
            yield return new WaitUntil(() => GameObject.Find(PLACED_OBJECTS_KEY) != null);
            yield return new WaitUntil(() => GameObject.Find(PREPLACED_OBJECTS_KEY) != null);
            yield return new WaitUntil(() => GameObject.Find(ENVIRONMENT_KEY) != null);
            if (instance.gameState.Equals(GameState.Playing))
            {
                ResetGumballMachine();
                ResetObjectsTransforms(PLACED_OBJECTS_KEY);
                ResetObjectsTransforms(PREPLACED_OBJECTS_KEY);
                ResetObjectsTransforms(ENVIRONMENT_KEY);
                DestroyDebris(ENVIRONMENT_KEY);
                RepairDestructibleObjects(ENVIRONMENT_KEY);
                FreezeDestructibleObjects(ENVIRONMENT_KEY);
                ResetDestructibleObjectLayer(ENVIRONMENT_KEY);
                FreezeLeverWeights(ENVIRONMENT_KEY);
                ResetRigidbodies(PLACED_OBJECTS_KEY);
                ResetRigidbodies(PREPLACED_OBJECTS_KEY);
            }
            else if (instance.gameState.Equals(GameState.Editing))
            {
                DeleteAllChildren(GameObject.Find(PLACED_OBJECTS_KEY));
                ScrapManager.ResetRemainingScrap();
                EditModeManager.ToggleButtonsBasedOnAvailableScrap();
            }
            else if (instance.gameState.Equals(GameState.GameOver))
            {
                SetGameState(GameState.Playing);
                instance.StartCoroutine(ResetLevel());
            }
        }

        private static IEnumerator ResetSceneForPlayMode()
        {
            yield return new WaitUntil(() => GameObject.Find(PLACED_OBJECTS_KEY) != null);
            yield return new WaitUntil(() => GameObject.Find(PREPLACED_OBJECTS_KEY) != null);
            yield return new WaitUntil(() => GameObject.Find(ENVIRONMENT_KEY) != null);
            TetherObjectsToPlacedScrews(PLACED_OBJECTS_KEY);
            TetherObjectsToPlacedScrews(PREPLACED_OBJECTS_KEY);
            UnfreezeObjectsRigidbodies(PLACED_OBJECTS_KEY);
            UnfreezeObjectsRigidbodies(PREPLACED_OBJECTS_KEY);
            RevertObjectsFromGray(PREPLACED_OBJECTS_KEY);
            SetObjectsActive(ENVIRONMENT_KEY, instance.gearBackgroundMetaData, false);
            RemoveAllRotationArrows(PLACED_OBJECTS_KEY);
            Physics2D.gravity = instance.defaultGravity;
        }

        private static void StopAllMusic()
        {
            AudioManager.instance.StopSound(instance.DialogueMusicSound.name);
            AudioManager.instance.StopSound(instance.CutsceneMusicSound.name);
            AudioManager.instance.StopSound(instance.MenuMusicSound.name);
            AudioManager.instance.StopSound(instance.Level1MusicSound.name);
            AudioManager.instance.StopSound(instance.Level2MusicSound.name);
        }

        private static IEnumerator ResetSceneForEditMode()
        {
            yield return new WaitUntil(() => GameObject.Find(PLACED_OBJECTS_KEY) != null);
            yield return new WaitUntil(() => GameObject.Find(PREPLACED_OBJECTS_KEY) != null);
            yield return new WaitUntil(() => GameObject.Find(ENVIRONMENT_KEY) != null);
            UntetherObjectsFromPlacedScrews(PLACED_OBJECTS_KEY);
            UntetherObjectsFromPlacedScrews(PREPLACED_OBJECTS_KEY);
            ResetGumballMachine();
            ResetObjectsTransforms(PLACED_OBJECTS_KEY);
            ResetObjectsTransforms(PREPLACED_OBJECTS_KEY);
            ResetObjectsTransforms(ENVIRONMENT_KEY);
            FreezeObjectsRigidbodies(PLACED_OBJECTS_KEY);
            FreezeObjectsRigidbodies(PREPLACED_OBJECTS_KEY);
            FreezeObjectsRigidbodies(ENVIRONMENT_KEY);
            GrayOutObjects(PREPLACED_OBJECTS_KEY);
            AddAllRotationArrows(PLACED_OBJECTS_KEY);
            DestroyDebris(ENVIRONMENT_KEY);
            RepairDestructibleObjects(ENVIRONMENT_KEY);
            SetObjectsActive(ENVIRONMENT_KEY, instance.gearBackgroundMetaData, true);
            ResetDestructibleObjectLayer(ENVIRONMENT_KEY);
            Physics2D.gravity = Vector2.zero;
        }

        private static void FreezeLeverWeights(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<LeverWeightBehavior>(true)
                .ToList()
                .ForEach(
                    leverWeight => leverWeight.FreezeRigidbody()
            );
        }

        private static void ResetRigidbodies(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<Rigidbody2D>()
                .ToList()
                .ForEach(
                    rigidbody => ResetRigidbody(rigidbody)
            );
        }

        private static void ResetRigidbody(Rigidbody2D rigidbody)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0f;
        }

        private static void FreezeDestructibleObjects(string key)
        {
            GameObject.Find(key)
               .GetComponentsInChildren<DestructibleObstacleLayerController>(true)
               .ToList()
               .ForEach(
                   layerController => layerController.FreezeRigidbody()
            );
        }

        private static void ResetDestructibleObjectLayer(string key)
        {
            GameObject.Find(key)
               .GetComponentsInChildren<DestructibleObstacleLayerController>(true)
               .ToList()
               .ForEach(
                   layerController => layerController.UpdateAllLayers(DestructibleObstacleLayerController.defaultLayer)
            );
        }

        private static void RepairDestructibleObjects(string key)
        {
            GameObject.Find(key)
               .GetComponentsInChildren<D2dDestructibleSprite>(true)
               .ToList()
               .ForEach(
                   destructibleSprite => destructibleSprite.Rebuild(2)
            );
        }

        private static void DestroyDebris(string key)
        {
            GameObject objectContainer = GameObject.Find(key);
            foreach (Transform objectTransform in objectContainer.transform)
            {
                if (objectTransform.gameObject.CompareTag("Debris"))
                {
                    Destroy(objectTransform.gameObject);
                }
            }
        }

        private static void ResetGumballMachine()
        {
            GameObject gumballMachine = GameObject.Find(GUMBALL_MACHINE_KEY);
            GumballMachineManager gumballMachineManager =
                gumballMachine.GetComponent<GumballMachineManager>();
            gumballMachineManager.SetGumballMachineState(GumballMachineState.Closed);
        }

        private static void ResetObjectsTransforms(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(false)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.ResetTransform()
            );
        }

        private static void UnfreezeObjectsRigidbodies(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.UnfreezeRigidbody()
            );
        }

        private static void FreezeObjectsRigidbodies(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.FreezeRigidbody()
            );
        }

        private static void TetherObjectsToPlacedScrews(string key)
        {
            GameObject objectContainer = GameObject.Find(key);
            List<Collider2D> objectColliders =
                objectContainer.GetComponentsInChildren<Collider2D>(true).ToList();
            for (int i = 0; i < objectColliders.Count; i++)
            {
                Collider2D collider1 = objectColliders[i];
                bool collider1BelongsToScrew =
                    collider1.gameObject.CompareTag("Screw");
                if (collider1BelongsToScrew)
                {
                    continue;
                }
                for (int j = 0; j < objectColliders.Count; j++)
                {
                    Collider2D collider2 = objectColliders[j];
                    bool collider2DoesNotBelongToScrew = i == j
                        || !collider2.gameObject.CompareTag("Screw");
                    if (collider2DoesNotBelongToScrew)
                    {
                        continue;
                    }
                    bool objectCollidesWithScrew =
                        Physics2D.IsTouching(collider1, collider2);
                    if (!objectCollidesWithScrew)
                    {
                        continue;
                    }
                    bool collider2IsFulcrumScrew =
                        collider2.gameObject.name.Equals("FulcrumScrew");
                    if (collider2IsFulcrumScrew)
                    {
                        FulcrumScrewBehavior fulcrumScrew = collider2.gameObject.GetComponent<FulcrumScrewBehavior>();
                        if (!fulcrumScrew.FulcrumJointShouldBeCreated)
                            continue;
                    }
                    else
                    {
                        PlacedObjectMetaData collider2MetaData = collider2.gameObject.GetComponent<PlacedObjectManager>().metaData;
                        PlacedObjectMetaData collider1MetaData = collider1.gameObject.GetComponent<PlacedObjectManager>().metaData;
                        bool collider2IsAxle =
                            collider2MetaData.Equals(instance.axleMetaData);
                        bool collider1IsGearorWheel =
                            collider1MetaData.Equals(instance.gear1MetaData)
                            || collider1MetaData.Equals(instance.wheelMetaData);
                        if (collider2IsAxle && !collider1IsGearorWheel)
                        {
                            continue;
                        }
                        bool collider2IsScrew = collider1MetaData.Equals(instance.screwMetaData);
                        if (collider2IsScrew && collider1IsGearorWheel)
                        {
                            continue;
                        }
                    }
                    TetherObjectToScrew(collider1.gameObject, collider2.gameObject);
                }
            }
        }

        private static void TetherObjectToScrew(GameObject otherObject, GameObject screw)
        {
            Transform previousParent = screw.transform.parent;
            otherObject.AddComponent<HingeJoint2D>();
            screw.transform.SetParent(otherObject.transform, true); /* screw's parent needs to be set to thing 
                                                                     * with joint; otherwise joint won't work */
            otherObject.GetComponent<HingeJoint2D>().anchor = screw.transform.localPosition;
            screw.transform.SetParent(previousParent, true); /* reverts reparenting done above; 
                                                              * e.g. with FulcrumScrew, it needs to be a child
                                                              * of LeverFulcrum, so this line sets the parent of
                                                              * FulcrumScrew back to LeverFulcrum */
            foreach (HingeJoint2D hingeJoint in otherObject.GetComponents<HingeJoint2D>())
            {
                hingeJoint.enableCollision = true;
            }
        }

        private static void UntetherObjectsFromPlacedScrews(string key)
        {
            GameObject objectContainer = GameObject.Find(key);
            List<Collider2D> objectColliders =
                objectContainer.GetComponentsInChildren<Collider2D>(true).ToList();
            foreach (Collider2D collider in objectColliders)
            {
                foreach (HingeJoint2D joint in collider.gameObject.GetComponents<HingeJoint2D>())
                {
                    Destroy(joint);
                }
            }
        }

        private static IEnumerator SetObjectsActive(string key, PlacedObjectMetaData metaData, bool setActive)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject objectContainer = GameObject.Find(key);
            foreach (Transform placeableobject in objectContainer.transform)
            {
                if (placeableobject.gameObject.GetComponent<PlacedObjectManager>().metaData.Equals(metaData))
                {
                    placeableobject.gameObject.SetActive(setActive);
                }
            }
        }

        private static void RevertObjectsFromGray(string key)
        {
                GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.RevertFromGray()
            );
        }

        private static void GrayOutObjects(string key)
        {
                GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.GrayOut()
            );
        }

        private static void RemoveAllRotationArrows(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.RemoveRotationArrows()
            );
        }

        private static void AddAllRotationArrows(string key)
        {
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.AddRotationArrows()
            );
        }

        public static void LoadNextLevel()
        {
            DeleteAllChildren(GameObject.Find(PLACED_OBJECTS_KEY));
            LevelManager.LoadNextLevel();
        }

        private static void DeleteAllChildren(GameObject gameObject)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public static void StartStaticCoroutine(IEnumerator coroutineEnumerator)
        {
            instance.StartCoroutine(coroutineEnumerator);
        }

        public static bool GameSceneSceneIsRunning()
        {
            return SceneManager.GetActiveScene().name.Equals(GAME_SCENE_KEY);
        }

        private void Update()
        {
            bool shouldSkipOpeningCutscene =
                gameState.Equals(GameState.OpeningCutscene) &&
                Input.anyKeyDown;
            if (shouldSkipOpeningCutscene)
            {
                SetGameState(GameState.MainMenu);
                return;
            }
            bool shouldOpenSettingsMenuByKeyPress =
                (gameState.Equals(GameState.Playing) ||
                gameState.Equals(GameState.Editing)) &&
                (Input.GetKeyDown(KeyCode.P) ||
                Input.GetKeyDown(KeyCode.Escape));
            if (shouldOpenSettingsMenuByKeyPress)
            {
                SetGameState(GameState.Paused);
                return;
            }
            bool shouldCloseSettingsMenuByKeyPress =
                gameState.Equals(GameState.Paused) &&
                (Input.GetKeyDown(KeyCode.P) ||
                Input.GetKeyDown(KeyCode.Escape));
            if (shouldCloseSettingsMenuByKeyPress)
            {
                SetGameState(previousGameState);
                return;
            }
        }
    }
}
