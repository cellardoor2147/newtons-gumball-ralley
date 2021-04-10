using System.Collections;
using UnityEngine;
using Core;
using Audio;

namespace Ball
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallMovement : MonoBehaviour
    {
        [SerializeField] private float baseMovementSpeed = 1.0f;
        [SerializeField] private float maxRadiusOfPull = 2.0f;
        [SerializeField] private float delayAfterRelease = 0.3f;

        private Rigidbody2D rigidBody;
        private bool isBeingPulled;
        private bool hasBeenReleased;

        [SerializeField] SoundMetaData BounceSound;
        [SerializeField] SoundMetaData RollingSound;

        [SerializeField] private float fadeTime = 0.5f;
        [SerializeField] private float finalVolume = 0f;
        [SerializeField] private float rollingVolume = 0.2f;

        private bool isFading;
        private bool isTouching;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0.0f;
        }

        private void Start()
        {
            if (AudioManager.instance == null)
            {
                Debug.LogError("No audiomanager found");
            }            
        }

        private void FixedUpdate()
        {
            if (!hasBeenReleased)
            {
                AudioManager.instance.StopSound(RollingSound.name);
                UpdateBallPositionRelativeToSling();
            }
            else {
                if (rigidBody.velocity.magnitude > 0.01f && !AudioManager.instance.isPlaying(RollingSound.name) && isTouching) 
                {
                    AudioManager.instance.SetVolume(RollingSound.name, rollingVolume);
                    AudioManager.instance.PlaySound(RollingSound.name);
                    isFading = false;
                } 
                else if (rigidBody.velocity.magnitude < 0.01f || !isTouching) 
                {
                    if (AudioManager.instance.isPlaying(RollingSound.name) && !isFading) 
                    {
                        AudioManager.instance.FadeSound(RollingSound.name, fadeTime, finalVolume);
                        AudioManager.instance.StopSound(RollingSound.name);
                        AudioManager.instance.SetVolume(RollingSound.name, rollingVolume);
                        isFading = true;
                    }
                }
            }
        }

        private void UpdateBallPositionRelativeToSling()
        {
            if (!isBeingPulled)
            {
                return;
            }
            Vector2 mousePositionAdjustedForCamera =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 distanceBetweenMouseAndSlingPositions =
                mousePositionAdjustedForCamera - GetSlingAnchorPosition();
            Vector2 clampedDistanceBetweenMouseAndSlingPositions =
                Mathf.Min(maxRadiusOfPull, distanceBetweenMouseAndSlingPositions.magnitude) *
                distanceBetweenMouseAndSlingPositions.normalized;
            Vector2 newPosition =
                GetSlingAnchorPosition() + clampedDistanceBetweenMouseAndSlingPositions;
            Vector2 newVelocity =
                (newPosition - (Vector2)transform.position) * 20;
            rigidBody.velocity = newVelocity;
        }

        private Vector2 GetSlingAnchorPosition()
        {
            return transform.parent.position;
        }

        private void OnMouseDown()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                return;
            }
            if (!hasBeenReleased)
            {
                isBeingPulled = true;
            }
        }

        private void OnMouseUp()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                return;
            }
            if (!hasBeenReleased)
            {
                isBeingPulled = false;
                hasBeenReleased = true;
                rigidBody.gravityScale = 1.0f;
                StartCoroutine(ReleaseAfterDelay());
            }
        }

        private void OnCollisionEnter2D(Collision2D other) 
        {
            if (hasBeenReleased) 
            {
                isTouching = true;
                AudioManager.instance.PlaySound(BounceSound.name);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (hasBeenReleased) 
            {
                isTouching = false;
            } 
        }

        private IEnumerator ReleaseAfterDelay()
        {
            yield return new WaitForSeconds(delayAfterRelease);
            GetComponent<SpringJoint2D>().enabled = false;
        }

        public void ResetPosition()
        {
            transform.position = GetSlingAnchorPosition();
            GetComponent<SpringJoint2D>().enabled = true;
            hasBeenReleased = false;
        }
    }
}