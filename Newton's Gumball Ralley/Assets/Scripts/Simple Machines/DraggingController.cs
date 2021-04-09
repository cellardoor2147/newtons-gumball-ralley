using UnityEngine;
using Core;
using GUI.EditMode;
using Audio;

namespace SimpleMachine
{
    public class DraggingController : MonoBehaviour
    {
        private static readonly string PLACED_OBJECTS_KEY = "Placed Objects";

        private bool hasBeenPlaced;
        private Collider2D collider2D;
        private bool colliderIsTriggerByDefault;
        private SpriteRenderer spriteRenderer;
        private Color defaultColor;
        private Rigidbody2D rigidbody2D;
        private Vector2 lastValidPosition;
        private Quaternion lastValidRotation;

        [SerializeField] SoundMetaData ScrewSound;

        private void Awake()
        {
            hasBeenPlaced = false;
            collider2D = GetComponent<Collider2D>();
            colliderIsTriggerByDefault = collider2D.isTrigger;
            spriteRenderer = GetComponent<SpriteRenderer>();
            defaultColor = spriteRenderer.color;
            rigidbody2D = GetComponent<Rigidbody2D>();
            lastValidPosition = transform.position;
            lastValidRotation = transform.rotation;
        }

        private void Start()
        {
            if (AudioManager.instance == null)
            {
                Debug.LogError("No audiomanager found");
            }
        }

        private Vector2 GetMousePositionInWorldCoordinates()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnMouseDown()
        {
            if (ShouldPreventDragging())
            {
                return;
            }
            EditModeManager.HideEditModeGUI();
            lastValidPosition = transform.position;
            lastValidRotation = transform.rotation;
            collider2D.isTrigger = true;
            transform.position = GetMousePositionInWorldCoordinates();
        }

        public void OnMouseDrag()
        {
            if (ShouldPreventDragging())
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
            if (ShouldPreventDragging())
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
                if(rigidbody2D == null) {
                    AudioManager.instance.PlaySound(ScrewSound.name);
                } 
            }
            collider2D.isTrigger = colliderIsTriggerByDefault;
            spriteRenderer.color = defaultColor;
            EditModeManager.ShowEditModeGUI();
        }

        public void OnMouseOver()
        {
            if (ShouldPreventDragging())
            {
                return;
            }
            bool playerRightClickedThisObject = Input.GetMouseButtonDown(1);
            if (playerRightClickedThisObject)
            {
                Destroy(gameObject);
            }
        }

        public bool ShouldPreventDragging()
        {
            return !(GameStateManager.GetGameState().Equals(GameState.Editing)
                && transform.parent.gameObject.name.Equals(PLACED_OBJECTS_KEY)
            );
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

        public void GrayOut()
        {
            spriteRenderer.color = Color.gray;
        }

        public void RevertFromGray()
        {
            spriteRenderer.color = defaultColor;
        }
    }
}
