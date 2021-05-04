using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.MainMenu
{
    public class MainMenuEffectsManager : MonoBehaviour
    {
        private static readonly string GUMBALL_CONTAINER_KEY = "Gumball Container";
        private static readonly string MAX_BODY_KEY = "Max Body";
        private static readonly string MAX_ARM_KEY = "Max Arm";
        private static readonly string QUINN_BODY_KEY = "Quinn Body";
        private static readonly string QUINN_ARM_KEY = "Quinn Arm";

        [SerializeField] private GameObject guiGumballPrefab;
        [SerializeField] private float gumballSpawnDelay;
        [SerializeField] private float gumballFallSpeed;
        [SerializeField] private float gumballImageSize;
        [SerializeField] private List<Sprite> gumballSprites;
        [SerializeField] private float armRotationLimit;
        [SerializeField] private float armRotationSpeed;

        private GameObject gumballContainer;
        private float spawnedGumballStartX;
        private float spawnedGumballEndX;
        private float spawnedGumballStartY;
        private float spawnedGumballEndY;
        private RectTransform maxTransform;
        private RectTransform maxArmTransform;
        private RectTransform quinnTransform;
        private RectTransform quinnArmTransform;
        private float maxTransformStartY;
        private float maxTransfromEndY;
        private float quinnTransformStartY;
        private float quinnTransformEndY;

        private void Awake()
        {
            gumballContainer = transform.Find(GUMBALL_CONTAINER_KEY).gameObject;
            Vector2 gumballContainerSize =
                gumballContainer.GetComponent<RectTransform>().rect.size;
            spawnedGumballStartX = -gumballContainerSize.x;
            spawnedGumballEndX = gumballContainerSize.x;
            spawnedGumballStartY = (gumballContainerSize.y / 2) + (gumballImageSize / 2);
            spawnedGumballEndY = -spawnedGumballStartY;
            maxTransform = transform.Find(MAX_BODY_KEY).GetComponent<RectTransform>();
            maxArmTransform = maxTransform.Find(MAX_ARM_KEY).GetComponent<RectTransform>();
            quinnTransform = transform.Find(QUINN_BODY_KEY).GetComponent<RectTransform>();
            quinnArmTransform = quinnTransform.Find(QUINN_ARM_KEY).GetComponent<RectTransform>();
            maxTransformStartY = -(maxTransform.rect.height / 2f);
            maxTransfromEndY = maxTransform.rect.height / 3f;
            quinnTransformStartY = -(quinnTransform.rect.height / 2f);
            quinnTransformEndY = quinnTransform.rect.height / 3f;
        }

        private void Update()
        {
            LowerRandomlySpawnedGumballs();
            DeleteOffScreenRandomlySpawnedGumballs();
        }

        private void OnEnable()
        {
            StartCoroutine(RandomlySpawnGumballsInBackground());
            StartCoroutine(RaiseMaxAndQuinnTransforms());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            ResetMaxAndQuinnTransforms();
        }

        private IEnumerator RandomlySpawnGumballsInBackground()
        {
            while (true)
            {
                RandomlySpawnGumball();
                yield return new WaitForSeconds(gumballSpawnDelay);
            }
        }

        private void RandomlySpawnGumball()
        {
            int chosenGumballSpriteIndex =
                Mathf.RoundToInt(Random.Range(0f, gumballSprites.Count - 1));
            Sprite chosenGumballSprite = gumballSprites[chosenGumballSpriteIndex];
            Vector2 chosenGumballPosition = new Vector2(
                Random.Range(spawnedGumballStartX, spawnedGumballEndX),
                spawnedGumballStartY
            );
            GameObject randomlySpawnedGumball = Instantiate(guiGumballPrefab, gumballContainer.transform);
            randomlySpawnedGumball.transform.localPosition = chosenGumballPosition;
            randomlySpawnedGumball.GetComponent<Image>().sprite = chosenGumballSprite;
        }

        private void LowerRandomlySpawnedGumballs()
        {
            foreach (Transform childTransform in gumballContainer.transform)
            {
                childTransform.GetComponent<RectTransform>().anchoredPosition +=
                    Vector2.down * Time.deltaTime * gumballFallSpeed;
            }
        }

        private void DeleteOffScreenRandomlySpawnedGumballs()
        {
            for (int i = gumballContainer.transform.childCount - 1; i >= 0; i--)
            {
                Transform randomlySpawnedGumballTransform =
                    gumballContainer.transform.GetChild(i).transform;
                bool randomlySpawnedGumballIsOffScreen =
                    randomlySpawnedGumballTransform.localPosition.y < spawnedGumballEndY;
                if (randomlySpawnedGumballIsOffScreen)
                {
                    Destroy(randomlySpawnedGumballTransform.gameObject);
                }
            }
        }

        private void ResetMaxAndQuinnTransforms()
        {
            maxTransform.anchoredPosition = new Vector2(
                maxTransform.anchoredPosition.x,
                maxTransformStartY
            );
            maxArmTransform.localEulerAngles = Vector3.zero;
            quinnTransform.anchoredPosition = new Vector2(
                quinnTransform.anchoredPosition.x,
                quinnTransformStartY
            );
            quinnArmTransform.localEulerAngles = Vector3.zero;
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
            yield return RotateMaxAndQuinArmTransforms();
        }

        private void MoveRectTransformUp(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                rectTransform.anchoredPosition.y + (Time.deltaTime * 350f)
            );
        }

        private IEnumerator RotateMaxAndQuinArmTransforms()
        {
            float maxArmRotationDirection = -1f;
            float quinnArmRotationDirection = 1f;
            while (true)
            {
                maxArmRotationDirection = RotateArmTransformAndGetRotationDirection(
                    maxArmTransform,
                    maxArmRotationDirection
                );
                quinnArmRotationDirection = RotateArmTransformAndGetRotationDirection(
                    quinnArmTransform,
                    quinnArmRotationDirection
                );
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        private float RotateArmTransformAndGetRotationDirection(
            RectTransform armTransform,
            float armRotationDirection
        )
        {
            armTransform.localEulerAngles = new Vector3(
                armTransform.localEulerAngles.x,
                armTransform.localEulerAngles.y,
                armTransform.localEulerAngles.z
                    + (armRotationDirection * armRotationSpeed * Time.deltaTime)
            );
            if (armTransform.localEulerAngles.z > 360f - armRotationLimit)
            {
                return 1f;
            }
            if (armTransform.localEulerAngles.z > armRotationLimit)
            {
                return -1f;
            }
            return armRotationDirection;
        }
    }
}
