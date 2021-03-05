using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallMovement : MonoBehaviour
{
    [SerializeField] private float baseMovementSpeed = 1.0f;
    [SerializeField] private float maxRadiusOfPull = 2.0f;
    [SerializeField] private float delayAfterRelease = 0.3f;

    private Rigidbody2D rigidBody;
    private Vector2 movementVector;
    private bool isBeingPulled;
    private bool hasBeenReleased;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateMovementVector();
    }

    private void UpdateMovementVector()
    {
        movementVector = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

    private void FixedUpdate()
    {
        if (!hasBeenReleased)
        {
            UpdateBallPositionRelativeToSling();
        }
        else
        {
            AddForce(movementVector * baseMovementSpeed, ForceMode2D.Impulse);
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
        Vector2 clampedDistanceBetweenMouseAndSpawnPositions =
            Mathf.Min(maxRadiusOfPull, distanceBetweenMouseAndSlingPositions.magnitude) *
            distanceBetweenMouseAndSlingPositions.normalized;
        rigidBody.position = GetSlingAnchorPosition() + clampedDistanceBetweenMouseAndSpawnPositions;
    }

    private Vector2 GetSlingAnchorPosition()
    {
        return transform.parent.position;
    }

    public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        rigidBody.AddForce(force, forceMode);
    }

    private void OnMouseDown()
    {
        if (!hasBeenReleased)
        {
            isBeingPulled = true;
            rigidBody.isKinematic = true;
        }
    }

    private void OnMouseUp()
    {
        if (!hasBeenReleased)
        {
            isBeingPulled = false;
            rigidBody.isKinematic = false;
            hasBeenReleased = true;
            StartCoroutine(ReleaseAfterDelay());
        }
    }

    private IEnumerator ReleaseAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterRelease);
        GetComponent<SpringJoint2D>().enabled = false;
    }
}
