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

        private List<TabController> tabControllers;
        private List<ContentController> contentControllers;

        private void Awake()
        {
            tabControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(TABS_CONTAINER_KEY)
                .GetComponentsInChildren<TabController>(true).ToList();
            contentControllers = transform
                .Find(PLACEABLE_OBJECTS_MENU_KEY)
                .Find(CONTENT_CONTAINER_KEY)
                .GetComponentsInChildren<ContentController>(true).ToList();
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

        public void SetActiveTab(PlaceableObjectType objectType)
        {
            SetAllTabsToInactive();
            TabController tabControllerToActivate = tabControllers.Find(
                tabController => tabController.objectType.Equals(objectType)
            );
            if (tabControllerToActivate != null)
            {
                tabControllerToActivate.SetTabColorToActive();
                ActivateContent(objectType);
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
    }
}
