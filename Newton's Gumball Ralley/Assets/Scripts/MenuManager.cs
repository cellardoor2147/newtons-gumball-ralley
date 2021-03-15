using Audio;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] string hoverOverSound = "ButtonHover";

    [SerializeField] string pressButtonSound = "ButtonPress";
    AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null) {
            Debug.LogError ("No audiomanager found");
        }
    }
    // To be implemented when GUI/gamestate manager is added
    
    // public void OnMouseOver () {
    //     audioManager.PlaySound(hoverOverSound);
    // }


}
