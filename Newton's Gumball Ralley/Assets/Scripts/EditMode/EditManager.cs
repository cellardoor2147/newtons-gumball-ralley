using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditManager : MonoBehaviour
{

    [SerializeField] private MachineTypeSO activeMachineType;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (CanSpawnMachine(activeMachineType, mousePosition)) {
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

    private bool CanSpawnMachine(MachineTypeSO machineTypeSO, Vector3 position){
        BoxCollider2D machineCollider = machineTypeSO.prefab.GetComponent<BoxCollider2D>();

        if (Physics2D.OverlapBox(position + (Vector3) machineCollider.offset, machineCollider.size, 0) != null) {
            return false;
        } else {
            return true;
        }
    }
}
