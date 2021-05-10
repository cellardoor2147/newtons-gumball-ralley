using UnityEngine;

namespace Hints
{
    public class HintArrowEffectsManager : MonoBehaviour
    {
        [SerializeField] public float minScale;
        [SerializeField] public float maxScale;

        private float timer;
        private Vector3 minScaleVector;
        private Vector3 maxScaleVector;
        private bool isGrowing;

        private void Awake()
        {
            SetMinScaleVector(minScale);
            SetMaxScaleVector(maxScale);
        }

        public void SetMinScaleVector(float minScaleFactor)
        {
            minScaleVector = GetScaleVector(minScaleFactor);
        }

        public void SetMaxScaleVector(float maxScaleFactor)
        {
            maxScaleVector = GetScaleVector(maxScaleFactor);
        }

        private Vector3 GetScaleVector(float scaleFactor)
        {
            return new Vector3(scaleFactor, scaleFactor, transform.localScale.z);
        }

        private void Update()
        {
            transform.localScale = isGrowing
                ? Vector3.Lerp(minScaleVector, maxScaleVector, timer)
                : Vector3.Lerp(maxScaleVector, minScaleVector, timer);
            if (timer >= 1f)
            {
                timer = 0f;
                isGrowing = !isGrowing;
            }
            timer += Time.deltaTime;
        }
    }
}
