using Ball;
using UnityEngine;

namespace Obstacle
{
    public class SpikeBehavior : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            bool collidedWithBall = collision.gameObject.tag.Equals("Player");
            if (collidedWithBall)
            {
                collision.gameObject.GetComponent<BallMovement>().Die();
            }
        }
    }
}