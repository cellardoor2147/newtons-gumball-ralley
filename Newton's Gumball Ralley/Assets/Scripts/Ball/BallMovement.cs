using System.Collections;
using UnityEngine;
using Core;

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

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.gravityScale = 0.0f;
        }

        private void FixedUpdate()
        {
            if (!hasBeenReleased)
            {
                UpdateBallPositionRelativeToSling();
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

        private IEnumerator ReleaseAfterDelay()
        {
            yield return new WaitForSeconds(delayAfterRelease);
            GetComponent<SpringJoint2D>().enabled = false;
        }
    }
}