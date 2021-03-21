using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineTypeSelectUI : MonoBehaviour
{
    [SerializeField] private List <MachineTypeSO> machineTypeSOList;
    [SerializeField] private EditManager editManager;

    private Dictionary <MachineTypeSO, Transform> MachineBtnDictionary;
    private void Awake()
    {
        Transform MachineBtnTemplate = transform.Find("MachineBtnTemplate");
        MachineBtnTemplate.gameObject.SetActive(false);

        MachineBtnDictionary = new Dictionary<MachineTypeSO, Transform>();
        spawnGUI(MachineBtnTemplate);
    }

    private void spawnGUI(Transform template) {

        int index = 0;
        foreach (MachineTypeSO machineTypeSO in machineTypeSOList)
        {
            Transform MachineBtnTransform = Instantiate(template, transform);
            MachineBtnTransform.gameObject.SetActive(true);

            MachineBtnTransform.GetComponent<RectTransform>().anchoredPosition += new Vector2(index * 60, 0);
            MachineBtnTransform.Find("Image").GetComponent<Image>().sprite = machineTypeSO.sprite;

            MachineBtnTransform.GetComponent<Button>().onClick.AddListener(() =>
            {
                editManager.SetActiveMachineType(machineTypeSO);
                UpdateSelectedVisual();
            });
            if (index == 0)
            {
                editManager.SetActiveMachineType(machineTypeSO);
            }
            MachineBtnDictionary[machineTypeSO] = MachineBtnTransform;
            index++;
        }
    }

    private void Start(){
        UpdateSelectedVisual();
    }

    private void UpdateSelectedVisual() {
        foreach (MachineTypeSO machineTypeSO in MachineBtnDictionary.Keys) {
            MachineBtnDictionary[machineTypeSO].Find("Selected").gameObject.SetActive(false);
            
        }

        MachineTypeSO activeMachineType = editManager.GetActiveMachineType();
        MachineBtnDictionary[activeMachineType].Find("Selected").gameObject.SetActive(true);
    }
}
