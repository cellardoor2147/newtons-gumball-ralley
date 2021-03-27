using UnityEngine;

namespace Screw
{
    public class ScrewBehavior : MonoBehaviour
    {
        private bool collidedWithSimpleMachine = false;
        private Collider2D screwCollider;

        void Start()
        {
            screwCollider = GetComponent<Collider2D>();
        }
        
        void Update()
        {
            if (!collidedWithSimpleMachine)
                screwCollider.enabled = false;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            collidedWithSimpleMachine = collision.gameObject.GetComponent<Rigidbody2D>() != null
                && collision.gameObject.CompareTag("SimpleMachine");
            if (collidedWithSimpleMachine)
            {
                collision.gameObject.AddComponent<HingeJoint2D>();
                transform.SetParent(collision.transform, true);
                collision.gameObject.GetComponent<HingeJoint2D>().anchor = gameObject.transform.localPosition;
                collision.gameObject.GetComponent<HingeJoint2D>().enableCollision = true;
            }
        }
    }
}
