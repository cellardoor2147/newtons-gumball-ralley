using UnityEngine;
using Core;
using GUI.EditMode;
using Audio;

namespace SimpleMachine
{
    public class DraggingController : MonoBehaviour
    {
        private static readonly string PLACED_OBJECTS_KEY = "Placed Objects";
        private static readonly string SIMPLE_MACHINE_TAG = "SimpleMachine";
        private static readonly string BALL_TAG = "Player";

        [SerializeField] private GameObject rotationArrowsPrefab;

        private bool hasBeenPlaced;
        private Collider2D collider2D;
        private bool colliderIsTriggerByDefault;
        private SpriteRenderer spriteRenderer;
        private Color defaultColor;
        private Rigidbody2D rigidbody2D;
        private Vector2 lastValidPosition;
        private Quaternion lastValidRotation;
        private GameObject rotationArrows;
        private GameObject placedObjectsContainer;
        private PlacedObjectManager objectManager;
        private PlacedObjectMetaData objectMetaData;
        
        [SerializeField] private PlacedObjectMetaData leverPlatformMetaData;
        [SerializeField] private PlacedObjectMetaData leverFulcrumMetaData;
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
            placedObjectsContainer = GameObject.Find(PLACED_OBJECTS_KEY);
            objectManager = GetComponent<PlacedObjectManager>();
            objectMetaData = objectManager.metaData;
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
            RemoveRotationArrows();
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
                    EditModeManager.ShowEditModeGUI();
                    return;
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
            AddRotationArrows();
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
                RemoveRotationArrows();
                EditModeManager.ShowEditModeGUI();
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

        public void RemoveRotationArrows()
        {
            if (rotationArrows != null)
            {
                Destroy(rotationArrows);
            }
        }

        public void AddRotationArrows()
        {
            if (rotationArrowsPrefab != null)
            {
                rotationArrows = Instantiate(
                    rotationArrowsPrefab,
                    transform.position,
                    Quaternion.identity,
                    placedObjectsContainer.transform
                );
                foreach (
                    RotatingController child in rotationArrows.GetComponentsInChildren<RotatingController>()
                )
                {
                    child.SetObjectToRotate(gameObject);
                }
            }
        }

        public void Rotate(float rotationMagnitude)
        {
            RemoveRotationArrows();
            RotateToNextValidPosition(rotationMagnitude);
            AddRotationArrows();
        }

        private void RotateToNextValidPosition(float rotationMagnitude)
        {
            RaycastHit2D[] hits;
            float totalRotationMagnitude = 0;
            do
            {
                totalRotationMagnitude +=
                    GetNextPotentiallyValidRotation(rotationMagnitude);
                hits = Physics2D.BoxCastAll(
                    transform.position,
                    ((BoxCollider2D)collider2D).size,
                    transform.rotation.eulerAngles.z + totalRotationMagnitude,
                    Vector2.zero
                );
            } while (
                HitsContainCollisionOtherThanSelf(hits)
                && totalRotationMagnitude < 360f
                && totalRotationMagnitude > -360f
            );
            transform.Rotate(new Vector3(0f, 0f, totalRotationMagnitude));
            lastValidRotation = transform.rotation;
        }
        
        private float GetNextPotentiallyValidRotation(float rotationMagnitude)
        {
            bool rotationWouldMakeGameObjectHorizontalOrVertical =
                (Mathf.RoundToInt(transform.rotation.eulerAngles.z + rotationMagnitude) % 90) == 0;
            if (rotationWouldMakeGameObjectHorizontalOrVertical)
            {
                rotationMagnitude *= 2;
            }
            return rotationMagnitude;
        }

        private bool HitsContainCollisionOtherThanSelf(RaycastHit2D[] hits)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (
                    hit.collider.gameObject != gameObject 
                    && (hit.collider.gameObject.CompareTag(SIMPLE_MACHINE_TAG)
                    || hit.collider.gameObject.CompareTag(BALL_TAG))
                )
                {
                    return true;
                }
            }
            return false;
        }
    }
}
