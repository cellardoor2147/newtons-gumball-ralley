using UnityEngine;

namespace DestructibleObject
{
    public class DestructibleObstacle : MonoBehaviour, IDestructible
    {
        [SerializeField] float breakSpeed;
        [SerializeField] UnityEngine.Object destructibleRef;

        float localVel = 0f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.name == "AxeWedgeCollider")
            {
                localVel = collision.transform.InverseTransformDirection(collision.rigidbody.velocity).x;
            }

            if (collision.collider.name == "SpikeWedgeCollider")
            {
                localVel = -collision.transform.InverseTransformDirection(collision.rigidbody.velocity).y;
            }

            bool isWedge = collision.collider.name == "AxeWedgeCollider" || collision.collider.name == "SpikeWedgeCollider";

            if (isWedge && localVel > breakSpeed)
            {
                Split(collision.GetContact(0));
            }
        }

        public void Split(ContactPoint2D contactPoint)
        {
            GameObject destructible = Instantiate(destructibleRef, contactPoint.point, transform.rotation) as GameObject;
            destructible.transform.localScale = transform.lossyScale;
            Destroy(this.gameObject);
        }
    }
}