using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GUI.EditMode
{
    public enum PlaceableObjectType
    {
        InclinePlane = 0,
        Screw = 1,
        Lever = 2,
        Pulley = 3,
        Wheel = 4,
        Wedge = 5,
        Miscellaneous = 6
    }

    public class EditModeManager : MonoBehaviour
    {
        private static readonly string PLACEABLE_OBJECTS_MENU_KEY =
            "Placeable Objects Menu";
        private static readonly string TABS_CONTAINER_KEY = "Tabs Container";
        private static readonly string CONTENT_CONTAINER_KEY = "Content Container";

        private static EditModeManager instance;

        private List<TabController> tabControllers;
        private List<ContentController> contentControllers;
        private RectTransform placeableObjectsMenuTransform;
        private float placeableObjectsMenuMaxYPosition;
        private float placeableObjectsMenuMinYPosition;
        private bool isLowering;
        private bool isRaising;

        private EditModeManager() { } // Prevents instantiation outside of this class

        private void Awake()
        {
            SetInstance();
            tabControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(TABS_CONTAINER_KEY)
                .GetComponentsInChildren<TabController>(true).ToList();
            contentControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(CONTENT_CONTAINER_KEY)
                .GetComponentsInChildren<ContentController>(true).ToList();
            placeableObjectsMenuTransform = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .GetComponent<RectTransform>();
            SetPlaceableObjectsMenuPositionConstraints();
        }

        private void SetInstance()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void SetPlaceableObjectsMenuPositionConstraints()
        {
            float tabsContainerHeight = placeableObjectsMenuTransform
                .Find(TABS_CONTAINER_KEY)
                .GetComponent<RectTransform>()
                .sizeDelta.y;
            placeableObjectsMenuMaxYPosition =
                placeableObjectsMenuTransform.sizeDelta.y / 2;
            placeableObjectsMenuMinYPosition =
                -(placeableObjectsMenuMaxYPosition + tabsContainerHeight);
        }

        private void Start()
        {
            SetActiveTab(PlaceableObjectType.InclinePlane);
        }

        private void SetAllTabsToInactive()
        {
            tabControllers.ForEach(
                tabController => tabController.SetTabColorToInactive()
            );
            contentControllers.ForEach(
                contentController => contentController.gameObject.SetActive(false)
            );
        }

        public static void SetActiveTab(PlaceableObjectType objectType)
        {
            instance.SetAllTabsToInactive();
            TabController tabControllerToActivate = instance.tabControllers.Find(
                tabController => tabController.objectType.Equals(objectType)
            );
            if (tabControllerToActivate != null)
            {
                tabControllerToActivate.SetTabColorToActive();
                instance.ActivateContent(objectType);
            }
            else
            {
                Debug.LogError($"Tried setting invalid placeable object type: {objectType}");
            }
        }

        private void ActivateContent(PlaceableObjectType objectType)
        {
            ContentController contentControllerToActivate = contentControllers.Find(
                contentController => contentController.objectType.Equals(objectType)
            );
            if (contentControllerToActivate != null)
            {
                contentControllerToActivate.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Tried activating invalid placeable object type: {objectType}");
            }
        }

        public static void HideEditModeGUI()
        {
            instance.StartCoroutine(instance.AsyncHideEditModeGUI());
        }

        private IEnumerator AsyncHideEditModeGUI()
        {
            yield return new WaitUntil(() => !(isLowering || isRaising));
            isLowering = true;
            while (
                placeableObjectsMenuTransform.anchoredPosition.y 
                > placeableObjectsMenuMinYPosition
            )
            {
                placeableObjectsMenuTransform.anchoredPosition = new Vector2(
                    placeableObjectsMenuTransform.anchoredPosition.x,
                    placeableObjectsMenuTransform.anchoredPosition.y - (Time.fixedDeltaTime * 1000f)
                );
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            isLowering = false;
            yield return null;
        }

        public static void ShowEditModeGUI()
        {
            instance.StartCoroutine(instance.AsyncShowEditModeGUI());
        }

        private IEnumerator AsyncShowEditModeGUI()
        {
            yield return new WaitUntil(() => !(isLowering || isRaising));
            isRaising = true;
            while (
                placeableObjectsMenuTransform.anchoredPosition.y
                < placeableObjectsMenuMaxYPosition
            )
            {
                placeableObjectsMenuTransform.anchoredPosition = new Vector2(
                    placeableObjectsMenuTransform.anchoredPosition.x,
                    placeableObjectsMenuTransform.anchoredPosition.y + (Time.fixedDeltaTime * 1000f)
                );
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            isRaising = false;
            yield return null;
        }
    }
}
