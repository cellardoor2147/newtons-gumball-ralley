using UnityEngine;
using Core;

public class GearBehavior : MonoBehaviour
{
 
    private Rigidbody2D rigidbody2D;
    private Vector3 ConstrainedPosition;
    private bool hasSet;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        if (GameStateManager.GetGameState().Equals(GameState.Playing)) 
        {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.position = transform.position = ConstrainedPosition;
        }
    } 
}
