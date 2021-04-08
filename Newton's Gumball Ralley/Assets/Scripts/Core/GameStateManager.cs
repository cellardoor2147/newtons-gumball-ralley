using UnityEngine;
using UnityEngine.SceneManagement;
using GUI;
using GUI.Dialogue;
using SimpleMachine;
using FulcrumScrew;
using Ball;
using System.Linq;
using System.Collections.Generic;
using Audio;
using System.Collections;

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
        public static readonly string PREPLACED_OBJECTS_KEY = "Preplaced Objects";

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
                    AudioManager.instance.StopSound(instance.MenuMusicSound.name);
                    AudioManager.instance.StopSound(instance.DialogueMusicSound.name);  
                    if (!AudioManager.instance.isPlaying(instance.Level2MusicSound.name)) {
                        AudioManager.instance.PlaySound(instance.Level2MusicSound.name);
                    } 
                    LoadScene(GAME_SCENE_KEY);
                    instance.StartCoroutine(TetherObjectsToPlacedScrews(PLACED_OBJECTS_KEY));
                    instance.StartCoroutine(TetherObjectsToPlacedScrews(PREPLACED_OBJECTS_KEY));
                    instance.StartCoroutine(UnfreezeObjectsRigidbodies(PLACED_OBJECTS_KEY));
                    instance.StartCoroutine(UnfreezeObjectsRigidbodies(PREPLACED_OBJECTS_KEY));
                    instance.StartCoroutine(RevertObjectsFromGray(PREPLACED_OBJECTS_KEY));
                    Physics2D.gravity = instance.defaultGravity;
                    GUIManager.SetActiveGUI(GUIType.PlayMode);
                    break;
                case GameState.Editing:
                    Time.timeScale = 1.0f;
                    LoadScene(GAME_SCENE_KEY);              
                    AudioManager.instance.StopSound(instance.MenuMusicSound.name);
                    AudioManager.instance.StopSound(instance.DialogueMusicSound.name);
                    if (!AudioManager.instance.isPlaying(instance.Level2MusicSound.name)) {
                        AudioManager.instance.PlaySound(instance.Level2MusicSound.name);
                    }
                    instance.StartCoroutine(UntetherObjectsFromPlacedScrews(PLACED_OBJECTS_KEY));
                    instance.StartCoroutine(UntetherObjectsFromPlacedScrews(PREPLACED_OBJECTS_KEY));
                    instance.StartCoroutine(ResetBallPosition());
                    instance.StartCoroutine(ResetObjectsTransforms(PLACED_OBJECTS_KEY));
                    instance.StartCoroutine(ResetObjectsTransforms(PREPLACED_OBJECTS_KEY));
                    instance.StartCoroutine(FreezeObjectsRigidbodies(PLACED_OBJECTS_KEY));
                    instance.StartCoroutine(FreezeObjectsRigidbodies(PREPLACED_OBJECTS_KEY));
                    instance.StartCoroutine(GrayOutObjects(PREPLACED_OBJECTS_KEY));
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

        private static IEnumerator ResetBallPosition()
        {
            yield return new WaitUntil(() => GameObject.Find(SLING_ANCHOR_KEY) != null);
            GameObject slingAnchor = GameObject.Find(SLING_ANCHOR_KEY);
            slingAnchor.GetComponentInChildren<BallMovement>().ResetPosition();
        }

        private static IEnumerator ResetObjectsTransforms(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.ResetTransform()
            );
        }

        private static IEnumerator UnfreezeObjectsRigidbodies(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.UnfreezeRigidbody()
            );
        }

        private static IEnumerator FreezeObjectsRigidbodies(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.FreezeRigidbody()
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
                    TetherObjectToScrew(collider1.gameObject, collider2.gameObject);
                }
            }
            yield return null;
        }

        private static void TetherObjectToScrew(GameObject otherObject, GameObject screw)
        {
            Transform previousParent = screw.transform.parent;
            otherObject.AddComponent<HingeJoint2D>();
            screw.transform.SetParent(otherObject.transform, true);
            otherObject.GetComponent<HingeJoint2D>().anchor = screw.transform.localPosition;
            screw.transform.SetParent(previousParent, true);
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

        private static IEnumerator RevertObjectsFromGray(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.RevertFromGray()
            );
        }

        private static IEnumerator GrayOutObjects(string key)
        {
            yield return new WaitUntil(() => GameObject.Find(key) != null);
            GameObject.Find(key)
                .GetComponentsInChildren<DraggingController>(true)
                .ToList()
                .ForEach(
                    draggingController => draggingController.GrayOut()
            );
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
