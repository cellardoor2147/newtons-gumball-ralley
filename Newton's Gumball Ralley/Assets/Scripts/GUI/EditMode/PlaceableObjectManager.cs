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
        private bool shouldDisableDragging = false;
        private bool isDisabledBasedOnCurrentLevel = false;
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

        public void ToggleBasedOnAvailableScrap()
        {
            Image objectImage = gameObject.GetComponent<Image>();

            if (ObjectMetaData != null
                && ObjectMetaData.amountOfScrap > ScrapManager.ScrapRemaining
                && !isDisabledBasedOnCurrentLevel)
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

        public void ToggleBasedOnCurrentLevel()
        {
            Image objectImage = gameObject.GetComponent<Image>();
            int worldIndex = LevelManager.GetCurrentWorldIndex();
            int levelIndex = LevelManager.GetCurrentLevelIndex();

            if (ObjectMetaData != null && worldIndex == 1)
            {
                bool isLevel1AndObjectShouldBeDisabled =
                    levelIndex == 1
                    && (ObjectMetaData.name.Equals("InclinePlane1")
                    || ObjectMetaData.name.Equals("InclinePlane1Inverted"));
                if (ObjectMetaData.name.Equals("InclinePlane2")
                    || isLevel1AndObjectShouldBeDisabled)
                {
                    objectImage.color = Color.red;
                    shouldDisableDragging = true;
                    isDisabledBasedOnCurrentLevel = true;
                }
                else if (objectImage.color == Color.red)
                {
                    RevertColorAndEnableDragging(objectImage);
                }
            }
            else if (objectImage.color == Color.red)
            {
                RevertColorAndEnableDragging(objectImage);
            }
        }

        private void RevertColorAndEnableDragging(Image objectImage)
        {
            objectImage.color = DefaultColor;
            shouldDisableDragging = false;
            isDisabledBasedOnCurrentLevel = false;
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
