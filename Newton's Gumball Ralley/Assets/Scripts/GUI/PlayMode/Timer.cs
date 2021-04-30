using UnityEngine;
using TMPro;

namespace GUI.PlayMode
{
    public class Timer : MonoBehaviour
    {
        private TextMeshProUGUI timerText;

        private void Awake()
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
            timerText.text = $"Time\n{Mathf.RoundToInt(LevelTimer.Timer.CurrentTime)}";
        }
    }
}
