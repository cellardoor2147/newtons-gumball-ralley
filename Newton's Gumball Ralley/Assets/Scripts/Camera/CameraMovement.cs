using UnityEngine;
using Background;
using Core;
using GUI.EditMode;
using System.Collections;

namespace MainCamera
{
    public class CameraMovement : MonoBehaviour
    {
        public static bool shouldPreventDragging;

        private static CameraMovement instance;

        [SerializeField] private float cameraDragSpeed = 0.1f;
        [SerializeField] private float cameraScrollSpeed = 0.1f;
        [SerializeField] private float cameraMinSize = 7f;
        [SerializeField] private float cameraMaxSize = 10f;
        private bool lastMachineAssigned;

        private CameraMovement() { } // Prevent instantiation outside this class

        private void Awake()
        {
            SetInstance();
        }

        private void SetInstance()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            bool cameraShouldNotMove =
                !GameStateManager.GetGameState().Equals(GameState.Editing);
            if (cameraShouldNotMove)
            {
                return;
            }
            UpdateCameraPosition();
            LimitCameraPosition();
            UpdateCameraZoom();
        }

        private void UpdateCameraPosition()
        {
            bool playerIsDragging = Input.GetMouseButton(0);
            if (playerIsDragging && !shouldPreventDragging)
            {
                if (lastMachineAssigned)
                {
                    EditModeManager.ClearLastSelectedMachine();
                }
                else 
                {
                    lastMachineAssigned = true;
                }
                Camera.main.transform.position -= new Vector3(
                    Input.GetAxis("Mouse X") * cameraDragSpeed,
                    Input.GetAxis("Mouse Y") * cameraDragSpeed,
                    0f
                );
            }
        }

        private void LimitCameraPosition()
        {
            transform.position = new Vector3(
                Mathf.Clamp(
                    transform.position.x,
                    RepeatedBackgroundManager.GetBorderLeftPositionX(),
                    RepeatedBackgroundManager.GetBorderRightPositionX()
                ),
                Mathf.Clamp(
                    transform.position.y,
                    RepeatedBackgroundManager.GetBorderUpPositionY(),
                    RepeatedBackgroundManager.GetBorderDownPositionY()
                ),
                transform.position.z
            );
        }

        private void UpdateCameraZoom()
        {
            if (Input.mouseScrollDelta.y == 0)
            {
                return;
            }
            float cameraNewSize =
                Camera.main.orthographicSize + (-Input.mouseScrollDelta.y * cameraScrollSpeed);
            Camera.main.orthographicSize =
                Mathf.Clamp(cameraNewSize, cameraMinSize, cameraMaxSize);
        }

        public static IEnumerator AsyncZoomOutForPlayMode()
        {
            yield return new WaitWhile(() => instance == null);
            instance.ZoomCameraOutToFitWholeLevelArea();
            RepeatedBackgroundManager.ExpandBackgroundForPlayMode();
        }

        private void ZoomCameraOutToFitWholeLevelArea()
        {
            transform.position = new Vector3(0f, 0f, transform.position.z);
            while (CameraIsWithinBoundsOfLevelArea())
            {
                Camera.main.orthographicSize++;
            }
        }

        private bool CameraIsWithinBoundsOfLevelArea()
        {
            return transform.position.x > RepeatedBackgroundManager.GetBorderLeftPositionX()
                || transform.position.y > RepeatedBackgroundManager.GetBorderUpPositionY();
        }

        public static IEnumerator AsyncZoomInForEditMode()
        {
            yield return new WaitWhile(() => instance == null);
            instance.transform.position = new Vector3(0f, 0f, instance.transform.position.z);
            Camera.main.orthographicSize = instance.cameraMaxSize;
            RepeatedBackgroundManager.ShrinkBackgroundForEditMode();
        }

        public static IEnumerator AsyncZoomOutForDialogue()
        {
            yield return new WaitWhile(() => instance == null);
            instance.transform.position = new Vector3(0f, 0f, instance.transform.position.z);
            RepeatedBackgroundManager.ShrinkBackgroundForEditMode();
            instance.ZoomCameraOutToFitWholeLevelArea();
            RepeatedBackgroundManager.ExpandBackgroundForPlayMode();
        }
    }
}
