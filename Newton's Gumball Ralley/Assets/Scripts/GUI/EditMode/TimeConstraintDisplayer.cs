using TMPro;
using Core.Levels;
using UnityEngine;

namespace GUI.EditMode
{
    public class TimeConstraintDisplayer : MonoBehaviour
    {
        private TextMeshProUGUI timeConstraintText;
        private static float timeConstraint;

        private void Awake()
        {
            timeConstraintText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {           
            timeConstraint = LevelManager.GetCurrentLevelTimeConstraint();
            timeConstraintText.text = $"Time Constraint\n{timeConstraint}";
        }
    }
}