﻿using System.Collections;
using UnityEngine;
using Core.Levels;
using Core;
using Audio;

namespace Ball
{
    public enum GumballMachineState
    {
        Closed = 0,
        Shaking = 1,
        Open = 2
    }

    public class GumballMachineManager : MonoBehaviour
    {
        private readonly static string SLING_ANCHOR_KEY = "Sling Anchor";
        private readonly static string BALL_KEY = "Ball";
        private readonly static string GO_BUTTON_KEY = "Go Button";

        [SerializeField] private Sprite gumballMachineClosedSprite;
        [SerializeField] private Sprite gumballMachineOpenSprite;
        [SerializeField] private float shakeTime;
        [SerializeField] private float delayBetweenShakes;
        [SerializeField] private float maxShakeDistance;
        [SerializeField] private float maxShakeRotationEulerAngles;
        [SerializeField] private float ballDispenseTimeMultiplier;
        [SerializeField] SoundMetaData ShakeSound;

        private SpriteRenderer spriteRender;
        private Collider2D collider2D;
        private GumballMachineState gumballMachineState;
        private BallMovement ballMovement;
        private SpriteRenderer ballSpriteRenderer;
        private SpriteRenderer goButtonSpriteRenderer;
        private Color goButtonDefaultColor;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;

        private void Awake()
        {
            spriteRender = GetComponent<SpriteRenderer>();
            collider2D = GetComponent<Collider2D>();
            GameObject slingAnchor = transform.Find(SLING_ANCHOR_KEY).gameObject;
            ballMovement =
                slingAnchor.transform.Find(BALL_KEY).GetComponent<BallMovement>();
            ballSpriteRenderer =
                slingAnchor.transform.Find(BALL_KEY).GetComponent<SpriteRenderer>();
            goButtonSpriteRenderer =
                transform.Find(GO_BUTTON_KEY).GetComponent<SpriteRenderer>();
            goButtonDefaultColor = goButtonSpriteRenderer.color;
            SetOriginalTransformAndResetTransform();
        }

        private void SetOriginalTransformAndResetTransform()
        {
            /*
             * If the level was loaded in the game scene from the unity editor,
             * LevelManager.currentLevel will be zero, seeing as a level transition
             * hasn't triggered currentLevel to being updated from its zero value.
             * In which case, it's appropriate to leave the gumball machine's transform
             * alone.
             */
            bool levelWasLoadedInGameSceneFromUnityEditor =
                LevelManager.GetCurrentLevelGumballMachineScale().x == 0f;
            if (!levelWasLoadedInGameSceneFromUnityEditor)
            {
                originalPosition = LevelManager.GetCurrentLevelGumballMachinePosition();
                originalRotation = LevelManager.GetCurrentLevelGumballMachineRotation();
                originalScale = LevelManager.GetCurrentLevelGumballMachineScale();
            }
            else
            {
                originalPosition = transform.position;
                originalRotation = transform.rotation;
                originalScale = transform.localScale;
            }
            ResetTransformToOriginalState();
        }

        private void ResetTransformToOriginalState()
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
        }

        public void SetGumballMachineState(GumballMachineState gumballMachineState)
        {
            StopAllCoroutines();
            switch (gumballMachineState)
            {
                case GumballMachineState.Closed:
                    ballSpriteRenderer.enabled = false;
                    AudioManager.instance.StopSound(ShakeSound.name);
                    StartCoroutine(ResetBallAndGumballMachine());
                    spriteRender.sprite = gumballMachineClosedSprite;
                    SetClickability(true);
                    break;
                case GumballMachineState.Shaking:
                    SetClickability(false);
                    StartCoroutine(ResetBallAndGumballMachine());
                    spriteRender.sprite = gumballMachineClosedSprite;
                    StartCoroutine(ShakeThenResetTransform());
                    break;
                case GumballMachineState.Open:
                    SetClickability(false);
                    StartCoroutine(DispenseGumballThenResetItsScaleAndColor());
                    spriteRender.sprite = gumballMachineOpenSprite;
                    break;
            }
            this.gumballMachineState = gumballMachineState;
        }

