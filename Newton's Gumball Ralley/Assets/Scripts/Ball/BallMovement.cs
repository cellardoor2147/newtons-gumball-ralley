using System.Collections;
using UnityEngine;
using Core.PlacedObjects;
using Core;
using LevelTimer;
using Audio;
using SimpleMachine;
using MainCamera;

namespace Ball
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BallMovement : MonoBehaviour
    {
        [SerializeField] private float baseMovementSpeed = 1.0f;
        [SerializeField] private float maxRadiusOfPull = 2.0f;
        [SerializeField] private float delayAfterRelease = 0.3f;
        [SerializeField] private float maxVelocityOnRelease = 3f;
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
        private Vector2 pulleyPosition;
        private bool pulledToMiddle;
        private bool enteredPlatform;
        private Transform parent;
        private PulleyBehavior pulleyBehavior;
        private bool isFading;
        private bool isTouching;
        private Rigidbody2D rigidBody;
        private bool isBeingPulled;
        private bool hasBeenReleased;
        private SpriteRenderer spriteRenderer;

        public Vector3 originalScale;
        public bool hasBeenDispensed;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0.0f;
            pullForce = new Vector2(1, 0);
            holdForce = new Vector2(100, 0);
            pushForce = new Vector2(25, 0);
            parent = transform.parent;
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;
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
            if (!isFading)
            {
                AudioManager.instance.SetVolume(RollingSound.name, Mathf.Clamp(rigidBody.velocity.magnitude / 20, 0, rollingVolume));
            }
            if (!hasBeenReleased || !GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                AudioManager.instance.StopSound(RollingSound.name);
                UpdateBallPositionRelativeToSling();
            }
            else if (enteredPlatform) {
                if (pulledToMiddle && pulleyBehavior.grounded)
                {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x / 2, rigidBody.velocity.y);
                }
                else if (!pulleyBehavior.grounded)
                {
                    transform.parent = parent;
                    if (pulleyBehavior.ballRollDirection.Equals(PulleyBehavior.BallRollDirection.Right)){
                        rigidBody.AddForce(pushForce);
                    }
                    else 
                    {
                        rigidBody.AddForce(-1 * pushForce);
                    }
                }
            }
            if (hasBeenReleased && rigidBody.velocity.magnitude > 0.01f && !AudioManager.instance.isPlaying(RollingSound.name) 
                && isTouching && !enteredPlatform && GameStateManager.GetGameState().Equals(GameState.Playing)) 
            {
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
            if (!GameStateManager.GetGameState().Equals(GameState.Playing) || !hasBeenDispensed)
            {
                return;
            }
            if (!hasBeenReleased)
            {
                CameraMovement.shouldPreventDragging = true;
                isBeingPulled = true;
            }
        }

        private void OnMouseUp()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing) || !hasBeenDispensed)
            {
                return;
            }
            if (!hasBeenReleased)
            {
                CameraMovement.shouldPreventDragging = false;
                isBeingPulled = false;
                hasBeenReleased = true;
                rigidBody.gravityScale = 1.0f;
                AudioManager.instance.SetVolume(RollingSound.name, rollingVolume);
                StartCoroutine(ReleaseAfterDelay());
            }
        }

        private void OnCollisionEnter2D(Collision2D other) 
        {
            if (hasBeenReleased && GameStateManager.GetGameState().Equals(GameState.Playing)) 
            {
                isTouching = true;
                AudioManager.instance.PlaySound(BounceSound.name);
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (hasBeenReleased && GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                isTouching = true;
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
            if (other.gameObject.name.Contains("FlatPlatform"))
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
                        StartCoroutine(PulltoMiddle(-1 * pullForce, other.gameObject));
                    }
                    else if (other.offset.x < 0 && rigidBody.velocity.x > 0)
                    {
                        StartCoroutine(PulltoMiddle(pullForce, other.gameObject));
                    }
                    else
                    {
                        enteredPlatform = false;
                    }
                }
            }
        }

        private IEnumerator PulltoMiddle(Vector2 force, GameObject pulleyPlatform)
        {
            while (transform.position.x < (pulleyPosition.x - 0.1f)
                    || transform.position.x > (pulleyPosition.x + 0.1f))
            {
                rigidBody.AddForce(force);
                yield return new WaitForFixedUpdate();
            }
            SetAsChild(pulleyPlatform);
            pulledToMiddle = true;
        }
        private IEnumerator ReleaseAfterDelay()
        {
            ClampVelocityOnRelease();
            yield return new WaitForSeconds(delayAfterRelease);
            ClampVelocityOnRelease();
            Timer.Start();
            GetComponent<SpringJoint2D>().enabled = false;
        }

        private void ClampVelocityOnRelease()
        {
            rigidBody.velocity = new Vector2(
                Mathf.Clamp(rigidBody.velocity.x, -maxVelocityOnRelease, maxVelocityOnRelease),
                Mathf.Clamp(rigidBody.velocity.y, -maxVelocityOnRelease, maxVelocityOnRelease)
            );
        }

        public IEnumerator AsyncReset()
        {
            yield return new WaitUntil(() => transform != null && rigidBody != null);
            Reset();
        }

        private void Reset()
        {
            StopAllCoroutines();
            transform.position = GetSlingAnchorPosition();
            transform.localScale = originalScale;
            transform.parent = parent;
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            rigidBody.gravityScale = 0f;
            rigidBody.constraints = RigidbodyConstraints2D.None;
            GetComponent<SpringJoint2D>().enabled = true;
            hasBeenReleased = false;
            enteredPlatform = false;
            isTouching = false;
            AudioManager.instance.StopSound(RollingSound.name);
            spriteRenderer.color = new Color(
                spriteRenderer.color.r,
                spriteRenderer.color.g,
                spriteRenderer.color.b,
                1f
            );
            hasBeenDispensed = false;
        }

        public void Die()
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            StartCoroutine(FadeOutThenTriggerGameOverPopup());
        }

        private IEnumerator FadeOutThenTriggerGameOverPopup()
        {
            while (spriteRenderer.color.a > 0f)
            {
                spriteRenderer.color = new Color(
                    spriteRenderer.color.r,
                    spriteRenderer.color.g,
                    spriteRenderer.color.b,
                    spriteRenderer.color.a - Time.deltaTime
                );
                yield return new WaitForSeconds(Time.deltaTime);
            }
            GameStateManager.SetGameState(GameState.GameOver);
        }
        private void SetAsChild(GameObject pulleyPlatform)
        {
            transform.parent = pulleyPlatform.transform;
        }
    }
}