using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edit;

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
        spawnGUI();
    }

    private void spawnGUI() {

        Transform MachineBtnTemplate = transform.Find("MachineBtnTemplate");
        
        int index = 0;
        foreach (MachineTypeSO machineTypeSO in machineTypeSOList)
        {
            Transform MachineBtnTransform = Instantiate(MachineBtnTemplate, transform);
            MachineBtnTransform.gameObject.SetActive(true);

            float GUI_width = MachineBtnTransform.GetComponent<RectTransform>().sizeDelta.x;
            
            MachineBtnTransform.GetComponent<RectTransform>().anchoredPosition += new Vector2(index * (GUI_width / 1.75f), 0);
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
