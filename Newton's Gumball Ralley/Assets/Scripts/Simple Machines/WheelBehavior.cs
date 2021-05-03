using System.Collections;
using UnityEngine;
using Audio;


namespace SimpleMachine{
    public class WheelBehavior : MonoBehaviour
    {
        private Rigidbody2D rigidBody;
        [SerializeField] SoundMetaData GearSound;

        private float gearVolume;
        private bool isFading;
        private float fadeTime = 0.5f;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();

            gearVolume = 0.15f;
        }

        private void FixedUpdate()
        {
            if (rigidBody.angularVelocity > 10)
            {
                if (!AudioManager.instance.isPlaying(GearSound.name))
                {
                    AudioManager.instance.PlaySound(GearSound.name);
                    AudioManager.instance.SetVolume(GearSound.name, gearVolume);
                    isFading = false;
                }
            }
            else if (rigidBody.angularVelocity < 1)
            {
                if (AudioManager.instance.isPlaying(GearSound.name) && !isFading)
                {
                    AudioManager.instance.FadeSound(GearSound.name, fadeTime, 0);
                    AudioManager.instance.StopSound(GearSound.name);
                    AudioManager.instance.SetVolume(GearSound.name, gearVolume);
                    isFading = true;
                }
            }
        }
    }
}

