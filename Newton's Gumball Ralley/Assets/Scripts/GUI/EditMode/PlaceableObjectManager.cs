using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core;
using SimpleMachine;
using TMPro;

namespace GUI.EditMode
{
    public class PlaceableObjectManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private static readonly string COST_TEXT_KEY = "Cost Text";

        [Range(0f, 50f)] [SerializeField] private float maxSpriteHeightInToolbar;
        [SerializeField] private GameObject placeableObjectPrefab;

        private GameObject placeableObjectsContainer;
        public PlacedObjectMetaData ObjectMetaData { get; private set; }
        public Color DefaultColor { get; private set; }
        private Image objectImage;
        private bool shouldDisableDragging = false;
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
            SetCostText();
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

        private void SetCostText()
        {
            transform.Find(COST_TEXT_KEY).GetComponent<TextMeshProUGUI>().text =
                ObjectMetaData.amountOfScrap.ToString();
        }

        public void ToggleBasedOnAvailableScrap()
        {
            Image objectImage = gameObject.GetComponent<Image>();

            if (ObjectMetaData != null
                && ObjectMetaData.amountOfScrap > ScrapManager.ScrapRemaining)
            {
                objectImage.color = Color.gray;
                shouldDisableDragging = true;
            }
            else if (objectImage.color == Color.gray)
            {
                objectImage.color = DefaultColor;
                shouldDisableDragging = false;
            }
        }

        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            if (placeableObjectsContainer == null)
            {
                placeableObjectsContainer = GameObject.Find(GameStateManager.PLACED_OBJECTS_KEY);
            }
            if (!shouldDisableDragging)
            {
            GameObject objectBeingPlaced =
                Instantiate(placeableObjectPrefab, placeableObjectsContainer.transform);
            objectBeingPlacedDraggingController =
                objectBeingPlaced.GetComponent<DraggingController>();
            objectBeingPlacedDraggingController.OnMouseDown();
            EditModeManager.ToggleButtonsBasedOnAvailableScrap();
            }           
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            if (objectBeingPlacedDraggingController == null)
            {
                return;
            }
            if (!shouldDisableDragging)
            {
                objectBeingPlacedDraggingController.OnMouseDrag();
            }
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if (objectBeingPlacedDraggingController == null)
            {
                return;
            }
            if (!shouldDisableDragging)
            {
                objectBeingPlacedDraggingController.OnMouseUp();
            }
        }
    }
}
