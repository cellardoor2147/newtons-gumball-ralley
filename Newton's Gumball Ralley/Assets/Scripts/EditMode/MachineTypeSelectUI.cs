using UnityEngine;
using UnityEngine.UI;
using Edit;

public class MachineTypeSelectUI : MonoBehaviour
{
    [SerializeField] private EditManager editManager;

    private void Awake()
    {
        GameObject container = GameObject.Find("Button Container");
        for (int i = 0; i < container.transform.childCount; i++)
        {
            Transform button = container.transform.GetChild(i);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                editManager.ResetActiveMachineType(); 
                editManager.SetActiveMachineType(button.GetComponent<ButtonGUI>().machineTypeSO);
                button.transform.Find("Selected").gameObject.SetActive(true);
            });
        }
    }

    private void Start(){
        editManager.ResetActiveMachineType();
    }
}
