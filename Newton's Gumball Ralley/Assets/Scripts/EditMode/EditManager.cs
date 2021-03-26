using UnityEngine;
using UnityEngine.EventSystems;

namespace Edit {
    public class EditManager : MonoBehaviour
    {

        public MachineTypeSO activeMachineType;

        // Update is called once per frame
        private void Update()
        {
            bool shouldRespondToMouseDown = Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
            if (shouldRespondToMouseDown) {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (CanSpawnMachine()) {
                    Instantiate(activeMachineType.prefab, mousePosition, Quaternion.identity);
                }
            }
        }

        public void SetActiveMachineType(MachineTypeSO machineTypeSO){
            activeMachineType = machineTypeSO;
        }

        public MachineTypeSO GetActiveMachineType () {
            return activeMachineType;
        }

        public void ResetActiveMachineType()
        {
            GameObject container = GameObject.Find("Button Container");
            for (int i = 0; i < container.transform.GetChildCount(); i++)
            {
                Transform button = container.transform.GetChild(i);
                button.Find("Selected").gameObject.SetActive(false);
            }
        }

        private bool CanSpawnMachine(){
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BoxCollider2D machineCollider = activeMachineType.prefab.GetComponent<BoxCollider2D>();

            bool ableToSpawnMachine = Physics2D.OverlapBox(position + machineCollider.offset, machineCollider.size, 0) == null;
            return ableToSpawnMachine;
        }
    }
}