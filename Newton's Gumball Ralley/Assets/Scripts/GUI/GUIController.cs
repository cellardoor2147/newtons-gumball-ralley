using Core;
using LevelTimer;
using UnityEngine;

namespace GUI
{
    public class GUIController : MonoBehaviour
    {
        private readonly static string PRIMARY_MENU_KEY = "Primary Menu";
        private readonly static string MODE_MENU_KEY = "Mode Menu";
        private readonly static string LEVEL_MENU_KEY = "Level Menu";
        private readonly static string GUMBALL_COLOR_MENU_KEY = "Gumball Color Menu";
        private readonly static string LANGUAGE_MENU_KEY = "Language Menu";
        private readonly static string CREDITS_MENU_KEY = "Credits Menu";

        public GUIType guiType;

        public void LoadMainMenu()
        {
            GameStateManager.SetGameState(GameState.MainMenu);
            LoadMainMenuPrimaryMenu();
        }

        public void LoadMainMenuPrimaryMenu()
        {
            if (!CanLoadMainMenuSubMenu())
            {
                return;
            }
            transform.Find(PRIMARY_MENU_KEY).gameObject.SetActive(true);
            transform.Find(MODE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LEVEL_MENU_KEY).gameObject.SetActive(false);
            transform.Find(GUMBALL_COLOR_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LANGUAGE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(CREDITS_MENU_KEY).gameObject.SetActive(false);
        }

        public void LoadMainMenuModeMenu()
        {
            if (!CanLoadMainMenuSubMenu())
            {
                return;
            }
            transform.Find(PRIMARY_MENU_KEY).gameObject.SetActive(false);
            transform.Find(MODE_MENU_KEY).gameObject.SetActive(true);
            transform.Find(LEVEL_MENU_KEY).gameObject.SetActive(false);
            transform.Find(GUMBALL_COLOR_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LANGUAGE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(CREDITS_MENU_KEY).gameObject.SetActive(false);
        }

        public void LoadMainMenuLevelMenu()
        {
            if (!CanLoadMainMenuSubMenu())
            {
                return;
            }
            transform.Find(PRIMARY_MENU_KEY).gameObject.SetActive(false);
            transform.Find(MODE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LEVEL_MENU_KEY).gameObject.SetActive(true);
            transform.Find(GUMBALL_COLOR_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LANGUAGE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(CREDITS_MENU_KEY).gameObject.SetActive(false);
        }

        public void LoadMainMenuGumballColorMenu()
        {
            if (!CanLoadMainMenuSubMenu())
            {
                return;
            }
            transform.Find(PRIMARY_MENU_KEY).gameObject.SetActive(false);
            transform.Find(MODE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LEVEL_MENU_KEY).gameObject.SetActive(false);
            transform.Find(GUMBALL_COLOR_MENU_KEY).gameObject.SetActive(true);
            transform.Find(LANGUAGE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(CREDITS_MENU_KEY).gameObject.SetActive(false);
        }

        public void LoadMainMenuLanguageMenu()
        {
            if (!CanLoadMainMenuSubMenu())
            {
                return;
            }
            transform.Find(PRIMARY_MENU_KEY).gameObject.SetActive(false);
            transform.Find(MODE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LEVEL_MENU_KEY).gameObject.SetActive(false);
            transform.Find(GUMBALL_COLOR_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LANGUAGE_MENU_KEY).gameObject.SetActive(true);
            transform.Find(CREDITS_MENU_KEY).gameObject.SetActive(false);
        }

        public void LoadMainMenuCreditsMenu()
        {
            if (!CanLoadMainMenuSubMenu())
            {
                return;
            }
            transform.Find(PRIMARY_MENU_KEY).gameObject.SetActive(false);
            transform.Find(MODE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LEVEL_MENU_KEY).gameObject.SetActive(false);
            transform.Find(GUMBALL_COLOR_MENU_KEY).gameObject.SetActive(false);
            transform.Find(LANGUAGE_MENU_KEY).gameObject.SetActive(false);
            transform.Find(CREDITS_MENU_KEY).gameObject.SetActive(true);
        }

        private bool CanLoadMainMenuSubMenu()
        {
            return GameStateManager.GetGameState().Equals(GameState.MainMenu)
                && guiType.Equals(GUIType.MainMenu);
        }

        public void LoadPlayMode()
        {
            Timer.Reset();
            GameStateManager.SetGameState(GameState.Playing);
        }

        public void LoadEditMode()
        {
            Timer.Reset();
            GameStateManager.SetGameState(GameState.Editing);
        }

        public void ResetLevel()
        {
            Timer.Reset();
            StartCoroutine(GameStateManager.ResetLevel());
        }

        public void SkipDialogue()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Dialogue))
            {
                GameStateManager.SetGameState(GameState.Playing);
            }
        }

        public void LoadSettingsMenu()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Playing) 
            || GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                GameStateManager.SetGameState(GameState.Paused);
            }
            else
            {
                StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.SettingsMenu));
            }
        }

        public void LoadNextLevel()
        {
            Timer.Reset();
            GameStateManager.LoadNextLevel();
        }

        public void GoBackFromSettingsMenu()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Paused))
            {
                GameStateManager.SetGameState(GameStateManager.GetPreviousGameState());
            }
            else
            {
                StartCoroutine(GUIManager.AsyncSetActiveGUI(GUIType.MainMenu));
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
