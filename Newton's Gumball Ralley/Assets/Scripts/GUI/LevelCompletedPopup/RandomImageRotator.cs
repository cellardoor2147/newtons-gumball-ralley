using UnityEngine;

namespace GUI.LevelCompletedPopup
{
    public class RandomImageRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;

        private bool shouldRotate;

        private void Update()
        {
            if (shouldRotate)
            {
                transform.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.deltaTime));
            }
        }

        public void SetShouldRotate(bool shouldRotate)
        {
            this.shouldRotate = shouldRotate;
        }
    }
}
