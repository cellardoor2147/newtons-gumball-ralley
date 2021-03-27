using UnityEngine;
using UnityEngine.EventSystems;

namespace Edit {
    public class EditManager : MonoBehaviour
    {

        public MachineTypeSO activeMachineType;

        public void SetActiveMachineType(MachineTypeSO machineTypeSO){
            activeMachineType = machineTypeSO;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Instantiate(activeMachineType.prefab, mousePosition, Quaternion.identity);
        }

        public void ResetActiveMachineType()
        {
            GameObject container = GameObject.Find("Button Container");
            for (int i = 0; i < container.transform.childCount; i++)
            {
                Transform button = container.transform.GetChild(i);
                button.Find("Selected").gameObject.SetActive(false);
            }
        }
    }
}