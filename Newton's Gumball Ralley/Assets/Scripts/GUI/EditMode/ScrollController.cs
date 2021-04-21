using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI.EditMode
{
    public enum ScrollDirection
    {
        Left = 0,
        Right = 1
    }

    public class ScrollController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly string TOOLBAR_CONTENT_CONTAINER_KEY = "Toolbar Content Container";

        [SerializeField] private ScrollDirection scrollDirection;

        private bool shouldScroll;
        private ToolbarManager toolbarManager;

        public void StopScrolling()
        {
            shouldScroll = false;
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            shouldScroll = true;
            toolbarManager = transform
                .parent
                .Find(TOOLBAR_CONTENT_CONTAINER_KEY)
                .GetComponent<ToolbarManager>();
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            shouldScroll = false;
        }

        private void Update()
        {
            if (!shouldScroll)
            {
                return;
            }
            if (scrollDirection.Equals(ScrollDirection.Left))
            {
                toolbarManager.HandleScrollLeft();
            }
            else
            {
                toolbarManager.HandleScrollRight();
            }
        }
    }
}
