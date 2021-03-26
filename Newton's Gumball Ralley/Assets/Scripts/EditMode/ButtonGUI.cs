using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGUI : MonoBehaviour
{
    public MachineTypeSO machineTypeSO;

    private void Awake()
    {
        this.gameObject.SetActive(true);

        transform.Find("Image").GetComponent<Image>().sprite = machineTypeSO.sprite;
    }

    private void Start()
    {
        transform.Find("Selected").gameObject.SetActive(false);
    }

}
