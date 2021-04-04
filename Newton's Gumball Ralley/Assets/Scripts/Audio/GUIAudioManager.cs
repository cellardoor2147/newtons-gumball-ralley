using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class GUIAudioManager : MonoBehaviour
{

    [SerializeField] SoundMetaData ButtonHoverSound;
    [SerializeField] SoundMetaData ButtonClickSound;

    private void Start()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogError("No audiomanager found");
        }
    }

    public void OnMouseDown()
    {
        AudioManager.instance.PlaySound(ButtonClickSound.name);
    }

    public void OnMouseOver()
    {
        AudioManager.instance.PlaySound(ButtonHoverSound.name);
    }
}
