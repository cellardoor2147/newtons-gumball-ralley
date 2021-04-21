using UnityEngine;
using UnityEngine.UI;

namespace GUI.EditMode
{
    public class ToolbarManager : MonoBehaviour
    {
        private static readonly string SCROLL_LEFT_BUTTON_KEY = "Scroll Left Button";
        private static readonly string SCROLL_RIGHT_BUTTON_KEY = "Scroll Right Button";

        private GameObject scrollLeftButton;
        private GameObject scrollRightButton;
        private ScrollRect scrollRect;

        private void Awake()
        {
            scrollLeftButton = transform.parent.Find(SCROLL_LEFT_BUTTON_KEY).gameObject;
            scrollRightButton = transform.parent.Find(SCROLL_RIGHT_BUTTON_KEY).gameObject;
            scrollRect = GetComponent<ScrollRect>();
        }

        public void SetContent(GameObject contentContainer)
        {
            scrollRect.content = contentContainer.GetComponent<RectTransform>();
            scrollRect.normalizedPosition = new Vector2(0f, 1f);
            scrollLeftButton.SetActive(false);
            bool shoudActivateRightButton = scrollRect.content.sizeDelta.x
                 > GetComponent<RectTransform>().rect.width;
            scrollRightButton.SetActive(shoudActivateRightButton);
        }

        public void HandleScrollLeft()
        {
            float currentPositionX = scrollRect.normalizedPosition.x;
            scrollRightButton.SetActive(true);
            bool shouldDeactivateScrollingLeft = currentPositionX <= 0f;
            if (shouldDeactivateScrollingLeft)
            {
                scrollLeftButton.SetActive(false);
                scrollLeftButton.GetComponent<ScrollController>().StopScrolling();
                return;
            }
            scrollRect.normalizedPosition = new Vector2(currentPositionX - Time.deltaTime, 1f);
        }

        public void HandleScrollRight()
        {
            float currentPositionX = scrollRect.normalizedPosition.x;
            scrollLeftButton.SetActive(true);
            bool shouldDeactivateScrollingRight = currentPositionX >= 1f;
            if (shouldDeactivateScrollingRight)
            {
                scrollRightButton.SetActive(false);
                scrollRightButton.GetComponent<ScrollController>().StopScrolling();
                return;
            }
            scrollRect.normalizedPosition = new Vector2(currentPositionX + Time.deltaTime, 1f);
        }
    }
}

