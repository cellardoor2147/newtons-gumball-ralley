using UnityEngine;
using UnityEngine.UI;
using Core;
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
            SetStarsColors();
        }

        private void SetButtonText()
        {
            transform.Find(BUTTON_KEY)
                .Find(TEXT_KEY)
                .GetComponent<TextMeshProUGUI>()
                .text = string.Format("{0}-{1}", worldIndex, levelIndex);
        }

        private void SetStarsColors()
        {
            int bestStarsEarned =
                PlayerProgressManager.GetBestStarsEarnedForLevel(worldIndex, levelIndex);
            for (int i = 1; i <= bestStarsEarned; i++)
            {
                transform.Find(STAR_COUNTER_KEY)
                    .Find(STAR_PREFIX + i.ToString())
                    .GetComponent<Image>()
                    .color = Color.white;
            }
        }

        public void LoadLevel()
        {
            LevelManager.LoadLevelWithIndices(worldIndex, levelIndex);
        }
    }
}