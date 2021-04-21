using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace GUI.EditMode
{
    public class TabController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool isClickable;

        private Image backgroundImage;
        private Color originalBackgroundImageColor;
        private TextMeshProUGUI labelText;

        public PlaceableObjectType objectType;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            originalBackgroundImageColor = backgroundImage.color;
            labelText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (isClickable)
            {
                return;
            }
            EditModeManager.SetActiveTab(objectType);
        }

        public void Hide()
        {
            backgroundImage.color = Color.clear;
            labelText.color = Color.clear;
        }

        public void Show()
        {
            backgroundImage.color = originalBackgroundImageColor;
            labelText.color = Color.black;
        }
    }
}
