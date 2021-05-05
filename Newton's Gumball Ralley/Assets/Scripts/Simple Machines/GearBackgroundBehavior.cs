using UnityEngine;
using Core;
using System.Collections;

namespace SimpleMachine 
{
    public class GearBackgroundBehavior : MonoBehaviour
    {
        private CircleCollider2D collider;
        private void Awake()
        {
            collider = transform.GetChild(0).GetComponent<CircleCollider2D>();
        }
        private void Update()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Editing) && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            else if (!GameStateManager.GetGameState().Equals(GameState.Editing) 
                && gameObject.activeSelf 
                && collider.IsTouchingLayers(-1))
            {
                gameObject.SetActive(false);
            }
        }
    }
}

