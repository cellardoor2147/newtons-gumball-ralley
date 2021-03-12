using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GUI
{
    public enum GUIType
    {
        MainMenu = 0,
        PlayMode = 1,
        PauseMenu = 2
    }

    public class GUIManager : MonoBehaviour
    {
        List<GUIController> guiControllers;

        private void Awake()
        {
            guiControllers = GetComponentsInChildren<GUIController>().ToList();
        }

        public void SetActiveGUI(GUIType guiType)
        {
            SetAllGUIToInactive();
            GUIController guiControllerToActivate =
                guiControllers.Find(guiController => guiController.guiType.Equals(guiType));
            if (guiControllerToActivate != null)
            {
                guiControllerToActivate.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Tried setting invalid GUI type: {guiType.ToString()}");
            }
        }

        private void SetAllGUIToInactive()
        {
            guiControllers.ForEach(guiController => guiController.gameObject.SetActive(false));
        }
    }
}
