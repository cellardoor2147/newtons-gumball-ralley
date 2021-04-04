using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI.EditMode
{
    public class TabController : MonoBehaviour, IPointerClickHandler
    {
        private Image backgroundImage;

        public PlaceableObjectType objectType;


        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            EditModeManager.SetActiveTab(objectType);
        }

        public void SetTabColorToActive()
        {
            backgroundImage.color = Color.white;
        }

        public void SetTabColorToInactive()
        {
            backgroundImage.color = Color.gray;
        }
    }
}
