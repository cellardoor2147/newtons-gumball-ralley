using System.Collections;
using UnityEngine;
using Audio;


namespace SimpleMachine{
    public class WheelBehavior : MonoBehaviour
    {
        private Rigidbody2D rigidBody;
        [SerializeField] SoundMetaData GearSound;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (rigidBody.angularVelocity > 0)
            {
                if (!AudioManager.instance.isPlaying(GearSound.name))
                {
                    AudioManager.instance.PlaySound(GearSound.name);
                }
            }
            else
            {
                AudioManager.instance.StopSound(GearSound.name);
            }
        }
    }
}

