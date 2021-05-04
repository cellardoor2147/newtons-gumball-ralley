using TMPro;
using Core.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.EditMode
{
    public class TimeConstraintDisplayer : MonoBehaviour
    {
        private static readonly string TIME_CONSTRAINT_BACKGROUND_KEY =
            "Time Constraint Background";

        private TextMeshProUGUI timeConstraintText;
        private static float timeConstraint;

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
        }

        private void Update()
        {           
            timeConstraint = LevelManager.GetCurrentLevelTimeConstraint();
            timeConstraintText.text =
                $"Finish in {timeConstraint} Seconds or Less to Get a Star!";
        }
    }
}