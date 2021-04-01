using UnityEngine;
using Core;

namespace SimpleMachine
{
    public class DraggingController : MonoBehaviour
    {
        private Collider2D collider2D;
        private SpriteRenderer spriteRenderer;
        private Color defaultColor;
        private Vector2 lastValidPosition;

        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            defaultColor = spriteRenderer.color;
        }

        private Vector2 GetMousePositionInWorldCoordinates()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnMouseDown()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                return;
            }
            lastValidPosition = transform.position;
            collider2D.isTrigger = true;
            transform.position = GetMousePositionInWorldCoordinates();
        }

        public void OnMouseDrag()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                return;
            }
            transform.position = GetMousePositionInWorldCoordinates();
            if (ObjectBeingPlacedHasCollided())
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.green;
            }
        }

        public void OnMouseUp()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                return;
            }
            if (ObjectBeingPlacedHasCollided())
            {
                transform.position = lastValidPosition;
            }
            else
            {
                collider2D.isTrigger = false;
            }
            spriteRenderer.color = defaultColor;
        }

        public void OnMouseOver()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                return;
            }
            bool playerRightClickedThisObject = Input.GetMouseButtonDown(1);
            if (playerRightClickedThisObject)
            {
                Destroy(gameObject);
            }
        }

        private bool ObjectBeingPlacedHasCollided()
        {
            return collider2D.IsTouchingLayers(1);
        }
    }
}
