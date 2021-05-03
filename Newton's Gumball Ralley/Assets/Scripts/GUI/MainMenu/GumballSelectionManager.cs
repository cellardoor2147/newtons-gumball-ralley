using UnityEngine;
using UnityEngine.UI;

namespace GUI.MainMenu
{
    public class GumballSelectionManager : MonoBehaviour
    {
        private static readonly string SHOW_GUMBALLS_LEFT_BUTTON_KEY =
            "Show Gumballs Left Button";
        private static readonly string SHOW_GUMBALLS_RIGHT_BUTTON_KEY =
            "Show Gumballs Right Button";
        private static readonly string BUTTONS_FOR_WORLDS_PREFIX =
            "Buttons for Gumballs ";
        private static readonly string GUMBALLS_1_TO_6_SUFFIX =
            "1-6";
        private static readonly string GUMBALLS_7_TO_12_SUFFIX =
            "7-12";

        private Button showGumballsLeftButton;
        private Button showGumballsRightButton;
        private GameObject buttonsForGumballs1To6;
        private GameObject buttonsForGumballs7To12;

        private void Awake()
        {
            showGumballsLeftButton =
                transform.Find(SHOW_GUMBALLS_LEFT_BUTTON_KEY).gameObject.GetComponent<Button>();
            showGumballsRightButton =
                transform.Find(SHOW_GUMBALLS_RIGHT_BUTTON_KEY).gameObject.GetComponent<Button>();
            buttonsForGumballs1To6 =
                transform.Find(BUTTONS_FOR_WORLDS_PREFIX + GUMBALLS_1_TO_6_SUFFIX).gameObject;
            buttonsForGumballs7To12 =
                transform.Find(BUTTONS_FOR_WORLDS_PREFIX + GUMBALLS_7_TO_12_SUFFIX).gameObject;
        }

        public void ShowGumballsLeft()
        {
            if (buttonsForGumballs7To12.activeSelf)
            {
                SetAllButtonsForGumballsContainersToInactive();
                buttonsForGumballs1To6.SetActive(true);
                showGumballsLeftButton.interactable = false;
                showGumballsRightButton.interactable = true;
            }
        }

        public void ShowGumballsRight()
        {
            if (buttonsForGumballs1To6.activeSelf)
            {
                SetAllButtonsForGumballsContainersToInactive();
                buttonsForGumballs7To12.SetActive(true);
                showGumballsLeftButton.interactable = true;
                showGumballsRightButton.interactable = false;
            }
        }

        private void SetAllButtonsForGumballsContainersToInactive()
        {
            buttonsForGumballs1To6.SetActive(false);
            buttonsForGumballs7To12.SetActive(false);
        }
    }
}
