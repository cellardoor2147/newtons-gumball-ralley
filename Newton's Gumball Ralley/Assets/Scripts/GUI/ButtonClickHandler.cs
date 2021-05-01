using UnityEngine;
using MainCamera;

public class ButtonClickHandler : MonoBehaviour
{
    public void SetMainCameraShouldPreventDragging(bool shouldPreventDragging)
    {
        CameraMovement.shouldPreventDragging = shouldPreventDragging;
    }
}
