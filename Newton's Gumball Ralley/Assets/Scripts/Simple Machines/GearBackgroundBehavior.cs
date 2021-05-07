using UnityEngine;
using Core;
using System.Collections;

namespace SimpleMachine 
{
    public class GearBackgroundBehavior : MonoBehaviour
    {
        private CircleCollider2D collider;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            collider = transform.GetChild(0).GetComponent<CircleCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        private void Update()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                spriteRenderer.enabled = true;
            }
            else if (!GameStateManager.GetGameState().Equals(GameState.Editing) 
                && collider.IsTouchingLayers(-1))
            {
                spriteRenderer.enabled = false;
            }
        }
    }
}

