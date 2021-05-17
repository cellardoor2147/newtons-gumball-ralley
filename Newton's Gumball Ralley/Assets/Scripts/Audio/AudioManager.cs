using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio {
    [System.Serializable] public class Sound {
        public SoundMetaData MetaData;
        
        [HideInInspector] public AudioSource source; 

        public void SetSource (AudioSource _source){
            source = _source;
            source.clip = MetaData.clip;
            source.loop = MetaData.loop;
        }

        public void Play (float universalVolume){
            source.pitch = MetaData.pitch;
            source.volume = MetaData.volume * universalVolume; 
            source.Play();
        }

        public void ResumeWithNewUniveralVolume(float universalVolume)
        {
            float playbackTime = source.time;
            source.volume = MetaData.volume * universalVolume;
            source.Play();
            source.time = playbackTime;
        }

        public void Pause (){
            source.Pause();
        }

        public void Stop()
        {
            source.Stop();
        }
    }
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        [SerializeField] Sound[] sounds;

        private static readonly string Sound_ = "Sound_";
        private static readonly string SoundNotFound = "AudioManager: Sound not found in list ";

        private float universalVolume = 0.5f;

        void Awake()
        {
            if (instance != null) {
                if (instance != this){
                    Destroy(this.gameObject);
                }
            } else {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        void Start () {
            foreach (Audio.Sound element in sounds)
            {
                GameObject currentSoundGameObject = new GameObject(Sound_ + element + "_" + element.MetaData.name);
                currentSoundGameObject.transform.SetParent (this.transform);
                element.SetSource (currentSoundGameObject.AddComponent<AudioSource>());
            }
        }

        public void PlaySound (string _name) {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    element.Play(universalVolume);
                    return;
                }
            }
            //no sound with _name
            Debug.LogWarning(SoundNotFound + _name);
        }

        public bool isPlaying (string _name) {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    if (element.source.isPlaying){
                        return true;
                    }
                    return false;
                }
            }
            //no sound with _name
            Debug.LogWarning(SoundNotFound + _name);
            return false;
        }

        public void PauseSound (string _name) {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    element.Pause();
                    return;
                }
            }
            //no sound with _name
            Debug.LogWarning(SoundNotFound + _name);
        }

        public void StopSound(string _name)
        {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    element.Stop();
                    return;
                }
            }
            //no sound with _name
            Debug.LogWarning(SoundNotFound + _name);
        }

        public IEnumerator Fade(Audio.Sound element, float duration, float targetVolume) 
        {
            float currentTime = 0;
            float start = element.MetaData.volume;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                element.MetaData.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }

        public void FadeSound(string _name, float duration, float targetVolume)
        {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    StartCoroutine(Fade(element, duration, targetVolume));
                    return;
                }
            }
            //no sound with _name
            Debug.LogWarning(SoundNotFound + _name);
        }

        public void SetVolume(string _name, float Volume)
        {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    element.MetaData.volume = Volume;
                    return;
                }
            }
            //no sound with _name
            Debug.LogWarning(SoundNotFound + _name);
        }

        public void SetUniversalVolume(float universalVolume)
        {
            this.universalVolume = universalVolume;
            ResetPlayingSoundVolumes();
        }

        private void ResetPlayingSoundVolumes()
        {
            foreach (Sound sound in sounds)
            {
                if (sound.source.isPlaying)
                {
                    sound.ResumeWithNewUniveralVolume(universalVolume);
                }
            }
        }
    }
}