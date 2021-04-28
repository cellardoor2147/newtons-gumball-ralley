using Ball;
using Core;
using UnityEngine;

namespace Obstacle
{
    public class SpikeBehavior : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            bool shouldKillBall = collision.gameObject.tag.Equals("Player")
                && GameStateManager.GetGameState().Equals(GameState.Playing);
            if (shouldKillBall)
            {
                collision.gameObject.GetComponent<BallMovement>().Die();
            }
        }
    }
}