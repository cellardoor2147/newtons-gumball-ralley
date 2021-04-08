using UnityEngine;
using Core;
using SnapCheck;
using GUI.EditMode;
using System.Collections.Generic;

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
        private PlacedObjectManager objectManager;
        private PlacedObjectMetaData objectMetaData;
        [SerializeField] private PlacedObjectMetaData leverPlatformMetaData;
        [SerializeField] private PlacedObjectMetaData leverFulcrumMetaData;

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
            objectManager = GetComponent<PlacedObjectManager>();
            objectMetaData = objectManager.metaData;
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
            if (objectMetaData.canSnap)
            {
                ToggleSnapLocations(true);
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
            if (ShouldSnap())
            {
                SnapToNearestLocation();
            }
        }

        public void OnMouseUp()
        {
            if (ShouldPreventDragging())
            {
                return;
            }

            ToggleSnapLocations(false);

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

        private bool ShouldSnap()
        {
            if (objectMetaData.Equals(leverFulcrumMetaData))
            {
                SnapChecker fulcrumSnapChecker = GetComponent<SnapChecker>();
                return fulcrumSnapChecker.ShouldSnap;
            }

            return false;
        }

        private void SnapToNearestLocation()
        {
            Vector3 closestSnapPoint = GetNearestSnapLocation();
            transform.position = closestSnapPoint;
        }

        private Vector3 GetNearestSnapLocation()
        {
            if (objectMetaData.Equals(leverFulcrumMetaData))
            {
                List<GameObject> fulcrumSnapObjects = GetSnapObjects();
                float closestDistance = Mathf.Infinity;
                Vector3 desiredSnapLocation = new Vector3();
                Vector2 mousePosition = GetMousePositionInWorldCoordinates();
                foreach (GameObject fulcrumSnapObject in fulcrumSnapObjects)
                {
                    float distanceToSnapPoint = Vector2.Distance(fulcrumSnapObject.transform.position, mousePosition);
                    if (distanceToSnapPoint < closestDistance)
                    {
                        closestDistance = distanceToSnapPoint;
                        Transform closestSnapPoint = fulcrumSnapObject.transform;
                        desiredSnapLocation = closestSnapPoint.position - new Vector3(0, 0.091f, 0);
                        // .091f offset is required to get fulcrum screw in correct place
                    }
                }
                return desiredSnapLocation;
            }
            return Vector3.zero;
        }

        private List<GameObject> GetSnapObjects()
        {
            GameObject snapPointHolder = GetComponent<SnapChecker>().SnapPointHolder;
            List<GameObject> snapPoints = new List<GameObject>();
            foreach (Transform snapPoint in snapPointHolder.transform)
            {
                snapPoints.Add(snapPoint.gameObject);
            }

            return snapPoints;
        }

        private void ToggleSnapLocations(bool activeState)
        {
            GameObject objectContainer = GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);

            if (objectMetaData.Equals(leverFulcrumMetaData))
            {
                foreach (Transform placedObject in objectContainer.transform)
                {
                    PlacedObjectManager placedObjectManager = placedObject.GetComponent<PlacedObjectManager>();
                    if (placedObjectManager != null && placedObjectManager.metaData.Equals(leverPlatformMetaData))
                        placedObject.GetChild(0).gameObject.SetActive(activeState);
                }
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
