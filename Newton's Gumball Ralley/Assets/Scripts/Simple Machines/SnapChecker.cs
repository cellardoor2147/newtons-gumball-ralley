﻿using Core;
using UnityEngine;

namespace SnapCheck
{
    public class SnapChecker : MonoBehaviour
    {
        private PlacedObjectManager objectManager;
        private string objectName;
        private Vector2 mousePosition;
        public GameObject SnapPointHolder { get; private set; }
        public bool ShouldSnap { get; private set; } = false;

        private void Awake()
        {
            objectManager = GetComponent<PlacedObjectManager>();
            objectName = objectManager.metaData.objectName;
        }

        private void Update()
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);           
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            switch (objectName)
            {
                case "LeverFulcrum":
                    if (collision.gameObject.name.Equals("LeverSnapPoints"))
                    {
                        bool mouseInSnappingRange = collision.OverlapPoint(mousePosition);
                        if (mouseInSnappingRange)
                        {
                            ShouldSnap = true;
                            SnapPointHolder = collision.gameObject;
                        }
                        else
                        {
                            ShouldSnap = false;
                            SnapPointHolder = null;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            ShouldSnap = false;
            SnapPointHolder = null;
        }
    }
}