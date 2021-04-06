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
        private DraggingController objectBeingPlacedDraggingController;

        private void Awake()
        {
            transform.Find(PLACEABLE_OBJECT_IMAGE_PREFIX).GetComponent<Image>().sprite =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite;
            placeableObjectsContainer =
                GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
        }
        
        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (placeableObjectsContainer == null)
            {
                placeableObjectsContainer = GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
            }
            GameObject objectBeingPlaced =
                Instantiate(placeableObjectPrefab, placeableObjectsContainer.transform);
            objectBeingPlacedDraggingController =
                objectBeingPlaced.GetComponent<DraggingController>();
            objectBeingPlacedDraggingController.OnMouseDown();
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            objectBeingPlacedDraggingController.OnMouseDrag();
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            objectBeingPlacedDraggingController.OnMouseUp();
        }
    }
}
