using UnityEngine;

namespace FulcrumScrew
{
    public class FulcrumScrewBehavior : MonoBehaviour
    {
        private bool collidedWithLeverPlatform = false;
        private Collider2D screwCollider;
        private Rigidbody2D fulcrumRB;

        void Start()
        {
            screwCollider = GetComponent<Collider2D>();
            fulcrumRB = GetComponentInParent<Rigidbody2D>();
        }

        void Update()
        {
            if (collidedWithLeverPlatform)
            {
                screwCollider.enabled = false;
                fulcrumRB.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            collidedWithLeverPlatform = collision.gameObject.GetComponent<Rigidbody2D>() != null
                && collision.gameObject.name == ("LeverPlatform");
            if (collidedWithLeverPlatform)
            {
                collision.gameObject.AddComponent<HingeJoint2D>();
                transform.SetParent(collision.transform, true);
                collision.gameObject.GetComponent<HingeJoint2D>().anchor = gameObject.transform.localPosition;
                collision.gameObject.GetComponent<HingeJoint2D>().enableCollision = true;
            }
        }
    }
}