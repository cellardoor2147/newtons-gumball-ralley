using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI.EditMode
{
    public class TabController : MonoBehaviour, IPointerClickHandler
    {
        private static readonly string EDIT_MODE_GUI_KEY = "Edit Mode GUI";

        private static EditModeManager editModeManager;


        private Image backgroundImage;

        public PlaceableObjectType objectType;


        private void Awake()
        {
            editModeManager = GameObject.Find(EDIT_MODE_GUI_KEY).GetComponent<EditModeManager>();
            backgroundImage = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            editModeManager.SetActiveTab(objectType);
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
