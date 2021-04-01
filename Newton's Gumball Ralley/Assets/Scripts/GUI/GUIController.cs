using Core;
using UnityEngine;

namespace GUI
{
    public class GUIController : MonoBehaviour
    {
        public GUIType guiType;

        public void LoadMainMenu()
        {
            GameStateManager.SetGameState(GameState.MainMenu);
        }

        public void LoadPlayMode()
        {
            // TODO: load dialogue based on current level data,
            // or go straight to play mode if there's no dialogue
            // for the current level
            GameStateManager.SetGameState(GameState.Playing);
        }

        public void LoadEditMode()
        {
            GameStateManager.SetGameState(GameState.Editing);
        }

        public void ResetLevel()
        {
            // TODO: reload level from JSON once serializer is
            // finished
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
            if (GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                GameStateManager.SetGameState(GameState.Paused);
            }
            else
            {
                GUIManager.SetActiveGUI(GUIType.SettingsMenu);
            }
        }

        public void GoBackFromSettingsMenu()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Paused))
            {
                GameStateManager.SetGameState(GameState.Playing);
            }
            else
            {
                GUIManager.SetActiveGUI(GUIType.MainMenu);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
