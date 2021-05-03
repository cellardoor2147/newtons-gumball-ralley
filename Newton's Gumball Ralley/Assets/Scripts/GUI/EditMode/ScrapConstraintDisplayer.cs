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
            scrapConstraintText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            scrapConstraint = LevelManager.GetCurrentLevelScrapConstraint();
            scrapConstraintText.text = $"Scrap Constraint\n{scrapConstraint}";
        }
    }
}