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
        private string objectName;

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
            objectName = objectManager.metaData.objectName;
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
            if (CanSnap())
            {               
                RenderSnapLocations();
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

            DisableSnapLocations();

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

        private bool CanSnap()
        {
            if (objectManager == null)
                return false;

            switch (objectName)
            {
                case "LeverFulcrum":
                    return true;
                default:
                    return false;
            }
        }

        private bool ShouldSnap()
        {
            switch (objectName)
            {
                case "LeverFulcrum":
                    SnapChecker fulcrumSnapChecker = GetComponent<SnapChecker>();
                    return fulcrumSnapChecker.ShouldSnap;
                default:
                    return false;
            }
        }

        private void SnapToNearestLocation()
        {
            switch (objectName)
            {
                case "LeverFulcrum":
                    Transform closestSnapPoint = GetNearestSnapLocation();
                    transform.position = closestSnapPoint.position - new Vector3(0, 0.091f, 0);
                    break;
                default:
                    break;
            }
        }

        private Transform GetNearestSnapLocation()
        {
            List<GameObject> fulcrumSnapObjects = GetSnapObjects();
            float closestDistance = Mathf.Infinity;
            Transform closestSnapPoint = null;
            Vector2 mousePosition = GetMousePositionInWorldCoordinates();
            foreach (GameObject fulcrumSnapObject in fulcrumSnapObjects)
            {
                float distanceToSnapPoint = Vector2.Distance(fulcrumSnapObject.transform.position, mousePosition);
                if (distanceToSnapPoint < closestDistance)
                {
                    closestDistance = distanceToSnapPoint;
                    closestSnapPoint = fulcrumSnapObject.transform;
                }
            }

            return closestSnapPoint;
        }

        private void RenderSnapLocations()
        {
            List<GameObject> placedObjects = GetPlacedObjects();

            ToggleSnapLocations(placedObjects, true);
        }

        private void DisableSnapLocations()
        {
            List<GameObject> placedObjects = GetPlacedObjects();

            ToggleSnapLocations(placedObjects, false);
        }

        private List<GameObject> GetPlacedObjects()
        {
            GameObject objectContainer = GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
            List<GameObject> placedObjects = new List<GameObject>();
            foreach (Transform placedObject in objectContainer.transform)
            {
                if (placedObject.GetComponent<PlacedObjectManager>())
                    placedObjects.Add(placedObject.gameObject);
            }

            return placedObjects;
        }

        private List<GameObject> GetSnapObjects()
        {
            SnapChecker snapChecker = GetComponent<SnapChecker>();
            GameObject snapPointHolder = snapChecker.SnapPointHolder;
            List<GameObject> snapPoints = new List<GameObject>();
            foreach (Transform snapPoint in snapPointHolder.transform)
            {
                snapPoints.Add(snapPoint.gameObject);
            }

            return snapPoints;
        }

        private void ToggleSnapLocations(List<GameObject> placedObjects, bool activeState)
        {
            switch (objectName)
            {
                case "LeverFulcrum":
                    List<GameObject> existingLeverPlatforms =
                        placedObjects.FindAll(placedObject =>
                        placedObject.GetComponent<PlacedObjectManager>().metaData.objectName.Equals("LeverPlatform"));
                    existingLeverPlatforms.ForEach(existingLeverPlatform =>
                        existingLeverPlatform.transform.GetChild(0).gameObject.SetActive(activeState));
                    break;
                default:
                    break;
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
