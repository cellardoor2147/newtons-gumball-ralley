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

        public void SetHintTextBasedOnCurrentLevel()
        {
            int worldIndex = LevelManager.GetCurrentWorldIndex();
            int levelIndex = LevelManager.GetCurrentLevelIndex();

            switch (worldIndex)
            {
                case 1:
                    switch (levelIndex)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
                case 2:
                    switch (levelIndex)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
                case 3:
                    switch (levelIndex)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
                case 4:
                    switch (levelIndex)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
                case 5:
                    switch (levelIndex)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
                case 6:
                    switch (levelIndex)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
            }
        }
    }
}