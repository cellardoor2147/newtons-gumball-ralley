using TMPro;
using Core.Levels;
using UnityEngine;

namespace GUI.EditMode
{
    public class HintTextController : MonoBehaviour
    {
        private TextMeshProUGUI hintText;

        private void Awake()
        {
            hintText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            hintText.text = LevelManager.GetCurrentLevelHintText();
        }
    }
}