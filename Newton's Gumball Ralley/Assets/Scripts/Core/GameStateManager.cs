﻿using UnityEngine;
using UnityEngine.SceneManagement;
using GUI;

namespace Core
{
    public enum GameState
    {
        OpeningCutscene = 0,
        MainMenu = 1,
        Playing = 2,
        Paused = 3
    }

    public class GameStateManager : MonoBehaviour
    {
        private readonly static string MAIN_MENU_SCENE_KEY = "Main Menu";
        private readonly static string GAME_SCENE_KEY = "Game";

        private static GameStateManager instance;

        private GameState gameState;

        private GameStateManager() {} // Prevents instantiation outside of this class

        private void Awake()
        {
            SetInstance();
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
                    break;
                case GameState.MainMenu:
                    Time.timeScale = 0.0f;
                    LoadScene(MAIN_MENU_SCENE_KEY);
                    GUIManager.SetActiveGUI(GUIType.MainMenu);
                    break;
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    LoadScene(GAME_SCENE_KEY);
                    GUIManager.SetActiveGUI(GUIType.PlayMode);
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    LoadScene(GAME_SCENE_KEY);
                    GUIManager.SetActiveGUI(GUIType.SettingsMenu);
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
