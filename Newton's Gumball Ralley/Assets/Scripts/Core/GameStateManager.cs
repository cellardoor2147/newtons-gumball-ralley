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
        
        [SerializeField] PlacedObjectMetaData gearBackgroundMetaData;
        [SerializeField] PlacedObjectMetaData axleMetaData;
        [SerializeField] PlacedObjectMetaData gear1MetaData;
        [SerializeField] PlacedObjectMetaData gear3MetaData;
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
                    AudioManager.instance.PlaySound(instance.CutsceneMusicSound.name);
                    break;
                case GameState.MainMenu:
                    Time.timeScale = 1.0f;
                    LoadScene(MAIN_MENU_SCENE_KEY);
                    instance.StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.MainMenu));
                    RepeatedBackgroundManager.SetDesiredNumberOfColumnsAndRows(5, 5);
                    AudioManager.instance.StopSound(instance.CutsceneMusicSound.name);
                    AudioManager.instance.PlaySound(instance.MenuMusicSound.name);
                    break;
                case GameState.Dialogue:
                    Time.timeScale = 0.0f;
                    AudioManager.instance.StopSound(instance.MenuMusicSound.name);
                    AudioManager.instance.PlaySound(instance.DialogueMusicSound.name);
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
                    ResetSceneForPlayMode();
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
                    ResetSceneForEditMode();
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
                    AudioManager.instance.StopSound(instance.Level2MusicSound.name);
                    AudioManager.instance.StopSound(instance.DialogueMusicSound.name);
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

        public static void ResetLevel()
        {
            if (instance.gameState.Equals(GameState.Playing))
            {
                instance.StartCoroutine(ResetGumballMachine());
                instance.StartCoroutine(ResetObjectsTransforms(PLACED_OBJECTS_KEY));
                instance.StartCoroutine(ResetObjectsTransforms(PREPLACED_OBJECTS_KEY));
                instance.StartCoroutine(ResetObjectsTransforms(ENVIRONMENT_KEY));
                instance.StartCoroutine(DestroyDebris(ENVIRONMENT_KEY));
                instance.StartCoroutine(RepairDestructibleObjects(ENVIRONMENT_KEY));
                instance.StartCoroutine(FreezeDestructibleObjects(ENVIRONMENT_KEY));
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
                ResetLevel();
            }
        }

        private static void ResetSceneForPlayMode()
        {
            instance.StartCoroutine(TetherObjectsToPlacedScrews(PLACED_OBJECTS_KEY));
            instance.StartCoroutine(TetherObjectsToPlacedScrews(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(UnfreezeObjectsRigidbodies(PLACED_OBJECTS_KEY));
            instance.StartCoroutine(UnfreezeObjectsRigidbodies(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(RevertObjectsFromGray(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(SetObjectsActive(PREPLACED_OBJECTS_KEY, instance.gearBackgroundMetaData, false));
            instance.StartCoroutine(RemoveAllRotationArrows(PLACED_OBJECTS_KEY));
            Physics2D.gravity = instance.defaultGravity;
        }

        private static void ResetSceneForEditMode()
        {
            instance.StartCoroutine(UntetherObjectsFromPlacedScrews(PLACED_OBJECTS_KEY));
            instance.StartCoroutine(UntetherObjectsFromPlacedScrews(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(ResetGumballMachine());
            instance.StartCoroutine(ResetObjectsTransforms(PLACED_OBJECTS_KEY));
            instance.StartCoroutine(ResetObjectsTransforms(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(ResetObjectsTransforms(ENVIRONMENT_KEY));
            instance.StartCoroutine(FreezeObjectsRigidbodies(PLACED_OBJECTS_KEY));
            instance.StartCoroutine(FreezeObjectsRigidbodies(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(FreezeObjectsRigidbodies(ENVIRONMENT_KEY));
            instance.StartCoroutine(GrayOutObjects(PREPLACED_OBJECTS_KEY));
            instance.StartCoroutine(AddAllRotationArrows(PLACED_OBJECTS_KEY));
            instance.StartCoroutine(DestroyDebris(ENVIRONMENT_KEY));
            instance.StartCoroutine(RepairDestructibleObjects(ENVIRONMENT_KEY));
            instance.StartCoroutine(SetObjectsActive(PREPLACED_OBJECTS_KEY, instance.gearBackgroundMetaData, true));
            instance.StartCoroutine(ResetDestructibleObjectLayer(ENVIRONMENT_KEY));
            Physics2D.gravity = Vector2.zero;
        }

        private static IEnumerator FreezeDestructibleObjects(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
               .GetComponentsInChildren<DestructibleObstacleLayerController>(true)
               .ToList()
               .ForEach(
                   layerController => layerController.FreezeRigidbody()
            );
        }

        private static IEnumerator ResetDestructibleObjectLayer(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
               .GetComponentsInChildren<DestructibleObstacleLayerController>(true)
               .ToList()
               .ForEach(
                   layerController => layerController.UpdateAllLayers(DestructibleObstacleLayerController.defaultLayer)
            );
        }

        private static IEnumerator RepairDestructibleObjects(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
               .GetComponentsInChildren<D2dDestructibleSprite>(true)
               .ToList()
               .ForEach(
                   destructibleSprite => destructibleSprite.Rebuild(2)
            );
        }

        private static IEnumerator DestroyDebris(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject objectContainer = GameObject.Find(key);
            foreach (Transform objectTransform in objectContainer.transform)
            {
                if (objectTransform.gameObject.CompareTag("Debris"))
                {
                    Destroy(objectTransform.gameObject);
                }
            }
        }

        private static IEnumerator ResetGumballMachine()
        {
            yield return new WaitUntil(() => GameObject.Find(GUMBALL_MACHINE_KEY) != null);
            GameObject gumballMachine = GameObject.Find(GUMBALL_MACHINE_KEY);
            GumballMachineManager gumballMachineManager =
                gumballMachine.GetComponent<GumballMachineManager>();
            gumballMachineManager.SetGumballMachineState(GumballMachineState.Closed);
        }

        private static IEnumerator ResetObjectsTransforms(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(false)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.ResetTransform()
            );
        }

        private static IEnumerator UnfreezeObjectsRigidbodies(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.UnfreezeRigidbody()
            );
        }

        private static IEnumerator FreezeObjectsRigidbodies(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.FreezeRigidbody()
            );
        }

        private static IEnumerator TetherObjectsToPlacedScrews(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
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
                    PlacedObjectMetaData collider2MetaData = collider2.gameObject.GetComponent<PlacedObjectManager>().metaData;
                    PlacedObjectMetaData collider1MetaData = collider1.gameObject.GetComponent<PlacedObjectManager>().metaData;
                    bool collider2IsAxle =
                        collider2MetaData.Equals(instance.axleMetaData);
                    bool collider1IsGearorWheel =
                        collider1MetaData.Equals(instance.gear1MetaData)
                        || collider1MetaData.Equals(instance.wheelMetaData)
                        || collider1MetaData.Equals(instance.gear3MetaData);
                    if (collider2IsAxle && !collider1IsGearorWheel)
                    {
                        continue;
                    }
                    bool collider2IsScrew = collider1MetaData.Equals(instance.screwMetaData);
                    if (collider2IsScrew && collider1IsGearorWheel)
                    {
                        continue;
                    }
                    TetherObjectToScrew(collider1.gameObject, collider2.gameObject);
                }
            }
            yield return null;
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
            otherObject.GetComponent<HingeJoint2D>().enableCollision = true;
        }

        private static IEnumerator UntetherObjectsFromPlacedScrews(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
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
            yield return null;
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

        private static IEnumerator RevertObjectsFromGray(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
                GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.RevertFromGray()
            );
        }

        private static IEnumerator GrayOutObjects(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
                GameObject.Find(key)
                .GetComponentsInChildren<PlacedObjectManager>(true)
                .ToList()
                .ForEach(
                    placedObjectManager => placedObjectManager.GrayOut()
            );
        }

        private static IEnumerator RemoveAllRotationArrows(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.RemoveRotationArrows()
            );
        }

        private static IEnumerator AddAllRotationArrows(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.AddRotationArrows()
            );
        }

        public static IEnumerator LoadNextLevel()
        {
            yield return new WaitUntil(() => GameObject.Find(PLACED_OBJECTS_KEY) != null);
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
