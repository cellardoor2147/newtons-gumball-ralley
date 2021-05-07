using UnityEngine;
using UnityEngine.UI;

namespace GUI.MainMenu
{
    public class LevelSelectionManager : MonoBehaviour
    {
        private static readonly string SHOW_LEVELS_LEFT_BUTTON_KEY =
            "Show Levels Left Button";
        private static readonly string SHOW_LEVELS_RIGHT_BUTTON_KEY =
            "Show Levels Right Button";
        private static readonly string BUTTONS_FOR_WORLDS_PREFIX =
            "Buttons for Worlds ";
        private static readonly string WORLDS_0_1_AND_2_SUFFIX =
            "0, 1 and 2";
        private static readonly string WORLDS_3_AND_4_SUFFIX =
            "3 and 4";
        private static readonly string WORLDS_5_AND_6_SUFFIX =
            "5 and 6";

        private Button showLevelsLeftButton;
        private Button showLevelsRightButton;
        private GameObject buttonsForWorlds01And2;
        private GameObject buttonsForWorlds3And4;
        private GameObject buttonsForWorlds5And6;

        private void Awake()
        {
            showLevelsLeftButton =
                transform.Find(SHOW_LEVELS_LEFT_BUTTON_KEY).gameObject.GetComponent<Button>();
            showLevelsRightButton =
                transform.Find(SHOW_LEVELS_RIGHT_BUTTON_KEY).gameObject.GetComponent<Button>();
            buttonsForWorlds01And2 =
                transform.Find(BUTTONS_FOR_WORLDS_PREFIX  + WORLDS_0_1_AND_2_SUFFIX).gameObject;
            buttonsForWorlds3And4 =
                transform.Find(BUTTONS_FOR_WORLDS_PREFIX + WORLDS_3_AND_4_SUFFIX).gameObject;
            buttonsForWorlds5And6 =
                transform.Find(BUTTONS_FOR_WORLDS_PREFIX + WORLDS_5_AND_6_SUFFIX).gameObject;
        }

        public void ShowLevelsLeft()
        {
            if (buttonsForWorlds3And4.activeSelf)
            {
                SetAllButtonsForWorldsContainersToInactive();
                buttonsForWorlds01And2.SetActive(true);
                showLevelsLeftButton.interactable = false;
            }
            else if (buttonsForWorlds5And6.activeSelf)
            {
                SetAllButtonsForWorldsContainersToInactive();
                buttonsForWorlds3And4.SetActive(true);
                showLevelsRightButton.interactable = true;
            }
        }

        public void ShowLevelsRight()
        {
            if (buttonsForWorlds01And2.activeSelf)
            {
                SetAllButtonsForWorldsContainersToInactive();
                buttonsForWorlds3And4.SetActive(true);
                showLevelsLeftButton.interactable = true;
            }
            else if (buttonsForWorlds3And4.activeSelf)
            {
                SetAllButtonsForWorldsContainersToInactive();
                buttonsForWorlds5And6.SetActive(true);
                showLevelsRightButton.interactable = false;
            }
        }

        private void SetAllButtonsForWorldsContainersToInactive()
        {
            buttonsForWorlds01And2.SetActive(false);
            buttonsForWorlds3And4.SetActive(false);
            buttonsForWorlds5And6.SetActive(false);
        }
    }
}
