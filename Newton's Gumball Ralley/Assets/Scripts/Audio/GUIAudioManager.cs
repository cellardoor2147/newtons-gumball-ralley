using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class GUIAudioManager : MonoBehaviour
{

    [SerializeField] SoundMetaData ButtonHoverSound;
    [SerializeField] SoundMetaData ButtonClickSound;

    AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audiomanager found");
        }
    }

    public void OnMouseOver()
    {
        audioManager.PlaySound(ButtonHoverSound.name);
    }
}
