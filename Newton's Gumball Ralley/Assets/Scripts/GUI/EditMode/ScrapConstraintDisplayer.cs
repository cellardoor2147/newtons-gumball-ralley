using TMPro;
using Core.Levels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace GUI.EditMode
{
    public class ScrapConstraintDisplayer : MonoBehaviour
    {
        private static readonly string SCRAP_CONSTRAINT_BACKGROUND_KEY =
            "Scrap Constraint Background";

        private static TextMeshProUGUI scrapConstraintText;
        public string scrapConstraint;
        [SerializeField] private LocalizeStringEvent stringRef;

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
            stringRef.StringReference.RefreshString();
        }

        private void Update()
        {
            scrapConstraint = LevelManager.GetCurrentLevelScrapConstraint().ToString();
            stringRef.StringReference.RefreshString();
        }
    }
}