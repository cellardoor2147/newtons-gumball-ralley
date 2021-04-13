using UnityEngine;

namespace GUI.LevelCompletedPopup
{
    public class RandomImageRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;

        private void Awake()
        {
            transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.deltaTime));
        }
    }
}
