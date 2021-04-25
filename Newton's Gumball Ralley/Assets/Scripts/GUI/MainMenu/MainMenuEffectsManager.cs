using System.Collections;
using UnityEngine;

namespace GUI.MainMenu
{
    public class MainMenuEffectsManager : MonoBehaviour
    {
        private static readonly string MAX_IMAGE_KEY = "Max Image";
        private static readonly string QUINN_IMAGE_KEY = "Quinn Image";

        private RectTransform maxTransform;
        private RectTransform quinnTransform;
        private float maxTransformStartY;
        private float maxTransfromEndY;
        private float quinnTransformStartY;
        private float quinnTransformEndY;

        private void Awake()
        {
            maxTransform = transform.Find(MAX_IMAGE_KEY).GetComponent<RectTransform>();
            quinnTransform = transform.Find(QUINN_IMAGE_KEY).GetComponent<RectTransform>();
            maxTransformStartY = -(maxTransform.rect.height / 2f);
            maxTransfromEndY = maxTransform.rect.height / 2.6f;
            quinnTransformStartY = -(quinnTransform.rect.height / 2f);
            quinnTransformEndY = quinnTransform.rect.height / 2.6f;
        }

        private void OnEnable()
        {
            StartCoroutine(RaiseMaxAndQuinnTransforms());
        }

        private void OnDisable()
        {
            ResetMaxAndQuinnTransforms();
        }

        private void ResetMaxAndQuinnTransforms()
        {
            maxTransform.anchoredPosition = new Vector2(
                maxTransform.anchoredPosition.x,
                maxTransformStartY
            );
            quinnTransform.anchoredPosition = new Vector2(
                quinnTransform.anchoredPosition.x,
                quinnTransformStartY
            );
        }

        private IEnumerator RaiseMaxAndQuinnTransforms()
        {
            yield return new WaitUntil(
                () => maxTransform != null && quinnTransform != null
            );
            while (
                maxTransform.anchoredPosition.y < maxTransfromEndY
                && quinnTransform.anchoredPosition.y < quinnTransformEndY
            )
            {
                MoveRectTransformUp(maxTransform);
                MoveRectTransformUp(quinnTransform);
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        private void MoveRectTransformUp(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                rectTransform.anchoredPosition.y + (Time.deltaTime * 350f)
            );
        }
    }
}
