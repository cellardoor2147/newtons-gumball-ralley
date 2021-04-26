using UnityEngine;
using Background;

namespace MainCamera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Sprite scrollDirectionIndicatorSprite;
        [SerializeField] private float cameraPanSpeed = 5.0f;
        [SerializeField] private float cameraScrollSpeed = 0.1f;
        [SerializeField] private float cameraMinSize = 2.5f;
        [SerializeField] private float cameraMaxSize = 7.0f;

        private void Update()
        {
            UpdateCameraPositionX();
            UpdateCameraPositionY();
            LimitCameraPosition();
            UpdateCameraZoom();
        }

        private void UpdateCameraPositionX()
        {
            bool shouldMoveCameraLeft =
                Input.mousePosition.x <= -16f;
            if (shouldMoveCameraLeft)
            {
                transform.Translate(Vector2.left * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraRight =
                Input.mousePosition.x >= Screen.width + 16f;
            if (shouldMoveCameraRight)
            {
                transform.Translate(Vector2.right * cameraPanSpeed * Time.deltaTime);
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

        private void UpdateCameraPositionY()
        {
            bool shouldMoveCameraDown =
                Input.mousePosition.y <= -16f;
            if (shouldMoveCameraDown)
            {
                transform.Translate(Vector2.down * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraUp =
                Input.mousePosition.y >= Screen.height + 16f;
            if (shouldMoveCameraUp)
            {
                transform.Translate(Vector2.up * cameraPanSpeed * Time.deltaTime);
            }
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
