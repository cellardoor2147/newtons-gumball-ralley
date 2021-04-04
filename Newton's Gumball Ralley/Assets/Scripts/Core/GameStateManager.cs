using UnityEngine;
using UnityEngine.SceneManagement;
using GUI;
using GUI.Dialogue;
using SimpleMachine;
using Ball;
using System.Linq;
using System.Collections.Generic;
using Audio;

namespace Core
{
    public enum GameState
    {
        OpeningCutscene = 0,
        MainMenu = 1,
        Dialogue = 2,
        Playing = 3,
        Editing = 4,
        Paused = 5
    }

    public class GameStateManager : MonoBehaviour
    {
        public static readonly string PLACED_OBJECTS_KEY = "Placed Objects";

        private readonly static string SLING_ANCHOR_KEY = "Sling Anchor";
        private readonly static string MAIN_MENU_SCENE_KEY = "Main Menu";
        private readonly static string GAME_SCENE_KEY = "Game";

        private static GameStateManager instance;
        
        // TODO: remove and instead load conversations from the current level,
        // if there is a conversation to load
        [SerializeField] private Conversation exampleConversation;

        [SerializeField] SoundMetaData CutsceneMusicSound;
        [SerializeField] SoundMetaData MenuMusicSound;
        [SerializeField] SoundMetaData Level1MusicSound;
        [SerializeField] SoundMetaData Level2MusicSound;
        [SerializeField] SoundMetaData DialogueMusicSound;

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
                Destroy(this.gameObject);
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

            SetGameState(GameState.OpeningCutscene);
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
                    GUIManager.SetActiveGUI(GUIType.Cutscene);
                    AudioManager.instance.PlaySound(instance.CutsceneMusicSound.name);
                    break;
                case GameState.MainMenu:
                    Time.timeScale = 0.0f;
                    LoadScene(MAIN_MENU_SCENE_KEY);
                    GUIManager.SetActiveGUI(GUIType.MainMenu);
                    AudioManager.instance.StopSound(instance.CutsceneMusicSound.name);
                    AudioManager.instance.PlaySound(instance.MenuMusicSound.name);
                    break;
                case GameState.Dialogue:
                    Time.timeScale = 0.0f;
                    AudioManager.instance.StopSound(instance.MenuMusicSound.name);
                    AudioManager.instance.PlaySound(instance.DialogueMusicSound.name);
                    LoadScene(GAME_SCENE_KEY);
                    GUIManager.SetActiveGUI(GUIType.Dialogue);
                    // TODO: remove and instead load conversations from the current level,
                    // if there is a conversation to load
                    GUIManager.StartConversation(instance.exampleConversation);
                    break;
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    AudioManager.instance.StopSound(instance.DialogueMusicSound.name);
                    AudioManager.instance.PlaySound(instance.Level2MusicSound.name);
                    LoadScene(GAME_SCENE_KEY);
                    TetherPlacedObjectsToPlacedScrews();
                    UnfreezePlacedObjectsRigidbodies();
                    Physics2D.gravity = instance.defaultGravity;
                    GUIManager.SetActiveGUI(GUIType.PlayMode);
                    break;
                case GameState.Editing:
                    Time.timeScale = 1.0f;
                    LoadScene(GAME_SCENE_KEY);
                    UntetherPlacedObjectsFromPlacedScrews();
                    ResetBallPosition();
                    ResetPlacedObjectsTransforms();
                    FreezePlacedObjectsRigidbodies();
                    Physics2D.gravity = Vector2.zero;
                    GUIManager.SetActiveGUI(GUIType.EditMode);
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    LoadScene(GAME_SCENE_KEY);
                    GUIManager.SetActiveGUI(GUIType.SettingsMenu);
                    AudioManager.instance.PauseSound(instance.Level2MusicSound.name);
                    break;
                default:
                    Debug.Log($"Tried setting invalid game state: {gameState}");
                    break;
            }
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

