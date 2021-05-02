using UnityEngine;
using UnityEngine.UI;
using Core;
using Core.Levels;
using TMPro;

namespace GUI.MainMenu
{
    public class GumballSelectionButtonManager : MonoBehaviour
    {
        private static readonly string BUTTON_KEY = "Button";
        private static readonly string STAR_REQUIREMENTS_KEY = "Star Requirement";
        private static readonly string TEXT_KEY = "Text (TMP)";

        [SerializeField] private int starRequirement;
        [SerializeField] private Sprite unlockedSprite;

        private void OnEnable()
        {
            SetStarRequirementText();
            SetButton();
        }

        private void SetStarRequirementText()
        {
            transform.Find(STAR_REQUIREMENTS_KEY)
                .Find(TEXT_KEY)
                .GetComponent<TextMeshProUGUI>()
                .text = starRequirement.ToString();
        }

        private void SetButton()
        {
            bool shouldBeClickable =
                starRequirement <= PlayerProgressManager.GetTotalEarnedStars();
            transform.Find(BUTTON_KEY)
                .GetComponent<Image>()
                .sprite = unlockedSprite;
            transform.Find(BUTTON_KEY)
                .GetComponent<Button>()
                .interactable = shouldBeClickable;
        }

        public void SetGumballColor()
        {
            GameStateManager.SetGumballSprite(unlockedSprite);
        }
    }
}