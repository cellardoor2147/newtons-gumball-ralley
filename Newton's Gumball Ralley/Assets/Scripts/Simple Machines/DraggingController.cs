using UnityEngine;
using Core;
using Screw;

namespace SimpleMachine
{
    public class DraggingController : MonoBehaviour
    {
        public Vector2 lastValidPosition;
        public Quaternion lastValidRotation;

        private Collider2D collider2D;
        private SpriteRenderer spriteRenderer;
        private Color defaultColor;
        private Rigidbody2D rigidbody2D;

        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
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
                ResetTransform();
            }
            else
            {
                lastValidPosition = transform.position;
                lastValidRotation = transform.rotation;
            }
            collider2D.isTrigger = false;
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

        public void ResetTransform()
        {
            bool shouldNotResetTransform = GetComponent<ScrewBehavior>() != null;
            if (shouldNotResetTransform)
            {
                return;
            }
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
