using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Levels;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace GUI.PlayMode
{
    public class Timer : MonoBehaviour
    {
        private static readonly string TIME_COUNTER_BACKGROUND_KEY =
            "Time Counter Background";

        public string time;

        [SerializeField] private LocalizeStringEvent stringRef;

        private TextMeshProUGUI timerText;

        private void Awake()
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (LevelManager.CurrentLevelShouldHideAllScrapAndTimeGUI())
            {
                transform
                    .parent
                    .Find(TIME_COUNTER_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.clear;
                timerText.color = Color.clear;
            }
            else
            {
                transform
                    .parent
                    .Find(TIME_COUNTER_BACKGROUND_KEY)
                    .GetComponent<Image>()
                    .color = Color.white;
                timerText.color = Color.black;
            }
            stringRef.StringReference.RefreshString();
        }
        
        void Update()
        {
            time = Mathf.RoundToInt(LevelTimer.Timer.CurrentTime).ToString();
            stringRef.StringReference.RefreshString();
        }
    }
}
