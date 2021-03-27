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
            GameStateManager.SetGameState(GameState.Playing);
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
