using System.Collections;
using UnityEngine;

namespace Hints
{
    public class HintArrowEffectsManager : MonoBehaviour
    {
        [SerializeField] public float minScale;
        [SerializeField] public float maxScale;
        [SerializeField] public float lifetimeDelay;

        private float timer;
        private Vector3 minScaleVector;
        private Vector3 maxScaleVector;
        private bool isGrowing;
        private SpriteRenderer arrowSpriteRenderer;
        private TextMesh hintTextMesh;

        private void Awake()
        {
            SetMinScaleVector(minScale);
            SetMaxScaleVector(maxScale);
            arrowSpriteRenderer = GetComponent<SpriteRenderer>();
            hintTextMesh = transform.parent.GetComponent<TextMesh>();
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

        private void OnEnable()
        {
            arrowSpriteRenderer.color = Color.black;
            hintTextMesh.color = Color.black;
            StartCoroutine(WaitForLifetimeDelayThenFadeOut());
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

        private IEnumerator WaitForLifetimeDelayThenFadeOut()
        {
            yield return new WaitForSeconds(lifetimeDelay);
            while (arrowSpriteRenderer.color.a > 0f)
            {
                arrowSpriteRenderer.color = new Color(
                    arrowSpriteRenderer.color.r,
                    arrowSpriteRenderer.color.g,
                    arrowSpriteRenderer.color.b,
                    arrowSpriteRenderer.color.a - Time.deltaTime
                );
                hintTextMesh.color = new Color(
                    hintTextMesh.color.r,
                    hintTextMesh.color.g,
                    hintTextMesh.color.b,
                    hintTextMesh.color.a - Time.deltaTime
                );
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
