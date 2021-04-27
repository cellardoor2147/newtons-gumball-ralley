using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using GUI;

namespace MainCamera
{
    public enum ArrowDirection
    {
        Left = 0,
        UpperLeft = 1,
        Up = 2,
        UpperRight = 3,
        Right = 4,
        LowerRight = 5,
        Down = 6,
        LowerLeft = 7,
        None = 8
    }

    public class DirectionalArrowManager : MonoBehaviour
    {
        private static DirectionalArrowManager instance;

        private List<DirectionalArrowController> arrowControllers;

        private DirectionalArrowManager() { } // prevent instantiation outside of this class

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            arrowControllers = GetComponentsInChildren<DirectionalArrowController>().ToList();
        }

        public static IEnumerator AsyncSetVisibleArrow(ArrowDirection arrowDirection)
        {
            yield return new WaitUntil(() => instance != null);
            if (
                !GUIManager.GetActiveGUIType().Equals(GUIType.PlayMode) 
                && !GUIManager.GetActiveGUIType().Equals(GUIType.EditMode)
            )
            {
                yield break;
            }
            foreach (DirectionalArrowController arrowController in instance.arrowControllers)
            {
                Image arrowImage = arrowController.GetComponent<Image>();
                arrowImage.enabled = arrowController.arrowDirection.Equals(arrowDirection);
            }
            Cursor.visible = arrowDirection.Equals(ArrowDirection.None);
        }
    }
}
