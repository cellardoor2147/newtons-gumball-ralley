using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonGUI : MonoBehaviour
{
    public MachineTypeSO machineTypeSO;
    private Color Color;


    private void Awake()
    {
        this.gameObject.SetActive(true);

        transform.Find("Image").GetComponent<Image>().sprite = machineTypeSO.sprite;
    }

    private void Start()
    {
        transform.Find("Selected").gameObject.SetActive(false);
    }

    private void Update()
    {
        if (transform.Find("Selected").gameObject.activeSelf) {
            for (int i = 0; i < transform.childCount; i++)
            {
                Color = transform.GetChild(i).GetComponent<Image>().color;
                Color.a = 0.5f;
                transform.GetChild(i).GetComponent<Image>().color = Color;
            }
        } 
        else {
            for (int i = 0; i < transform.childCount; i++)
            {
                Color = transform.GetChild(i).GetComponent<Image>().color;
                Color.a = 1f;
                transform.GetChild(i).GetComponent<Image>().color = Color;
            }
        }
    }
}
