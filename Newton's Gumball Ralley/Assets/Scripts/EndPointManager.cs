﻿using UnityEngine;
using Core;
using Core.Levels;
using LevelTimer;

public class EndPointManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otherObjectCollider)
    {
        if (otherObjectCollider.CompareTag("Player")
            && GameStateManager.GetGameState().Equals(GameState.Playing))
        {
            Timer.Stop();
            LevelManager.SetCurrentLevelIsComplete(true);
            GameStateManager.SetGameState(GameState.Dialogue);
        }
    }
}
