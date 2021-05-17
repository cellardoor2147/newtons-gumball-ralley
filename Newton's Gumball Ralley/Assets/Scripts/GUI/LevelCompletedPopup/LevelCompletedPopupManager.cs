using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Audio;
using Core;
using Core.Levels;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace GUI.LevelCompletedPopup
{
    public class LevelCompletedPopupManager : MonoBehaviour
    {
        private static readonly string STARS_KEY = "Stars";
        private static readonly string LEVEL_BEAT_STAR_KEY = "Level Beat Star Container";
        private static readonly string TIME_STAR_KEY = "Time Star Container";
        private static readonly string SCRAP_STAR_KEY = "Scrap Star Container";
        private static readonly string IMAGE_KEY = "Image";
        private static readonly string STAR_CONDITION_KEY = "Condition";
        private static readonly string CONDITION_TEXT_KEY = "Text (TMP)";

        [SerializeField] private SoundMetaData starGetSound;

        private Image levelBeatStarImage;
        private Image timeStarImage;
        private Image scrapStarImage;
        private TextMeshProUGUI timeStarText;
        private TextMeshProUGUI scrapStarText;


        private void Awake()
        {
            levelBeatStarImage = transform
                .Find(STARS_KEY)
                .Find(LEVEL_BEAT_STAR_KEY)
                .Find(IMAGE_KEY)
                .GetComponent<Image>();
            timeStarImage = transform
                .Find(STARS_KEY)
                .Find(TIME_STAR_KEY)
                .Find(IMAGE_KEY)
                .GetComponent<Image>();
            timeStarText = transform
                .Find(STARS_KEY)
                .Find(TIME_STAR_KEY)
                .Find(STAR_CONDITION_KEY)
                .Find(CONDITION_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
            scrapStarImage = transform
                .Find(STARS_KEY)
                .Find(SCRAP_STAR_KEY)
                .Find(IMAGE_KEY)
                .GetComponent<Image>();
            scrapStarText = transform
                .Find(STARS_KEY)
                .Find(SCRAP_STAR_KEY)
                .Find(STAR_CONDITION_KEY)
                .Find(CONDITION_TEXT_KEY)
                .GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            ToggleStarsBasedOnCurrentLevelConstraints();
            StartCoroutine(RecordProgressWhenLevelComplete());
            StartCoroutine(DisplayAchievedStars());
        }

        private void ToggleStarsBasedOnCurrentLevelConstraints()
        {
            levelBeatStarImage.color = Color.black;
            if (LevelManager.CurrentLevelShouldUseTimeConstraint())
            {
                timeStarImage.transform.parent.gameObject.SetActive(true);
                timeStarText.text = $"Under {LevelManager.GetCurrentLevelTimeConstraint()}";
                timeStarImage.color = Color.black;
            }
            else
            {
                timeStarImage.transform.parent.gameObject.SetActive(false);
            }
            if (LevelManager.CurrentLevelShouldUseScrapConstraint())
            {
                scrapStarImage.transform.parent.gameObject.SetActive(true);
                timeStarText.text =
                    $"Over {LevelManager.GetCurrentLevelTimeConstraint()} remaining scrap";
                scrapStarImage.color = Color.black;
            }
            else
            {
                scrapStarImage.transform.parent.gameObject.SetActive(false);
            }
        }

        private IEnumerator RecordProgressWhenLevelComplete()
        {
            yield return new WaitUntil(() => GameStateManager.GetGameState().Equals(GameState.LevelCompleted));
            PlayerProgressManager.RecordProgressForCurrentLevel();
        }

        private IEnumerator DisplayAchievedStars()
        {
            yield return DisplayAchievedStar(levelBeatStarImage);
            if (StarCalculator.PlayerGotTimeStar())
            {
                yield return DisplayAchievedStar(timeStarImage);
            }
            if (StarCalculator.PlayerGotScrapStar())
            {
                yield return DisplayAchievedStar(scrapStarImage);
            }
        }

        private IEnumerator DisplayAchievedStar(Image starImage)
        {
            while (starImage.color.r < 1f)
            {
                starImage.color = new Color(
                    starImage.color.r + Time.deltaTime * 2f,
                    starImage.color.g + Time.deltaTime * 2f,
                    starImage.color.b + Time.deltaTime * 2f,
                    starImage.color.a
                );
                yield return new WaitForEndOfFrame();
            }
            AudioManager.instance.PlaySound(starGetSound.name);
            starImage.GetComponent<RandomImageRotator>().SetShouldRotate(true);
        }
    }
}
