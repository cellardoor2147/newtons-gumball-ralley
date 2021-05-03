using UnityEngine;
using Core.PlacedObjects;

namespace SimpleMachine
{
    public class FulcrumScrewBehavior : MonoBehaviour
    {
        public bool FulcrumJointShouldBeCreated { get; private set; } = false;
        private Rigidbody2D fulcrumRB;

        void Start()
        {
            fulcrumRB = GetComponentInParent<Rigidbody2D>();
        }

        void Update()
        {
            if (FulcrumJointShouldBeCreated && fulcrumRB != null)
            {
                fulcrumRB.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            FulcrumJointShouldBeCreated = ShouldAttachToLeverPlatform(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            FulcrumJointShouldBeCreated = false;
        }

        private bool ShouldAttachToLeverPlatform(Collider2D collision)
        {
            return collision.gameObject.GetComponent<PlacedObjectManager>() != null
                && collision.gameObject.GetComponent<PlacedObjectManager>().metaData.name.Equals("LeverPlatform");
        }
    }
}