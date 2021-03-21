using UnityEngine;
using UnityEngine.EventSystems;

public class EditManager : MonoBehaviour
{

    private MachineTypeSO activeMachineType;

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

    private bool CanSpawnMachine(){
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        BoxCollider2D machineCollider = activeMachineType.prefab.GetComponent<BoxCollider2D>();

        bool ableToSpawnMachine = Physics2D.OverlapBox(position + machineCollider.offset, machineCollider.size, 0) == null;
        return ableToSpawnMachine;
    }
}
