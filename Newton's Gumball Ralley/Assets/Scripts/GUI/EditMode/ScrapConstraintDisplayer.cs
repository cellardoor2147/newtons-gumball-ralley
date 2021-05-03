using TMPro;
using Core.Levels;
using UnityEngine;

namespace GUI.EditMode
{
    public class ScrapConstraintDisplayer : MonoBehaviour
    {
        private static TextMeshProUGUI scrapConstraintText;
        private static float scrapConstraint;

        private void Awake()
        {
            scrapConstraint = LevelManager.GetCurrentLevelScrapConstraint();
            scrapConstraintText = GetComponent<TextMeshProUGUI>();
            scrapConstraintText.text = $"Scrap Constraint\n{scrapConstraint}";
        }

        private void Update()
        {
            scrapConstraintText.text = $"Scrap Constraint\n{scrapConstraint}";
        }

        public static void UpdateScrapConstraint()
        {
            scrapConstraint = LevelManager.GetCurrentLevelScrapConstraint();
        }
    }
}