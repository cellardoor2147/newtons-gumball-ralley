using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edit;

public class MachineTypeSelectUI : MonoBehaviour
{
    [SerializeField] private EditManager editManager;

    private void Awake()
    {
        GameObject container = GameObject.Find("Button Container");
        for (int i = 0; i < container.transform.GetChildCount(); i++)
        {
            Transform button = container.transform.GetChild(i);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ResetActiveMachineType(); 
                editManager.SetActiveMachineType(button.GetComponent<ButtonGUI>().machineTypeSO);
                button.transform.Find("Selected").gameObject.SetActive(true);
            });
        }
    }

    private void Start(){
        ResetActiveMachineType();
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
}
