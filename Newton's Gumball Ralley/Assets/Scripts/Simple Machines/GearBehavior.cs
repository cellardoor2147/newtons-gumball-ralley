using UnityEngine;
using System.Collections.Generic;
using Audio;
using Core;

public class GearBehavior : MonoBehaviour
{
    private enum SpinState
    {
        NotSpinning = 0,
        SpinningRight = 1,
        SpinningLeft = 2,
    }

    private Rigidbody2D rigidbody2D;
    private Vector3 ConstrainedPosition;
    private bool hasSet;
    [SerializeField] float torque;

    private SpinState spinState;

    [SerializeField] SoundMetaData GearSound;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        torque = 5000f;
        spinState = SpinState.NotSpinning;
    }

    void Update()
    {
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
            spinState.Equals(SpinState.SpinningRight) && rigidbody2D.angularVelocity < 180)
        {
            rigidbody2D.AddTorque(torque);
        }
        if (GameStateManager.GetGameState().Equals(GameState.Playing) &&
            spinState.Equals(SpinState.SpinningLeft) && rigidbody2D.angularVelocity > -180)
        {
            rigidbody2D.AddTorque(torque * -1);
        }
        else if (spinState.Equals(SpinState.NotSpinning) || !GameStateManager.GetGameState().Equals(GameState.Playing))
        {
            AudioManager.instance.StopSound(GearSound.name);
            spinState = SpinState.NotSpinning;
        }
    }

    private void OnMouseDown()
    {
        if (GameStateManager.GetGameState().Equals(GameState.Playing) && spinState.Equals(SpinState.NotSpinning))
        {
            AudioManager.instance.StopSound(GearSound.name);
            spinState = SpinState.SpinningRight;
        }
        else if (GameStateManager.GetGameState().Equals(GameState.Playing) && spinState.Equals(SpinState.SpinningRight))
        {
            AudioManager.instance.StopSound(GearSound.name);
            spinState = SpinState.SpinningLeft;
        }
        else if (GameStateManager.GetGameState().Equals(GameState.Playing) && spinState.Equals(SpinState.SpinningLeft))
        {
            AudioManager.instance.StopSound(GearSound.name);
            spinState = SpinState.SpinningRight;
        }

    } 
}
