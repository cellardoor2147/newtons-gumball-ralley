using TMPro;
using Core.Levels;
using UnityEngine;

namespace GUI.EditMode
{
    public class ScrapManager : MonoBehaviour
    {
        public static readonly string BUTTON_CONTAINER = "Toolbar Content Container";

        public static float ScrapRemaining { get; private set; }
        private static TextMeshProUGUI scrapRemainingText;

        private void Awake()
        {
            ScrapRemaining = LevelManager.GetCurrentLevelScrapAllotted();
            scrapRemainingText = GetComponent<TextMeshProUGUI>();
            scrapRemainingText.text = ScrapRemaining.ToString();
        }

        private void Update()
        {
            scrapRemainingText.text = $"Scrap\n{ScrapRemaining}";
        }

        public static void ChangeScrapRemaining(float value)
        {
            ScrapRemaining += value;
        }

        public static void ResetRemainingScrap()
        {
            ScrapRemaining = LevelManager.GetCurrentLevelScrapAllotted();
        }
    }
}