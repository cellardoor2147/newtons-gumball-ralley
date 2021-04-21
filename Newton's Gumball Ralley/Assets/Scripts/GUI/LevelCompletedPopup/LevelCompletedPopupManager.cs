using System.Collections;
using UnityEngine;
using Core;

public class LevelCompletedPopupManager : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(RecordProgressWhenLevelComplete());
    }

    private IEnumerator RecordProgressWhenLevelComplete()
    {
        yield return new WaitUntil(() => GameStateManager.GetGameState().Equals(GameState.LevelCompleted));
        PlayerProgressManager.RecordProgressForCurrentLevel();
    }
}
