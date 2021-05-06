﻿using TMPro;
using Core.Levels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace GUI.EditMode
{
    public class ScrapManager : MonoBehaviour
    {
        public static readonly string BUTTON_CONTAINER = "Toolbar Content Container";
        public static readonly string SCRAP_COUNTER_BACKGROUND_KEY = "Scrap Counter Background";

        public static float ScrapRemaining { get; private set; }
        private static TextMeshProUGUI scrapRemainingText;
        [SerializeField] private LocalizeStringEvent stringRef;

        public string scrapRemainingString;

        private void Awake()
        {
            ScrapRemaining = LevelManager.GetCurrentLevelScrapAllotted();
            scrapRemainingText = GetComponent<TextMeshProUGUI>();
            scrapRemainingText.text = ScrapRemaining.ToString();
        }

        private void OnEnable()
        {
            if (LevelManager.CurrentLevelShouldHideAllScrapAndTimeGUI())
            {
                transform
                    .parent
                    .Find(SCRAP_COUNTER_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.clear;
                scrapRemainingText.color = Color.clear;
            }
            else
            {
                transform
                    .parent
                    .Find(SCRAP_COUNTER_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.white;
                scrapRemainingText.color = Color.black;
            }
            stringRef.StringReference.RefreshString();
        }

        private void Update()
        {
            scrapRemainingString = ScrapRemaining.ToString();
            stringRef.StringReference.RefreshString();
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