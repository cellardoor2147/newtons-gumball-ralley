﻿using System.Collections;
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

        private List<TabController> inactiveTabControllers;
        private List<TabController> activeTabControllers;
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

        private void ActivateContent(PlaceableObjectType objectType)
        {
            foreach (ContentController contentController in instance.contentControllers)
            {
                contentController.gameObject.SetActive(
                    contentController.objectType.Equals(objectType)
                );
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
