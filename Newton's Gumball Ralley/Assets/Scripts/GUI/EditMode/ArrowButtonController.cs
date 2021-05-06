using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI.EditMode
{
    public enum ToolbarArrowDirection
    {
        Down = 0,
        Up = 1
    }

    public class ArrowButtonController : MonoBehaviour, IPointerClickHandler
    {
        private RectTransform rectTransform;
        private ToolbarArrowDirection arrowDirection;
        private Vector3 originalAnchoredPosition;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalAnchoredPosition = rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            EditModeManager.ShowEditModeGUI();
            arrowDirection = ToolbarArrowDirection.Down;
            SetArrowRotation();
            SetArrowPosition();
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (arrowDirection.Equals(ToolbarArrowDirection.Down))
            {
                EditModeManager.HideEditModeGUI();
                arrowDirection = ToolbarArrowDirection.Up;
            }
            else
            {
                EditModeManager.ShowEditModeGUI();
                arrowDirection = ToolbarArrowDirection.Down;
            }
            SetArrowRotation();
            SetArrowPosition();
        }

        private void SetArrowRotation()
        {
            rectTransform.localEulerAngles = arrowDirection.Equals(ToolbarArrowDirection.Down)
                ? new Vector3(0f, 0f, 0f)
                : new Vector3(0f, 0f, 180f);
        }

        private void SetArrowPosition()
        {
            rectTransform.anchoredPosition = arrowDirection.Equals(ToolbarArrowDirection.Down)
                ? originalAnchoredPosition
                : originalAnchoredPosition + Vector3.up * 100f;
        }
    }
}
