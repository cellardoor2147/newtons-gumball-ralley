using UnityEngine;
using Core;

namespace GUI.SettingsMenu 
{
    public class SettingsMenuManager : MonoBehaviour
    {
        private static readonly string INTERACTABLES_KEY = "Interactables";
        private static readonly string BUTTONS_KEY = "Buttons";
        private static readonly string QUIT_TO_MAIN_KEY = "Quit To Main Menu Button";

        private GameObject quitToMainButton;

        private void Awake()
        {
            quitToMainButton = transform
                .Find(INTERACTABLES_KEY)
                .Find(BUTTONS_KEY)
                .Find(QUIT_TO_MAIN_KEY)
                .gameObject;
        }

        private void OnEnable()
        {
            quitToMainButton.SetActive(GameStateManager.GameSceneSceneIsRunning());
        }
    }
}