        private IEnumerator ResetBallAndGumballMachine()
        {
            yield return ballMovement.AsyncReset();
            SetOriginalTransformAndResetTransform();
        }

        private void SetClickability(bool isClickable)
        {
            collider2D.enabled = isClickable;
        }

        private void OnMouseDown()
        {
            bool shouldShakeGumballMachine =
                gumballMachineState.Equals(GumballMachineState.Closed)
                && GameStateManager.GetGameState().Equals(GameState.Playing);
            if (shouldShakeGumballMachine)
            {
                SetGoButtonVisibility(false);
                SetGumballMachineState(GumballMachineState.Shaking);
                if (!AudioManager.instance.isPlaying(ShakeSound.name))
                {
                    AudioManager.instance.PlaySound(ShakeSound.name);
                }
            }
        }

        public void SetGumballVisibility(bool isVisible)
        {
            ballSpriteRenderer.color = isVisible ? Color.white : Color.clear;
        }

        public void SetGoButtonVisibility(bool isVisible)
        {
            goButtonSpriteRenderer.color = isVisible ? goButtonDefaultColor : Color.clear;
        }

        private IEnumerator ShakeThenResetTransform()
        {
            yield return Shake();
            ResetTransformToOriginalState();
            SetGumballMachineState(GumballMachineState.Open);
        }

        private IEnumerator Shake()
        {
            float timer = 0f;
            while (timer < shakeTime)
            {
                timer += Time.deltaTime;
                transform.position = GetRandomShakePosition();
                transform.localEulerAngles = GetRandomShakeRotation();
                yield return new WaitForSeconds(delayBetweenShakes);
            }
        }

        private Vector3 GetRandomShakePosition()
        {
            return new Vector3(
                GetClampedRandomShakePositionOnAxis(
                    transform.position.x,
                    originalPosition.x
                ),
                GetClampedRandomShakePositionOnAxis(
                    transform.position.y,
                    originalPosition.y
                ),
                transform.position.z
            );
        }

        private float GetClampedRandomShakePositionOnAxis(
            float currentAxisValue,
            float startingAxisValue
        )
        {
            float randomShakePositionOnAxis = Random.Range(-1f, 1f) + currentAxisValue;
            float minShakePositionOnAxis = -maxShakeDistance + startingAxisValue;
            float maxShakePositionOnAxis = maxShakeDistance + startingAxisValue;
            return Mathf.Clamp(
                randomShakePositionOnAxis,
                minShakePositionOnAxis,
                maxShakePositionOnAxis
            );
        }
       
        private Vector3 GetRandomShakeRotation()
        {
            return new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                Random.Range(-maxShakeRotationEulerAngles, maxShakeRotationEulerAngles)
            );
        }

        private IEnumerator DispenseGumballThenResetItsScaleAndColor()
        {
            Vector3 ballScale = ballMovement.originalScale;
            yield return DispenseGumball(ballScale);
            ballMovement.transform.localScale = ballMovement.originalScale;
            ballSpriteRenderer.color = Color.white;
            ballMovement.hasBeenDispensed = true;
        }

        private IEnumerator DispenseGumball(Vector3 ballScale)
        {
            ballMovement.transform.localScale = Vector3.zero;
            ballSpriteRenderer.enabled = true;
            ballSpriteRenderer.color = Color.black;
            while
            (
                ballMovement.transform.localScale.x 
                < ballScale.x
            )
            {
                ballMovement.transform.localScale = new Vector3(
                    ballMovement.transform.localScale.x + Time.deltaTime,
                    ballMovement.transform.localScale.y + Time.deltaTime,
                    ballMovement.transform.localScale.z
                );
                float ballSpriteRendererColorMultiplier =
                    ballMovement.transform.localScale.x 
                    / ballScale.x;
                ballSpriteRenderer.color = new Color(
                    ballSpriteRendererColorMultiplier,
                    ballSpriteRendererColorMultiplier,
                    ballSpriteRendererColorMultiplier,
                    ballSpriteRenderer.color.a
                );
                yield return new WaitForSeconds(Time.deltaTime * ballDispenseTimeMultiplier);
            }
        }
    }
}
