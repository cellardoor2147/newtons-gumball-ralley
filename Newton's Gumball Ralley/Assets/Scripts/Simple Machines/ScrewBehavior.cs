using UnityEngine;

namespace Screw
{
    public class ScrewBehavior : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                if (collision.gameObject.CompareTag("SimpleMachine"))
                {
                    collision.gameObject.AddComponent<HingeJoint2D>();
                    transform.SetParent(collision.transform, true);
                    collision.gameObject.GetComponent<HingeJoint2D>().anchor = gameObject.transform.localPosition;
                    collision.gameObject.GetComponent<HingeJoint2D>().enableCollision = true;
                }
            }
        }
    }
}
