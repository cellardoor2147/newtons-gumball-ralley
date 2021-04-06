using UnityEngine;

namespace DestructibleObject
{
    public class DestructibleObstacle : MonoBehaviour, IDestructible
    {
        [SerializeField] float breakSpeed;
        [SerializeField] UnityEngine.Object destructibleRef;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            float localYVel = collision.transform.InverseTransformDirection(collision.rigidbody.velocity).y;
            if (collision.collider.name == "Wedge" && Mathf.Abs(localYVel) > breakSpeed)
            {
                Split(collision.transform);
            }
        }

        public void Split(Transform hitTransform)
        {
            GameObject destructible = Instantiate(destructibleRef, hitTransform.position, hitTransform.rotation) as GameObject;
            destructible.transform.localScale = transform.lossyScale;
            Destroy(this.gameObject);
        }
    }
}