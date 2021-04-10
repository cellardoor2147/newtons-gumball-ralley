using UnityEngine;
using Core;
using TMPro;

namespace GUI.MainMenu
{
    public class LevelSelectionButtonManager : MonoBehaviour
    {
        private static readonly string BUTTON_KEY = "Button";
        private static readonly string TEXT_KEY = "Text (TMP)";

        [SerializeField] private int worldIndex;
        [SerializeField] private int levelIndex;

        private void Awake()
        {
            SetButtonText();
        }

        private void SetButtonText()
        {
            transform.Find(BUTTON_KEY)
                .Find(TEXT_KEY)
                .GetComponent<TextMeshProUGUI>()
                .text = string.Format("{0}-{1}", worldIndex, levelIndex);
        }

        public void LoadLevel()
        {
            LevelManager.LoadLevel(worldIndex, levelIndex);
        }
    }
}