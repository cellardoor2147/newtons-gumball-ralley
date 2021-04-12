using Core;
using UnityEngine;

namespace DestructibleObject
{
    public class DestructibleObstacle : MonoBehaviour, IDestructible
    {
        [SerializeField] float breakSpeed;
        [SerializeField] UnityEngine.Object destructibleRef;
        private RaycastHit2D[] contacts = new RaycastHit2D[1];

        float localVel = 0f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameStateManager.GetGameState() == GameState.Playing)
            {
                if (collision.name == "AxeWedgeCollider")
                {
                    localVel = collision.transform.InverseTransformDirection(collision.attachedRigidbody.velocity).x;
                }

                if (collision.name == "SpikeWedgeCollider")
                {
                    localVel = -collision.transform.InverseTransformDirection(collision.attachedRigidbody.velocity).y;
                }

                bool isWedge = collision.name == "AxeWedgeCollider" || collision.name == "SpikeWedgeCollider";

                Debug.Log(localVel);
                if (isWedge && localVel > breakSpeed)
                {
                    collision.Raycast(Vector2.down, contacts);
                    Split(contacts[0]);
                }
            }
        }

        public void Split(RaycastHit2D contactPoint)
        {
            GameObject destructible = Instantiate(destructibleRef, contactPoint.point, transform.rotation) as GameObject;
            destructible.transform.localScale = transform.lossyScale;
            destructible.transform.parent = this.transform.parent;
            ToggleObject(false);
        }

        public void ToggleObject(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}