using TMPro;
using Core;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
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
            scrapRemainingText.text = ScrapRemaining.ToString();
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