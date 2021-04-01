using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleMachine
{
    public class DraggingController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Color defaultColor;

        private Vector2 GetMousePositionInWorldCoordinates()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            transform.position = GetMousePositionInWorldCoordinates();
        }

        public void OnDrag(PointerEventData pointerEventData)
        {
            transform.position = GetMousePositionInWorldCoordinates();
        }

        public void OnEndDrag(PointerEventData pointerEventData)
        {
            transform.position = GetMousePositionInWorldCoordinates();
        }
    }
}
