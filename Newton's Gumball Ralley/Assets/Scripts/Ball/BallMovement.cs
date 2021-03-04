using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallMovement : MonoBehaviour
{
    [SerializeField] private float baseMovementSpeed = 1.0f;

    private Rigidbody2D rigidBody;
    private Vector2 movementVector;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movementVector = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

    private void FixedUpdate()
    {
        rigidBody.AddForce(movementVector * baseMovementSpeed, ForceMode2D.Impulse);
    }

    public void Launch(Vector2 launchForce)
    {
        rigidBody.AddForce(launchForce);
    }
}
