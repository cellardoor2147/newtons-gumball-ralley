using UnityEngine;

namespace SimpleMachine
{
    public class RotatingController : MonoBehaviour
    {
        [SerializeField] private float rotationMagnitude;

        private GameObject objectToRotate;

        public void SetObjectToRotate(GameObject objectToRotate)
        {
            this.objectToRotate = objectToRotate;
        }

        private void OnMouseDown()
        {
            objectToRotate.GetComponent<DraggingController>().Rotate(rotationMagnitude);
        }
    }
}
