using TMPro;
using Core.Levels;
using UnityEngine;

namespace GUI.EditMode
{
    public class TimeConstraintDisplayer : MonoBehaviour
    {
        private static TextMeshProUGUI timeConstraintText;
        private static float timeConstraint;

        private void Awake()
        {
            timeConstraint = LevelManager.GetCurrentLevelTimeConstraint();
            timeConstraintText = GetComponent<TextMeshProUGUI>();
            timeConstraintText.text = $"Time Constraint\n{timeConstraint}";
        }

        private void Update()
        {
            timeConstraintText.text = $"Time Constraint\n{timeConstraint}";
        }

        public static void UpdateTimeConstraint()
        {
            timeConstraint = LevelManager.GetCurrentLevelTimeConstraint();           
        }
    }
}