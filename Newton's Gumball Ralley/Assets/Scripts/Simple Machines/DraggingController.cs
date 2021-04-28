using UnityEngine;
using Core;
using SnapCheck;
using GUI.EditMode;
using System.Collections.Generic;
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
        private GameObject rotationArrows;
        private GameObject placedObjectsContainer;
        private PlacedObjectManager objectManager;
        private PlacedObjectMetaData objectMetaData;

        [SerializeField] private PlacedObjectMetaData leverPlatformMetaData;
        [SerializeField] private PlacedObjectMetaData leverFulcrumMetaData;
        [SerializeField] private PlacedObjectMetaData screwMetaData;
        [SerializeField] private PlacedObjectMetaData gear1MetaData;
        [SerializeField] private PlacedObjectMetaData gear2MetaData;
        [SerializeField] private PlacedObjectMetaData gear3MetaData;
        [SerializeField] private PlacedObjectMetaData gearBackgroundMetaData;
        [SerializeField] private PlacedObjectMetaData wheelMetaData;
        [SerializeField] private PlacedObjectMetaData axleMetaData;
        [SerializeField] SoundMetaData ScrewSound;

        private void Awake()
        {
            hasBeenPlaced = false;
            collider2D = GetComponent<Collider2D>();
            colliderIsTriggerByDefault = collider2D.isTrigger;
            spriteRenderer = GetComponent<SpriteRenderer>();
            defaultColor = spriteRenderer.color;
            rigidbody2D = GetComponent<Rigidbody2D>();
            placedObjectsContainer = GameObject.Find(PLACED_OBJECTS_KEY);
            objectManager = GetComponent<PlacedObjectManager>();
            objectMetaData = objectManager.metaData;
            objectManager.SetLastValidPosition(transform.position);
            objectManager.SetLastValidRotation(transform.rotation);
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
            if (objectMetaData.canSnap)
            {
                ToggleSnapLocations(true);
            }
            EditModeManager.HideEditModeGUI();
            objectManager.SetLastValidPosition(transform.position);
            objectManager.SetLastValidRotation(transform.rotation);
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
                if (objectMetaData.Equals(wheelMetaData)) 
                {
                    transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
                spriteRenderer.color = Color.red;
            }
            else
            {
                if (objectMetaData.Equals(wheelMetaData))
                {
                    transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                }
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
                    EditModeManager.ShowEditModeGUI();
                    return;
                }
                objectManager.ResetTransform();
            }
            else
            {
                if (!hasBeenPlaced)
                {
                    ScrapManager.ChangeScrapRemaining(-objectMetaData.amountOfScrap);
                    EditModeManager.ToggleButtonsBasedOnAvailableScrap();
                }
                hasBeenPlaced = true;
                objectManager.SetLastValidPosition(transform.position);
                objectManager.SetLastValidRotation(transform.rotation);
                if (rigidbody2D == null) {
                    AudioManager.instance.PlaySound(ScrewSound.name);
                }
            }
            collider2D.isTrigger = colliderIsTriggerByDefault;
            if (objectMetaData.Equals(wheelMetaData))
            {
                transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
            }
            spriteRenderer.color = defaultColor;
            AddRotationArrows();
            EditModeManager.ShowEditModeGUI();
        }

        public void OnMouseOver()
        {
            if (ShouldPreventDragging())
            {
                if (!gameObject.GetComponent<PlacedObjectManager>().metaData.Equals(gear2MetaData) 
                || !GameStateManager.GetGameState().Equals(GameState.Editing)) 
                {
                    return;
                } 
            }
            bool playerRightClickedThisObject = Input.GetMouseButtonDown(1);
            if (playerRightClickedThisObject)
            {
                RemoveRotationArrows();
                EditModeManager.ShowEditModeGUI();
                if (hasBeenPlaced)
                {
                    ScrapManager.ChangeScrapRemaining(objectMetaData.amountOfScrap);
                    EditModeManager.ToggleButtonsBasedOnAvailableScrap();
                }
                Destroy(gameObject);
            }
        }

        public bool ShouldPreventDragging()
        {
            return (!(GameStateManager.GetGameState().Equals(GameState.Editing)
                && transform.parent.gameObject.name.Equals(PLACED_OBJECTS_KEY)) 
                || (objectMetaData.Equals(gear2MetaData) && hasBeenPlaced)
            );
        }

        private bool ShouldSnap()
        {
            if (GetComponent<SnapChecker>() != null)
            {
                SnapChecker snapChecker = GetComponent<SnapChecker>();
                return snapChecker.ShouldSnap;
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
            else if (objectMetaData.Equals(axleMetaData) || objectMetaData.Equals(gear2MetaData))
            {
                List<GameObject> screwSnapObjects = GetSnapObjects();
                Vector3 desiredSnapLocation = new Vector3();

                desiredSnapLocation = screwSnapObjects[0].transform.position - new Vector3(0, 0, 0);
                // 0f offset is required to get screw in correct place

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

            if (objectMetaData.Equals(leverFulcrumMetaData) || objectMetaData.Equals(gearBackgroundMetaData))
            {
                foreach (Transform placedObject in objectContainer.transform)
                {
                    PlacedObjectManager placedObjectManager = placedObject.GetComponent<PlacedObjectManager>();
                    if (placedObjectManager != null && (placedObjectManager.metaData.Equals(leverPlatformMetaData)
                                                        || placedObjectManager.metaData.Equals(gear2MetaData))) 
                    {
                        placedObject.GetChild(0).gameObject.SetActive(activeState);
                    }
                }
            }
            else if (objectMetaData.Equals(axleMetaData))  {
                foreach (Transform placedObject in objectContainer.transform)
                {
                    PlacedObjectManager placedObjectManager = placedObject.GetComponent<PlacedObjectManager>();
                    if (placedObjectManager != null && (placedObjectManager.metaData.Equals(gear1MetaData)
                                                        || placedObjectManager.metaData.Equals(gear3MetaData)
                                                        || placedObjectManager.metaData.Equals(wheelMetaData))) {
                        placedObject.GetChild(0).gameObject.SetActive(activeState);
                    }
                }
            }
        }

        private bool ShouldPreventObjectFromBeingPlaced()
        {
            bool objectIsScrew = rigidbody2D == null; 
            bool objectHasCollided;
            if (!objectMetaData.Equals(gear2MetaData)) 
            {
                objectHasCollided = collider2D.IsTouchingLayers(1) || collider2D.IsTouchingLayers(LayerMask.GetMask("Ball"));
            }
            else 
            {
                objectHasCollided = GetComponent<SnapChecker>().SnapPointHolder == null;
            }
            return !objectIsScrew && objectHasCollided;
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
            objectManager.SetLastValidRotation(transform.rotation);
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
