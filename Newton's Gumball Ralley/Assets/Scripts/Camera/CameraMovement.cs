using UnityEngine;
using Background;
using Core;
using GUI.EditMode;

namespace MainCamera
{
    public class CameraMovement : MonoBehaviour
    {
        public static bool shouldPreventDragging;

        [SerializeField] private float cameraDragSpeed = 0.1f;
        [SerializeField] private float cameraScrollSpeed = 0.1f;
        [SerializeField] private float cameraMinSize = 7f;
        [SerializeField] private float cameraMaxSize = 10f;
        private bool lastMachineAssigned;

        private void Update()
        {
            bool cameraShouldNotMove =
                !(GameStateManager.GetGameState().Equals(GameState.Playing)
                || GameStateManager.GetGameState().Equals(GameState.Editing));
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
                    if (EditModeManager.GetLastSelectedMachine() != null)
                    {
                        EditModeManager.ClearLastSelectedMachine();
                    }
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
    }
}