        private static void ResetBallPosition()
        {
            GameObject slingAnchor = GameObject.Find(SLING_ANCHOR_KEY);
            if (slingAnchor == null)
            {
                // Scene hasn't loaded yet, so sling anchor won't exist
                return;
            }
            slingAnchor.GetComponentInChildren<BallMovement>().ResetPosition();
        }

        private static void ResetPlacedObjectsTransforms()
        {
            GameObject.Find(PLACED_OBJECTS_KEY)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.ResetTransform()
            );
        }

        private static void UnfreezePlacedObjectsRigidbodies()
        {
            GameObject placedObjectsContainer = GameObject.Find(PLACED_OBJECTS_KEY);
            if (placedObjectsContainer == null)
            {
                return;
            }
            placedObjectsContainer
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.UnfreezeRigidbody()
            );
        }

        private static void FreezePlacedObjectsRigidbodies()
        {
            GameObject placedObjectsContainer = GameObject.Find(PLACED_OBJECTS_KEY);
            if (placedObjectsContainer == null)
            {
                return;
            }
            placedObjectsContainer
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.FreezeRigidbody()
            );
        }

        private static void TetherPlacedObjectsToPlacedScrews()
        {
            List<Collider2D> placedObjectColliders = GetPlacedObjectsColliders();
            for (int i = 0; i < placedObjectColliders.Count; i++)
            {
                Collider2D collider1 = placedObjectColliders[i];
                bool collider1BelongsToScrew =
                    collider1.gameObject.GetComponent<Rigidbody2D>() == null;
                if (collider1BelongsToScrew)
                {
                    continue;
                }
                for (int j = 0; j < placedObjectColliders.Count; j++)
                {
                    Collider2D collider2 = placedObjectColliders[j];
                    bool collider2DoesNotBelongToScrew = i == j
                        || collider2.gameObject.GetComponent<Rigidbody2D>() != null;
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
                    TetherObjectToScrew(collider1.gameObject, collider2.gameObject);
                }
            }
        }

        private static void TetherObjectToScrew(GameObject otherObject, GameObject screw)
        {
            otherObject.AddComponent<HingeJoint2D>();
            screw.transform.SetParent(otherObject.transform, true);
            otherObject.GetComponent<HingeJoint2D>().anchor = screw.transform.localPosition;
            otherObject.GetComponent<HingeJoint2D>().enableCollision = true;
        }

        private static void UntetherPlacedObjectsFromPlacedScrews()
        {
            List<Transform> screws = new List<Transform>();
            foreach (Collider2D collider in GetPlacedObjectsColliders())
            {
                foreach (Transform child in collider.transform)
                {
                    if (child.gameObject.GetComponent<Rigidbody2D>() == null)
                    {
                        screws.Add(child);
                    }
                }
                foreach (HingeJoint2D joint in collider.gameObject.GetComponents<HingeJoint2D>())
                {
                    Destroy(joint);
                }
            }
            GameObject placedObjectContainer = GameObject.Find(PLACED_OBJECTS_KEY);
            foreach (Transform screw in screws)
            {
                screw.SetParent(placedObjectContainer.transform, true);
            }
        }

        private static List<Collider2D> GetPlacedObjectsColliders()
        {
            GameObject placedObjectContainer = GameObject.Find(PLACED_OBJECTS_KEY);
            if (placedObjectContainer == null)
            {
                // Game scene not loaded in yet, so this container
                // doesn't exist
                return new List<Collider2D>();
            }
            return placedObjectContainer.GetComponentsInChildren<Collider2D>(true).ToList();
        }

        public static void ResetCurrentLevel()
        {
            // TODO: properly reset scene using JSON object
            // once level serialization works properly
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SetGameState(GameState.Playing);
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
                gameState.Equals(GameState.Playing) &&
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
                SetGameState(GameState.Playing);
                return;
            }
        }
    }
}
