using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core;
using SimpleMachine;

namespace GUI.EditMode
{
    public class PlaceableObjectManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private static readonly string PLACEABLE_OBJECT_IMAGE_PREFIX =
            "Placeable Object Image";

        [SerializeField] GameObject placeableObjectPrefab;

        private GameObject placeableObjectsContainer;
        private Color placeableObjectDefaultColor;
        private SpriteRenderer objectBeingPlacedSpriteRenderer;
        private GameObject objectBeingPlaced;
        private Collider2D objectBeingPlacedCollider;

        private void Awake()
        {
            placeableObjectsContainer =
                GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
            placeableObjectDefaultColor =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().color;
            transform.Find(PLACEABLE_OBJECT_IMAGE_PREFIX).GetComponent<Image>().sprite =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite;
        }

        private Vector2 GetMousePositionInWorldCoordinates()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            objectBeingPlaced =
                Instantiate(placeableObjectPrefab, placeableObjectsContainer.transform);
            objectBeingPlacedSpriteRenderer = objectBeingPlaced.GetComponent<SpriteRenderer>();
            objectBeingPlacedCollider = objectBeingPlaced.GetComponent<Collider2D>();
            objectBeingPlacedCollider.isTrigger = true;
            objectBeingPlaced.transform.position = GetMousePositionInWorldCoordinates();
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            objectBeingPlaced.transform.position = GetMousePositionInWorldCoordinates();
            if (ObjectBeingPlacedHasCollided())
            {
                objectBeingPlacedSpriteRenderer.color = Color.red;
            }
            else
            {
                objectBeingPlacedSpriteRenderer.color = Color.green;
            }
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if (ObjectBeingPlacedHasCollided())
            {
                Destroy(objectBeingPlaced);
                objectBeingPlaced = null;
            }
            else
            {
                objectBeingPlacedSpriteRenderer.color = placeableObjectDefaultColor;
                objectBeingPlacedCollider.isTrigger = false;
                DraggingController objectBeingPlacedDraggingController =
                    objectBeingPlaced.GetComponent<DraggingController>();
                objectBeingPlacedDraggingController.lastValidPosition =
                    objectBeingPlaced.transform.position;
                objectBeingPlacedDraggingController.lastValidRotation =
                    objectBeingPlaced.transform.rotation;
            }
        }

        private bool ObjectBeingPlacedHasCollided()
        {
            return objectBeingPlacedCollider.IsTouchingLayers(1);
        }
    }
}
