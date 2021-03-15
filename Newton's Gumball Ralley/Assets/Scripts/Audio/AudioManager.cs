using UnityEngine;

namespace Audio {
    [System.Serializable] public class Sound {
        public SoundMetaData MetaData;
        
        private AudioSource source; 

        public void SetSource (AudioSource _source){
            source = _source;
            source.clip = MetaData.clip;
            source.loop = MetaData.loop;
        }

        public void Play (){
            source.pitch = MetaData.pitch;
            source.volume = MetaData.volume; 
            source.Play();
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
            PlaySound ("Menu_Music"); 
        }
        // To be implemented when GUI/gamestate manager is added
        
        // void Update () {
        //     if (time.time >5f)
        // }

        public void PlaySound (string _name) {
            foreach (Audio.Sound element in sounds)
            {
                if (element.MetaData.name == _name)
                {
                    element.Play();
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
        
    }
}