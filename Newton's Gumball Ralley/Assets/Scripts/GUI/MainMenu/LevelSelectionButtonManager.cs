using UnityEngine;
using UnityEngine.UI;
using Core.Levels;
using TMPro;

namespace GUI.MainMenu
{
    public class LevelSelectionButtonManager : MonoBehaviour
    {
        private static readonly string BUTTON_KEY = "Button";
        private static readonly string TEXT_KEY = "Text (TMP)";
        private static readonly string STAR_COUNTER_KEY = "Star Counter";
        private static readonly string STAR_PREFIX = "Star ";

        [SerializeField] private int worldIndex;
        [SerializeField] private int levelIndex;

        private void OnEnable()
        {
            SetButtonText();
            SetClickability();
            SetStars();
        }

        private void SetButtonText()
        {
            transform.Find(BUTTON_KEY)
                .Find(TEXT_KEY)
                .GetComponent<TextMeshProUGUI>()
                .text = string.Format("{0}-{1}", worldIndex, levelIndex);
        }

        private void SetClickability()
        {
            transform.Find(BUTTON_KEY).GetComponent<Button>().interactable =
                PlayerProgressManager.LevelShouldBePlayable(worldIndex, levelIndex);
        }

        private void SetStars()
        {
            int numberOfEarnableStars =
                LevelManager.GetLevelNumberOfEarnableStars(worldIndex, levelIndex);
            int bestStarsEarned =
                PlayerProgressManager.GetBestStarsEarnedForLevel(worldIndex, levelIndex);
            for (int i = 1; i <= 3; i++)
            {
                transform.Find(STAR_COUNTER_KEY)
                    .Find(STAR_PREFIX + i.ToString())
                    .GetComponent<Image>()
                    .color = (i <= bestStarsEarned) ? Color.white : Color.black;
                transform.Find(STAR_COUNTER_KEY)
                    .Find(STAR_PREFIX + i.ToString())
                    .gameObject
                    .SetActive(i <= numberOfEarnableStars);
            }
        }

        public void LoadLevel()
        {
            LevelManager.LoadLevelWithIndices(worldIndex, levelIndex);
        }
    }
}