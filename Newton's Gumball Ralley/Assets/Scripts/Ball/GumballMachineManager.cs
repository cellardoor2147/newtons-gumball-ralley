﻿using System.Collections;
using UnityEngine;
using Core;

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

        [SerializeField] private Sprite gumballMachineClosedSprite;
        [SerializeField] private Sprite gumballMachineOpenSprite;
        [SerializeField] private float shakeTime;
        [SerializeField] private float delayBetweenShakes;
        [SerializeField] private float maxShakeDistance;
        [SerializeField] private float maxShakeRotationEulerAngles;
        [SerializeField] private float ballDispenseTimeMultiplier;

        private SpriteRenderer spriteRender;
        private Collider2D collider2D;
        private GumballMachineState gumballMachineState;
        private GameObject slingAnchor;
        private BallMovement ballMovement;
        private SpriteRenderer ballSpriteRenderer;
        private Vector3 originalPosition;
        private Vector3 originalEulerAngles;
        private Vector3 originalScale;

        private void Awake()
        {
            spriteRender = GetComponent<SpriteRenderer>();
            collider2D = GetComponent<Collider2D>();
            slingAnchor = transform.Find(SLING_ANCHOR_KEY).gameObject;
            ballMovement =
                slingAnchor.transform.Find(BALL_KEY).GetComponent<BallMovement>();
            ballSpriteRenderer =
                slingAnchor.transform.Find(BALL_KEY).GetComponent<SpriteRenderer>();
            SetGumballMachineState(GumballMachineState.Closed);
            originalPosition = transform.position;
            originalEulerAngles = transform.localEulerAngles;
            originalScale = transform.localScale;
        }

        public void SetGumballMachineState(GumballMachineState gumballMachineState)
        {
            StopAllCoroutines();
            switch (gumballMachineState)
            {
                case GumballMachineState.Closed:
                    ballSpriteRenderer.enabled = false;
                    StartCoroutine(ResetBallPosition());
                    spriteRender.sprite = gumballMachineClosedSprite;
                    SetClickability(true);
                    break;
                case GumballMachineState.Shaking:
                    SetClickability(false);
                    StartCoroutine(ResetBallPosition());
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

        private IEnumerator ResetBallPosition()
        {
            yield return ballMovement.AsyncResetPosition();
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
                SetGumballMachineState(GumballMachineState.Shaking);
            }
        }

        private IEnumerator ShakeThenResetTransform()
        {
            yield return Shake();
            transform.position = originalPosition;
            transform.localEulerAngles = originalEulerAngles;
            transform.localScale = originalScale;
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
            ballMovement.enabled = false;
            Vector3 originalScale = slingAnchor.transform.localScale;
            yield return DispenseGumball(originalScale);
            slingAnchor.transform.localScale = originalScale;
            ballSpriteRenderer.color = Color.white;
            ballMovement.enabled = true;
        }

        private IEnumerator DispenseGumball(Vector3 originalScale)
        {
            slingAnchor.transform.localScale = Vector3.zero;
            ballSpriteRenderer.enabled = true;
            ballSpriteRenderer.color = Color.black;
            while (slingAnchor.transform.localScale.x < originalScale.x)
            {
                slingAnchor.transform.localScale = new Vector3(
                    slingAnchor.transform.localScale.x + Time.deltaTime,
                    slingAnchor.transform.localScale.y + Time.deltaTime,
                    slingAnchor.transform.localScale.z
                );
                float ballSpriteRendererColorMultiplier =
                    slingAnchor.transform.localScale.x / originalScale.x;
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