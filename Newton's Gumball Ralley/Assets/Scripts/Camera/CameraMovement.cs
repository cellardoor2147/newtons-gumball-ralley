using UnityEngine;

namespace MainCamera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float cameraPanSpeed = 5.0f;
        [SerializeField] private float cameraPanBorderThickness = 20.0f;
        [SerializeField] private float cameraPanLimitX = 10.0f;
        [SerializeField] private float cameraPanLimitY = 5.0f;
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
            bool shouldMoveCameraLeft = Input.mousePosition.x <= cameraPanBorderThickness;
            if (shouldMoveCameraLeft)
            {
                transform.Translate(Vector2.left * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraRight = Input.mousePosition.x >= Screen.width - cameraPanBorderThickness;
            if (shouldMoveCameraRight)
            {
                transform.Translate(Vector2.right * cameraPanSpeed * Time.deltaTime);
            }
        }

        private void LimitCameraPosition()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -cameraPanLimitX, cameraPanLimitX),
                Mathf.Clamp(transform.position.y, -cameraPanLimitY, cameraPanLimitY),
                transform.position.z
            );
        }

        private void UpdateCameraPositionY()
        {
            bool shouldMoveCameraDown = Input.mousePosition.y <= cameraPanBorderThickness;
            if (shouldMoveCameraDown)
            {
                transform.Translate(Vector2.down * cameraPanSpeed * Time.deltaTime);
            }
            bool shouldMoveCameraUp = Input.mousePosition.y >= Screen.height - cameraPanBorderThickness;
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
