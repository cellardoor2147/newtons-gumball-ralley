using TMPro;
using Core.Levels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;


namespace GUI.EditMode
{
    public class TimeConstraintDisplayer : MonoBehaviour
    {
        private static readonly string TIME_CONSTRAINT_BACKGROUND_KEY =
            "Time Constraint Background";

        [SerializeField] private LocalizeStringEvent stringRef;

        private TextMeshProUGUI timeConstraintText;
        public string timeConstraint;

        private void Awake()
        {
            timeConstraintText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (LevelManager.CurrentLevelShouldHideStarConstraints())
            {
                transform
                    .parent
                    .Find(TIME_CONSTRAINT_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.clear;
                timeConstraintText.color = Color.clear;
            }
            else
            {
                transform
                    .parent
                    .Find(TIME_CONSTRAINT_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.white;
                timeConstraintText.color = Color.black;
            }
            stringRef.StringReference.RefreshString();
        }

        private void Update()
        {
            timeConstraint = LevelManager.GetCurrentLevelTimeConstraint().ToString();
            stringRef.StringReference.RefreshString();
        }
    }
}