using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core;
using SimpleMachine;

namespace GUI.EditMode
{
    public class PlaceableObjectManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float maxSpriteHeightInToolbar;
        [SerializeField] private GameObject placeableObjectPrefab;

        private GameObject placeableObjectsContainer;
        public PlacedObjectMetaData ObjectMetaData { get; private set; }
        public Color DefaultColor { get; private set; }
        private Image objectImage;
        private DraggingController objectBeingPlacedDraggingController;

        private void Awake()
        {
            GetComponent<Image>().sprite =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite;
            GetComponent<RectTransform>().sizeDelta = GetSizeDelta();
            placeableObjectsContainer =
                GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
            ObjectMetaData = placeableObjectPrefab.GetComponent<PlacedObjectManager>().metaData;
            objectImage = GetComponent<Image>();
            DefaultColor = objectImage.color;
        }

        private Vector2 GetSizeDelta()
        {
            float spriteWidth =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite.rect.size.x;
            float spriteHeight =
                placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite.rect.size.y;
            if (spriteHeight > maxSpriteHeightInToolbar)
            {
                spriteWidth *= maxSpriteHeightInToolbar / spriteHeight;
                spriteHeight = maxSpriteHeightInToolbar;
            }
            return new Vector2(spriteWidth, spriteHeight);
        }
        
        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (placeableObjectsContainer == null)
            {
                placeableObjectsContainer = GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
            }
            GameObject objectBeingPlaced =
                Instantiate(placeableObjectPrefab, placeableObjectsContainer.transform);
            ScrapManager.ChangeScrapRemaining(-ObjectMetaData.amountOfScrap);
            ScrapManager.ToggleButtonsDependingOnCost();
            objectBeingPlacedDraggingController =
                objectBeingPlaced.GetComponent<DraggingController>();
            objectBeingPlacedDraggingController.OnMouseDown();
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            if (objectBeingPlacedDraggingController == null)
            {
                return;
            }
            objectBeingPlacedDraggingController.OnMouseDrag();
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if (objectBeingPlacedDraggingController == null)
            {
                return;
            }
            objectBeingPlacedDraggingController.OnMouseUp();
        }
    }
}
