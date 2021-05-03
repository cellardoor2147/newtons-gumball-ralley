using TMPro;
using Core.Levels;
using UnityEngine;

namespace GUI.EditMode
{
    public class TimeConstraintDisplayer : MonoBehaviour
    {
        private static TextMeshProUGUI timeConstraintText;
        private static float timeConstraint;

        private void Update()
        {
            timeConstraintText = GetComponent<TextMeshProUGUI>();
            timeConstraint = LevelManager.GetCurrentLevelTimeConstraint();
            timeConstraintText.text = $"Time Constraint\n{timeConstraint}";
        }
    }
}