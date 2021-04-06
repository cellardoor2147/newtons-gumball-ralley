using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace GUI
{
    public class CutsceneLoader : MonoBehaviour
    {
        private readonly static string CUTSCENE_VIEW_KEY = "Cutscene View";
        private readonly static string VIDEO_PLAYER_KEY = "Video Player";

        private RawImage cutsceneTexture;

        private void Start()
        {
            cutsceneTexture = transform.Find(CUTSCENE_VIEW_KEY)
                .GetComponent<RawImage>();
            transform.Find(VIDEO_PLAYER_KEY)
                .GetComponent<VideoPlayer>()
                .loopPointReached += HandleOpeningCutsceneFinished;
        }

        private void HandleOpeningCutsceneFinished(VideoPlayer videoPlayer)
        {
            videoPlayer.Stop();
            StartCoroutine(FadeOutAndLoadMainMenu());
        }

        private IEnumerator FadeOutAndLoadMainMenu()
        {
            while (cutsceneTexture.color.a > 0.0f)
            {
                cutsceneTexture.color = new Color(
                    cutsceneTexture.color.r,
                    cutsceneTexture.color.g,
                    cutsceneTexture.color.b,
                    cutsceneTexture.color.a - Time.deltaTime
                );
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(3);
            GameStateManager.SetGameState(GameState.MainMenu);
            yield return null;
        }
    }
}
