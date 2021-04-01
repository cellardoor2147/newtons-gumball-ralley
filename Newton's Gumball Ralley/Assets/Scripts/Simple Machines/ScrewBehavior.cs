using UnityEngine;
using Core;

namespace Screw
{
    public class ScrewBehavior : MonoBehaviour
    {
        private Collider2D screwCollider;

        void Start()
        {
            screwCollider = GetComponent<Collider2D>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (ShouldAttachToSimpleMachine(collision))
            {
                collision.gameObject.AddComponent<HingeJoint2D>();
                transform.SetParent(collision.transform, true);
                collision.gameObject.GetComponent<HingeJoint2D>().anchor = gameObject.transform.localPosition;
                collision.gameObject.GetComponent<HingeJoint2D>().enableCollision = true;
            }
        }

        private bool ShouldAttachToSimpleMachine(Collider2D collision)
        {
            return GameStateManager.GetGameState().Equals(GameState.Editing)
                && collision.gameObject.GetComponent<Rigidbody2D>() != null
                && collision.gameObject.CompareTag("SimpleMachine");
        }
    }
}
