using UnityEngine;
using System.Collections;
using Audio;

[CreateAssetMenu(fileName = "SoundMetaData", menuName = "ScriptableObjects/SoundMetaData", order = 1)]
public class SoundMetaData : ScriptableObject
{
    public string name = "New MyScriptableObject";
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 0.7f;
    [Range(0.5f, 1f)] public float pitch = 1f;

    public bool loop = false;
}

