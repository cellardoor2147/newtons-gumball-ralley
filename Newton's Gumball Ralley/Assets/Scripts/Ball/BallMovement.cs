using System.Collections;
using UnityEngine;
using Core;
using Audio;
using SimpleMachine;

namespace Ball
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallMovement : MonoBehaviour
    {
        [SerializeField] private float baseMovementSpeed = 1.0f;
        [SerializeField] private float maxRadiusOfPull = 2.0f;
        [SerializeField] private float delayAfterRelease = 0.3f;

        private Rigidbody2D rigidBody;
        private CircleCollider2D circleCollider;
        private bool isBeingPulled;
        private bool hasBeenReleased;

        [SerializeField] SoundMetaData BounceSound;
        [SerializeField] SoundMetaData RollingSound;

        [SerializeField] PlacedObjectMetaData simplePulleyMetaData;
        [SerializeField] PlacedObjectMetaData compoundPulleyMetaData;


        [SerializeField] private float fadeTime = 0.5f;
        [SerializeField] private float finalVolume = 0f;
        [SerializeField] private float rollingVolume = 0.2f;

        private Vector2 pullForce;
        private Vector2 pushForce;
        private Vector2 holdForce;
        private Vector2 downForce;

        private float bounciness;

        private Vector2 pulleyPosition;
        private bool pulledToMiddle;
        private bool enteredPlatform;

        private PulleyBehavior pulleyBehavior;

        private bool isFading;
        private bool isTouching;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            bounciness = 0.5f;
            circleCollider.sharedMaterial.bounciness = bounciness;
            rigidBody.gravityScale = 0.0f;
            pullForce = new Vector2(1, 0);
            holdForce = new Vector2(100, 0);
            pushForce = new Vector2(25, 0);
            downForce = new Vector2(0, -500);
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
                circleCollider.sharedMaterial.bounciness = bounciness;
            }
            else if (enteredPlatform) {
                if (pulledToMiddle && pulleyBehavior.grounded)
                {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x / 2, rigidBody.velocity.y);
                    circleCollider.sharedMaterial.bounciness = 0;
                    rigidBody.AddForce(downForce);
                    if (transform.position.x < pulleyPosition.x)
                    {
                        rigidBody.AddForce(holdForce);

                    }
                    else if (transform.position.x > pulleyPosition.x)
                    {
                        rigidBody.AddForce(-1 * holdForce);
                    }
                }
                else if (!pulleyBehavior.grounded)
                {
                    circleCollider.sharedMaterial.bounciness = bounciness;
                    if (pulleyBehavior.ballRollDirection.Equals(PulleyBehavior.BallRollDirection.Right)){
                        rigidBody.AddForce(pushForce);
                    }
                    else 
                    {
                        rigidBody.AddForce(-1 * pushForce);
                    }
                }
            }
            if (rigidBody.velocity.magnitude > 0.01f && !AudioManager.instance.isPlaying(RollingSound.name) && isTouching && !enteredPlatform) 
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
                circleCollider.sharedMaterial.bounciness = bounciness;
                rigidBody.gravityScale = 1.0f;
                AudioManager.instance.SetVolume(RollingSound.name, rollingVolume);
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform.parent.parent.gameObject.CompareTag("SimpleMachine"))
            {
                if (other.transform.parent.parent.gameObject.GetComponent<PlacedObjectManager>().metaData.Equals(simplePulleyMetaData)
                    || other.transform.parent.parent.gameObject.GetComponent<PlacedObjectManager>().metaData.Equals(compoundPulleyMetaData))
                {
                    pulledToMiddle = false;
                    enteredPlatform = !enteredPlatform;
                    pulleyBehavior = other.transform.parent.parent.gameObject.GetComponent<PulleyBehavior>();
                    pulleyPosition = other.transform.position;
                    if (other.offset.x > 0 && rigidBody.velocity.x < 0)
                    {
                        StartCoroutine(PulltoMiddle(-1 * pullForce));
                    }
                    else if (other.offset.x < 0 && rigidBody.velocity.x > 0)
                    {
                        StartCoroutine(PulltoMiddle(pullForce));
                    }
                    else
                    {
                        enteredPlatform = false;
                    }
                }
            }
        }

        private IEnumerator PulltoMiddle(Vector2 force)
        {
            while (transform.position.x < (pulleyPosition.x - 0.1f)
                    || transform.position.x > (pulleyPosition.x + 0.1f))
            {
                rigidBody.AddForce(force);
                yield return new WaitForFixedUpdate();
            }
            pulledToMiddle = true;
        }
        private IEnumerator ReleaseAfterDelay()
        {
            yield return new WaitForSeconds(delayAfterRelease);
            GetComponent<SpringJoint2D>().enabled = false;
        }

        public IEnumerator AsyncResetPosition()
        {
            yield return new WaitUntil(() => transform != null && rigidBody != null);
            ResetPosition();
        }

        private void ResetPosition()
        {
            transform.position = GetSlingAnchorPosition();
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            rigidBody.gravityScale = 0f;
            GetComponent<SpringJoint2D>().enabled = true;
            hasBeenReleased = false;
            enteredPlatform = false;
        }
    }
}