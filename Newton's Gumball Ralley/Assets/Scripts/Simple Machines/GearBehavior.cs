using UnityEngine;
using System.Collections.Generic;
using Audio;
using Core;

namespace SimpleMachine {
    public class GearBehavior : MonoBehaviour
    {
        [HideInInspector] public enum SpinState
        {
            NotSpinning = 0,
            SpinningRight = 1,
            SpinningLeft = 2,
        }

        private Rigidbody2D rigidbody2D;
        private Vector3 ConstrainedPosition;
        private bool hasSet;
        private bool hasSpawned;
        [SerializeField] private float torque;
        [SerializeField] private float spinSpeed;
        [HideInInspector] public SpinState spinState;
        [HideInInspector] public bool shouldSpinLeft;
        [HideInInspector] public bool shouldSpinRight;

        [SerializeField] SoundMetaData GearSound;
        [SerializeField] private GameObject gearUIPrefab;
        private GameObject placedObjectsContainer;
        
        private GameObject gearUI;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spinState = SpinState.NotSpinning;
            shouldSpinLeft = true;
            shouldSpinRight = true;
        }

        void Update()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Playing) && !hasSpawned) 
            {
                hasSpawned = true;
                AddGearUI();
            }
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                hasSpawned = false;
                RemoveGearUI();
            }
            if (GameStateManager.GetGameState().Equals(GameState.Playing) && !hasSet) 
            {
                hasSet = true;
                ConstrainedPosition = transform.position;
            }
            else if (GameStateManager.GetGameState().Equals(GameState.Editing) && hasSet) 
            {
                hasSet = false;
            }
            switch (spinState)
            {
                case SpinState.NotSpinning:
                    rigidbody2D.angularVelocity = 0f;
                    rigidbody2D.freezeRotation = true;
                    break;
                case SpinState.SpinningRight:
                    rigidbody2D.angularVelocity = 0f;
                    rigidbody2D.freezeRotation = false;
                    if (!AudioManager.instance.isPlaying(GearSound.name))
                    {
                        AudioManager.instance.PlaySound(GearSound.name);
                    }
                    break;
                case SpinState.SpinningLeft:
                    rigidbody2D.angularVelocity = 0f;
                    rigidbody2D.freezeRotation = false;
                    if (!AudioManager.instance.isPlaying(GearSound.name))
                    {
                        AudioManager.instance.PlaySound(GearSound.name);
                    }
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Playing)) 
            {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.position = transform.position = ConstrainedPosition;
            }
            if (GameStateManager.GetGameState().Equals(GameState.Playing) &&
            spinState.Equals(SpinState.SpinningRight) && rigidbody2D.angularVelocity < spinSpeed)
            {
                rigidbody2D.AddTorque(torque);
            }
            if (GameStateManager.GetGameState().Equals(GameState.Playing) &&
                spinState.Equals(SpinState.SpinningLeft) && rigidbody2D.angularVelocity > (-1 * spinSpeed))
            {
                rigidbody2D.AddTorque(torque * -1);
            }
            else if (spinState.Equals(SpinState.NotSpinning) || !GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                AudioManager.instance.StopSound(GearSound.name);
                spinState = SpinState.NotSpinning;
            } 
        }

        public void RemoveGearUI()
        {
            if (gearUI != null)
            {
                Destroy(gearUI);
            }
        }

        public void AddGearUI()
        {
            if (gearUIPrefab != null)
            {
                gearUI = Instantiate(
                    gearUIPrefab,
                    transform.position,
                    Quaternion.identity
                );
                foreach (
                    SpinningController child in gearUI.GetComponentsInChildren<SpinningController>()
                )
                {
                    child.SetObjectToSpin(gameObject);
                }
            }
        }
    }
}
