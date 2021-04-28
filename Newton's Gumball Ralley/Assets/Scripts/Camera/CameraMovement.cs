using UnityEngine;
using Background;

namespace MainCamera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float cameraBorderThickness = 20f;
        [SerializeField] private float cameraPanSpeed = 5.0f;
        [SerializeField] private float cameraScrollSpeed = 0.1f;
        [SerializeField] private float cameraMinSize = 7f;
        [SerializeField] private float cameraMaxSize = 10f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            UpdateCameraPositionAndDrawDirectionalArrows();
            LimitCameraPosition();
            UpdateCameraZoom();
        }

        private void UpdateCameraPositionAndDrawDirectionalArrows()
        {
            bool shouldMoveCameraLeft =
                Input.mousePosition.x <= cameraBorderThickness;
            if (shouldMoveCameraLeft)
            {
                transform.Translate(Vector2.left * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraRight =
                Input.mousePosition.x >= Screen.width - cameraBorderThickness - 32f;
            if (shouldMoveCameraRight)
            {
                transform.Translate(Vector2.right * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraDown =
                Input.mousePosition.y <= cameraBorderThickness + 32f;
            if (shouldMoveCameraDown)
            {
                transform.Translate(Vector2.down * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraUp =
                Input.mousePosition.y >= Screen.height - cameraBorderThickness;
            if (shouldMoveCameraUp)
            {
                transform.Translate(Vector2.up * cameraPanSpeed * Time.deltaTime);
            }

            if (shouldMoveCameraLeft && shouldMoveCameraUp)
            {
                AsyncSetVisibleArrow(ArrowDirection.UpperLeft);
            }
            else if (shouldMoveCameraLeft && shouldMoveCameraDown)
            {
                AsyncSetVisibleArrow(ArrowDirection.LowerLeft);
            }
            else if (shouldMoveCameraRight && shouldMoveCameraUp)
            {
                AsyncSetVisibleArrow(ArrowDirection.UpperRight);
            }
            else if (shouldMoveCameraRight && shouldMoveCameraDown)
            {
                AsyncSetVisibleArrow(ArrowDirection.LowerRight);
            }
            else if (shouldMoveCameraLeft)
            {
                AsyncSetVisibleArrow(ArrowDirection.Left);
            }
            else if (shouldMoveCameraUp)
            {
                AsyncSetVisibleArrow(ArrowDirection.Up);
            }
            else if (shouldMoveCameraRight)
            {
                AsyncSetVisibleArrow(ArrowDirection.Right);
            }
            else if (shouldMoveCameraDown)
            {
                AsyncSetVisibleArrow(ArrowDirection.Down);
            }
            else
            {
                AsyncSetVisibleArrow(ArrowDirection.None);
            }
        }

        private void AsyncSetVisibleArrow(ArrowDirection arrowDirection)
        {
            StartCoroutine(DirectionalArrowManager.AsyncSetVisibleArrow(arrowDirection));
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
