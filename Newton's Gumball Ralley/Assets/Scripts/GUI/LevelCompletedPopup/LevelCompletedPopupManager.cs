using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Audio;
using Core;

namespace GUI.LevelCompletedPopup
{
    public class LevelCompletedPopupManager : MonoBehaviour
    {
        private static readonly string STARS_KEY = "Stars";
        private static readonly string LEVEL_BEAT_STAR_KEY = "Level Beat Star Container";
        private static readonly string TIME_STAR_KEY = "Time Star Container";
        private static readonly string SCRAP_STAR_KEY = "Scrap Star Container";
        private static readonly string IMAGE_KEY = "Image";

        [SerializeField] private SoundMetaData starGetSound;

        private Image levelBeatStar;
        private Image timeStar;
        private Image scrapStar;

        private void Awake()
        {
            levelBeatStar = transform
                .Find(STARS_KEY)
                .Find(LEVEL_BEAT_STAR_KEY)
                .Find(IMAGE_KEY)
                .GetComponent<Image>();
            timeStar = transform
                .Find(STARS_KEY)
                .Find(TIME_STAR_KEY)
                .Find(IMAGE_KEY)
                .GetComponent<Image>();
            scrapStar = transform
                .Find(STARS_KEY)
                .Find(SCRAP_STAR_KEY)
                .Find(IMAGE_KEY)
                .GetComponent<Image>();
        }

        private void OnEnable()
        {
            levelBeatStar.color = Color.black;
            timeStar.color = Color.black;
            scrapStar.color = Color.black;
            StartCoroutine(RecordProgressWhenLevelComplete());
            StartCoroutine(DisplayAchievedStars());
        }

        private IEnumerator RecordProgressWhenLevelComplete()
        {
            yield return new WaitUntil(() => GameStateManager.GetGameState().Equals(GameState.LevelCompleted));
            PlayerProgressManager.RecordProgressForCurrentLevel();
        }

        private IEnumerator DisplayAchievedStars()
        {
            yield return DisplayAchievedStar(levelBeatStar);
            if (StarCalculator.PlayerGotTimeStar())
            {
                yield return DisplayAchievedStar(timeStar);
            }
            if (StarCalculator.PlayerGotScrapStar())
            {
                yield return DisplayAchievedStar(scrapStar);
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
