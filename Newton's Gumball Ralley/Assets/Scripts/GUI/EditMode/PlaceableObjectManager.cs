using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.PlacedObjects;
using Core.Levels;
using Core;
using SimpleMachine;
using TMPro;

namespace GUI.EditMode
{
    public class PlaceableObjectManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private static readonly string COST_TEXT_KEY = "Cost Text";

        [Range(0f, 30f)] [SerializeField] private float maxSpriteHeightInToolbar;
        [SerializeField] private GameObject placeableObjectPrefab;
        [SerializeField] private Sprite spriteOverride;

        private GameObject placeableObjectsContainer;
        public PlacedObjectMetaData ObjectMetaData { get; private set; }
        public Color DefaultColor { get; private set; }
        private Image objectImage;
        private bool shouldDisableDragging = false;
        private bool isDisabledBasedOnCurrentLevel = false;
        private bool objectShouldBeEnabled = false;
        private bool objectShouldBeDisabled = false;
        private DraggingController objectBeingPlacedDraggingController;

        private void Awake()
        {
            GetComponent<Image>().sprite =
                spriteOverride != null
                ? spriteOverride
                : placeableObjectPrefab.GetComponent<SpriteRenderer>().sprite;
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
            int worldIndex = LevelManager.GetCurrentWorldIndex();
            int levelIndex = LevelManager.GetCurrentLevelIndex();

            if (ObjectMetaData != null && worldIndex == 1)
            {
                switch (levelIndex)
                {
                    case 1:
                        objectShouldBeEnabled = ObjectMetaData.name.Equals("InclinePlane3");
                        if (!objectShouldBeEnabled)
                        {
                            ToggleObject(this.gameObject, false);
                        }
                        else if (!gameObject.activeSelf)
                        {
                            ToggleObject(this.gameObject, true);
                        }
                        break;
                    case 2:
                        objectShouldBeEnabled =
                            ObjectMetaData.name.Equals("InclinePlane1")
                            || ObjectMetaData.name.Equals("InclinePlane1Inverted");
                        if (!objectShouldBeEnabled)
                        {
                            ToggleObject(this.gameObject, false);
                        }
                        else if (!gameObject.activeSelf)
                        {
                            ToggleObject(this.gameObject, true);
                        }
                        break;
                    default:
                        if (!gameObject.activeSelf)
                        {
                            ToggleObject(this.gameObject, true);
                        }
                        if (ObjectMetaData.name.Equals("InclinePlane2"))
                        {
                            ToggleObject(this.gameObject, false);
                        }
                        break;
                }
            }
            else if (ObjectMetaData != null && worldIndex == 5)
            {
                switch (levelIndex)
                {
                    case 1:
                        objectShouldBeDisabled = ObjectMetaData.name.Equals("Gear1");
                        if (objectShouldBeDisabled)
                        {
                            ToggleObject(this.gameObject, false);
                        }
                        else if (!gameObject.activeSelf)
                        {
                            ToggleObject(this.gameObject, true);
                        }
                        break;
                }
            }
            else if (!gameObject.activeSelf)
            {
                ToggleObject(this.gameObject, true);
            }
        }

        private void ToggleObject(GameObject machine, bool shouldEnable)
        {
            machine.SetActive(shouldEnable);
            shouldDisableDragging = !shouldEnable;
            isDisabledBasedOnCurrentLevel = !shouldEnable;
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
