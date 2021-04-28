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
        private bool isDisabled;

        public PlaceableObjectType objectType;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            originalBackgroundImageColor = backgroundImage.color;
            labelText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if (isDisabled)
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
            if (!isDisabled)
            {
                backgroundImage.color = originalBackgroundImageColor;
                labelText.color = Color.black;
            }
        }

        public void ReddenAndDisable()
        {
            backgroundImage.color = new Color(.25f, 0, 0, 1);
            labelText.color = new Color(.5f, 0, 0, 1);
            isDisabled = true;
        }

        public void Disable()
        {
            isDisabled = true;
        }

        public void RevertToOriginalColorsAndEnable()
        {
            backgroundImage.color = originalBackgroundImageColor;
            labelText.color = Color.black;
            isDisabled = false;
        }

        public void Enable()
        {
            isDisabled = false;
        }
    }
}
