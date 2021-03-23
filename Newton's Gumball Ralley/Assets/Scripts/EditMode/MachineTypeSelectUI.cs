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

        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        RectTransform lengthRectTransform = GetComponent<RectTransform>();
        float GUI_width = MachineBtnTemplate.GetComponent<RectTransform>().sizeDelta.x;
        
        
        int index = 0;
        foreach (MachineTypeSO machineTypeSO in machineTypeSOList)
        {
            Transform MachineBtnTransform = Instantiate(MachineBtnTemplate, transform);
            MachineBtnTransform.gameObject.SetActive(true);

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
        layoutGroup.spacing = (lengthRectTransform.rect.width - (index * GUI_width)) / (index - 1);
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
