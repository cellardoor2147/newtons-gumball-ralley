using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GUI
{
    public enum GUIType
    {
        Cutscene = 0,
        MainMenu = 1,
        PlayMode = 2,
        SettingsMenu = 3
    }

    public class GUIManager : MonoBehaviour
    {
        private readonly static string CANVAS_KEY = "Canvas";
        
        private static GUIManager instance;

        private List<GUIController> guiControllers;

        private GUIManager() {} // Prevents instantiation outside of this class

        private void Awake()
        {
            SetInstance();
            AttachMainCameraToCanvas();
            guiControllers = GetComponentsInChildren<GUIController>(true).ToList();
        }

        private void SetInstance()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void AttachMainCameraToCanvas()
        {
            transform.Find(CANVAS_KEY).GetComponent<Canvas>().worldCamera = Camera.main;
        }

        public static void SetActiveGUI(GUIType guiType)
        {
            SetAllGUIToInactive();
            GUIController guiControllerToActivate = instance.guiControllers.Find(
                guiController => guiController.guiType.Equals(guiType)
            );
            if (guiControllerToActivate != null)
            {
                guiControllerToActivate.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Tried setting invalid GUI type: {guiType}");
            }
        }

        private static void SetAllGUIToInactive()
        {
            instance.guiControllers.ForEach(
                guiController => guiController.gameObject.SetActive(false)
            );
        }
    }
}
