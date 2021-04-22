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
        private static List<PlaceableObjectManager> placeableObjects;

        private void Awake()
        {
            ScrapRemaining = LevelManager.GetCurrentLevelScrapAllotted();
            scrapRemainingText = GetComponent<TextMeshProUGUI>();
            scrapRemainingText.text = ScrapRemaining.ToString();
            placeableObjects = 
                GameObject.Find(BUTTON_CONTAINER)
                .GetComponentsInChildren<PlaceableObjectManager>(true)
                .ToList();
        }

        private void Update()
        {
            scrapRemainingText.text = ScrapRemaining.ToString();
        }

        public static void ChangeScrapRemaining(float value)
        {
            ScrapRemaining += value;
        }

        public static void ToggleButtonsDependingOnCost()
        {
            foreach (Transform button in EditModeManager.ActiveContentController.transform)
            {
                PlaceableObjectManager placeableObject = button.GetComponent<PlaceableObjectManager>();
                Image objectImage = placeableObject.gameObject.GetComponent<Image>();

                if (placeableObject.ObjectMetaData != null
                    && placeableObject.ObjectMetaData.amountOfScrap > ScrapRemaining)
                {
                    objectImage.color = Color.gray;
                }
                else if (objectImage.color == Color.gray)
                {
                    objectImage.color = placeableObject.DefaultColor;
                }
            }
        }
    }
}