using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Core;
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
        Wedge = 5
    }

    public class EditModeManager : MonoBehaviour
    {
        private static readonly string PLACEABLE_OBJECTS_MENU_KEY =
            "Placeable Objects Menu";
        private static readonly string INACTIVE_TABS_CONTAINER_KEY = "Inactive Tabs Container";
        private static readonly string ACTIVE_TABS_CONTAINER_KEY = "Active Tabs Container";
        private static readonly string TOOLBAR_KEY = "Toolbar";
        private static readonly string TOOLBAR_CONTENT_CONTAINER_KEY = "Toolbar Content Container";

        private static EditModeManager instance;
        public static ContentController ActiveContentController { get; private set; }

        private List<TabController> inactiveTabControllers;
        private List<TabController> activeTabControllers;
        private List<ContentController> contentControllers;
        private ToolbarManager toolbarManager;
        private RectTransform placeableObjectsMenuTransform;
        private float placeableObjectsMenuMaxYPosition;
        private float placeableObjectsMenuMinYPosition;
        private IEnumerator hideGUICoroutine;
        private IEnumerator showGUICoroutine;

        private EditModeManager() { } // Prevents instantiation outside of this class

        private void Awake()
        {
            SetInstance();
            inactiveTabControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(INACTIVE_TABS_CONTAINER_KEY)
                .GetComponentsInChildren<TabController>(true).ToList();
            activeTabControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(ACTIVE_TABS_CONTAINER_KEY)
                .GetComponentsInChildren<TabController>(true).ToList();
            contentControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(TOOLBAR_KEY)
                .Find(TOOLBAR_CONTENT_CONTAINER_KEY)
                .GetComponentsInChildren<ContentController>(true).ToList();
            toolbarManager = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(TOOLBAR_KEY)
                .Find(TOOLBAR_CONTENT_CONTAINER_KEY)
                .GetComponent<ToolbarManager>();
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

        private void OnEnable()
        {
            StartCoroutine(AsyncToggleButtonsBasedOnAvailableScrap());
        }

        private void SetPlaceableObjectsMenuPositionConstraints()
        {
            float tabsContainerHeight = placeableObjectsMenuTransform
                .Find(INACTIVE_TABS_CONTAINER_KEY)
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

        public static IEnumerator AsyncSetActiveTab(PlaceableObjectType objectType)
        {
            yield return new WaitUntil(() => ActiveContentController != null);
            SetActiveTab(objectType);
        }

        public static void SetActiveTab(PlaceableObjectType objectType)
        {
            foreach (TabController inactiveTab in instance.inactiveTabControllers)
            {
                if (inactiveTab.objectType.Equals(objectType))
                {
                    inactiveTab.Hide();
                }
                else
                {
                    inactiveTab.Show();
                }
            }
            foreach (TabController activeTab in instance.activeTabControllers)
            {
                if (activeTab.objectType.Equals(objectType))
                {
                    activeTab.Show();
                }
                else
                {
                    activeTab.Hide();
                }
            }
            instance.ActivateContent(objectType);
        }

        public static IEnumerator DisableFutureTabs()
        {
            yield return new WaitUntil(() => instance != null);
            EnableAllTabs();
            int worldIndex = LevelManager.GetCurrentWorldIndex();
            List<PlaceableObjectType> desiredObjectTypes = new List<PlaceableObjectType>();

            switch (worldIndex)
            {
                case 1:
                    desiredObjectTypes.Add(PlaceableObjectType.InclinePlane);
                    foreach (TabController inactiveTab in instance.inactiveTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, inactiveTab, false);
                    }
                    foreach (TabController activeTab in instance.activeTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, activeTab, true);
                    }
                    break;
                case 2:
                    desiredObjectTypes.Add(PlaceableObjectType.InclinePlane);
                    desiredObjectTypes.Add(PlaceableObjectType.Screw);
                    foreach (TabController inactiveTab in instance.inactiveTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, inactiveTab, false);
                    }
                    foreach (TabController activeTab in instance.activeTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, activeTab, true);
                    }
                    break;
                case 3:
                    desiredObjectTypes.Add(PlaceableObjectType.InclinePlane);
                    desiredObjectTypes.Add(PlaceableObjectType.Screw);
                    desiredObjectTypes.Add(PlaceableObjectType.Lever);
                    foreach (TabController inactiveTab in instance.inactiveTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, inactiveTab, false);
                    }
                    foreach (TabController activeTab in instance.activeTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, activeTab, true);
                    }
                    break;
                case 4:
                    desiredObjectTypes.Add(PlaceableObjectType.InclinePlane);
                    desiredObjectTypes.Add(PlaceableObjectType.Screw);
                    desiredObjectTypes.Add(PlaceableObjectType.Lever);
                    desiredObjectTypes.Add(PlaceableObjectType.Wedge);
                    foreach (TabController inactiveTab in instance.inactiveTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, inactiveTab, false);
                    }
                    foreach (TabController activeTab in instance.activeTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, activeTab, true);
                    }
                    break;
                case 5:
                    desiredObjectTypes.Add(PlaceableObjectType.InclinePlane);
                    desiredObjectTypes.Add(PlaceableObjectType.Screw);
                    desiredObjectTypes.Add(PlaceableObjectType.Lever);
                    desiredObjectTypes.Add(PlaceableObjectType.Wedge);
                    desiredObjectTypes.Add(PlaceableObjectType.Wheel);
                    foreach (TabController inactiveTab in instance.inactiveTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, inactiveTab, false);
                    }
                    foreach (TabController activeTab in instance.activeTabControllers)
                    {
                        EnableDesiredTabs(desiredObjectTypes, activeTab, true);
                    }
                    break;
            }
        }

        private static void EnableDesiredTabs(List<PlaceableObjectType> desiredObjectTypes, TabController tab, bool isActiveTab)
        {
            bool shouldNotBeDisabled = desiredObjectTypes.Contains(tab.objectType);
            if (!shouldNotBeDisabled && !isActiveTab)
            {
                tab.ReddenAndDisable();
            }
            else if (!shouldNotBeDisabled && isActiveTab)
            {
                tab.Disable();
            }

        }

        public static void EnableAllTabs()
        {
            foreach (TabController inactiveTab in instance.inactiveTabControllers)
            {                
                inactiveTab.RevertToOriginalColorsAndEnable();
            }
            foreach (TabController activeTab in instance.activeTabControllers)
            {
                activeTab.Enable();
            }
        }

        private void ActivateContent(PlaceableObjectType objectType)
        {
            foreach (ContentController contentController in instance.contentControllers)
            {
                bool contentControllerGameObjectShouldBeActivated =
                    contentController.objectType.Equals(objectType);
                contentController.gameObject.SetActive(
                    contentControllerGameObjectShouldBeActivated
                );
                if (contentControllerGameObjectShouldBeActivated)
                {
                    ActiveContentController = contentController;
                    ToggleButtonsBasedOnCurrentLevel();
                    ToggleButtonsBasedOnAvailableScrap();
                    toolbarManager.SetContent(contentController.gameObject);
                }
            }
        }

        private IEnumerator AsyncToggleButtonsBasedOnAvailableScrap()
        {
            yield return new WaitUntil(() => ActiveContentController != null);
            ToggleButtonsBasedOnAvailableScrap();
        }

        public static void ToggleButtonsBasedOnAvailableScrap()
        {
            foreach (Transform button in ActiveContentController.transform)
            {
                PlaceableObjectManager placeableObject = button.GetComponent<PlaceableObjectManager>();
                placeableObject.ToggleBasedOnAvailableScrap();
            }
        }

        public static IEnumerator AsyncToggleButtonsBasedOnCurrentLevel()
        {
            yield return new WaitUntil(() => ActiveContentController != null);
            ToggleButtonsBasedOnCurrentLevel();
        }

        public static void ToggleButtonsBasedOnCurrentLevel()
        {
            foreach (Transform button in ActiveContentController.transform)
            {
                PlaceableObjectManager placeableObject = button.GetComponent<PlaceableObjectManager>();
                placeableObject.ToggleBasedOnCurrentLevel();
            }
        }

        private void ResetCoroutines()
        {
            if (hideGUICoroutine != null)
            {
                StopCoroutine(hideGUICoroutine);
                hideGUICoroutine = null;
            }
            if (showGUICoroutine != null)
            {
                StopCoroutine(showGUICoroutine);
                showGUICoroutine = null;
            }
        }

        public static void HideEditModeGUI()
        {
            instance.ResetCoroutines();
            instance.hideGUICoroutine = instance.AsyncHideEditModeGUI();
            instance.StartCoroutine(instance.hideGUICoroutine);
        }

        private IEnumerator AsyncHideEditModeGUI()
        {

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
            yield return null;
        }

        public static void ShowEditModeGUI()
        {
            instance.ResetCoroutines();
            instance.showGUICoroutine = instance.AsyncShowEditModeGUI();
            instance.StartCoroutine(instance.showGUICoroutine);
        }

        private IEnumerator AsyncShowEditModeGUI()
        {
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
            yield return null;
        }
    }
}
