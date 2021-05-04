using TMPro;
using Core.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.EditMode
{
    public class ScrapConstraintDisplayer : MonoBehaviour
    {
        private static readonly string SCRAP_CONSTRAINT_BACKGROUND_KEY =
            "Scrap Constraint Background";

        private static TextMeshProUGUI scrapConstraintText;
        private static float scrapConstraint;

        private void Awake()
        {
            scrapConstraintText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (LevelManager.CurrentLevelShouldHideStarConstraints())
            {
                transform
                    .parent
                    .Find(SCRAP_CONSTRAINT_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.clear;
                scrapConstraintText.color = Color.clear;
            }
            else
            {
                transform
                    .parent
                    .Find(SCRAP_CONSTRAINT_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.white;
                scrapConstraintText.color = Color.black;
            }
        }

        private void Update()
        {
            scrapConstraint = LevelManager.GetCurrentLevelScrapConstraint();
            scrapConstraintText.text = $"Have {scrapConstraint} Scrap Left to Get a Star!";
        }
    }
}