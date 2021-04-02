using UnityEngine;
using Core;
namespace SimpleMachine
{
    public class DraggingController : MonoBehaviour
    {
        private bool hasBeenPlaced;
        private Collider2D collider2D;
        private bool colliderIsTriggerByDefault;
        private SpriteRenderer spriteRenderer;
        private Color defaultColor;
        private Rigidbody2D rigidbody2D;
        private Vector2 lastValidPosition;
        private Quaternion lastValidRotation;

        private void Awake()
        {
            hasBeenPlaced = false;
            collider2D = GetComponent<Collider2D>();
            colliderIsTriggerByDefault = collider2D.isTrigger;
            spriteRenderer = GetComponent<SpriteRenderer>();
            defaultColor = spriteRenderer.color;
            rigidbody2D = GetComponent<Rigidbody2D>();
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
            lastValidRotation = transform.rotation;
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
            if (ShouldPreventObjectFromBeingPlaced())
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
            if (ShouldPreventObjectFromBeingPlaced())
            {
                if (!hasBeenPlaced)
                {
                    Destroy(gameObject);
                }
                ResetTransform();
            }
            else
            {
                hasBeenPlaced = true;
                lastValidPosition = transform.position;
                lastValidRotation = transform.rotation;
            }
            collider2D.isTrigger = colliderIsTriggerByDefault;
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

        private bool ShouldPreventObjectFromBeingPlaced()
        {
            bool objectIsScrew = rigidbody2D == null;
            bool objectHasCollided = collider2D.IsTouchingLayers(1);
            return !objectIsScrew && objectHasCollided;
        }

        public void ResetTransform()
        {
            transform.position = lastValidPosition;
            transform.rotation = lastValidRotation;
        }

        public void UnfreezeRigidbody()
        {
            bool canNotUnfreezeRigidBody = rigidbody2D == null;
            if (canNotUnfreezeRigidBody)
            {
                return;
            }
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
        }

        public void FreezeRigidbody()
        {
            bool canNotFreezeRigidBody = rigidbody2D == null;
            if (canNotFreezeRigidBody)
            {
                return;
            }
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
