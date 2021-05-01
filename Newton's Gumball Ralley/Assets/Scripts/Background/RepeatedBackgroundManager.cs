using UnityEngine;
using Core.Levels;

namespace Background
{
    public class RepeatedBackgroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject backgroundTexture;
        [SerializeField] private Color backgroundTintForWorld1;
        [SerializeField] private Color backgroundTintForWorld2;
        [SerializeField] private Color backgroundTintForWorld3;
        [SerializeField] private Color backgroundTintForWorld4;
        [SerializeField] private Color backgroundTintForWorld5;
        [SerializeField] private Color backgroundTintForWorld6;

        private static RepeatedBackgroundManager instance;

        private float backgroundTextureWidth;
        private float backgroundTextureHeight;
        private int desiredNumberOfRows = 8;
        private int desiredNumberOfColumns = 15;

        private RepeatedBackgroundManager() { } // prevent instantiation outside this class

        private void Awake()
        {
            SetInstance();
            backgroundTextureWidth =
                backgroundTexture.GetComponent<SpriteRenderer>().size.x *
                backgroundTexture.transform.localScale.x;
            backgroundTextureHeight =
                backgroundTexture.GetComponent<SpriteRenderer>().size.y *
                backgroundTexture.transform.localScale.y;
            RenderRepeatedBackground();
        }

        private void SetInstance()
        {
            if (instance != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void RenderRepeatedBackground()
        {
            ClearPreviouslyRenderedBackground();
            float nextRenderPositionX = GetStartingRenderPositionX();
            for (int currCol = 0; currCol < desiredNumberOfColumns; currCol++)
            {
                float nextRenderPositionY = GetStartingRenderPositionY();
                for (int currRow = 0; currRow < desiredNumberOfRows; currRow++)
                {
                    GameObject nextBackgroundTexture =
                        Instantiate(backgroundTexture, transform);
                    nextBackgroundTexture.GetComponent<SpriteRenderer>().color =
                        GetBackgroundColor();
                    nextBackgroundTexture.transform.position =
                        new Vector2(nextRenderPositionX, nextRenderPositionY);
                    nextRenderPositionY += backgroundTextureHeight;
                }
                nextRenderPositionX += backgroundTextureWidth;
            }
        }

        private void ClearPreviouslyRenderedBackground()
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

        private float GetStartingRenderPositionX()
        {
            bool thereIsAnOddNumberOfDesiredColumns = desiredNumberOfColumns % 2 == 1;
            int numberOfColumnsToDrawToTheLeft;
            float offsetAmount;
            if (thereIsAnOddNumberOfDesiredColumns)
            {
                numberOfColumnsToDrawToTheLeft = (desiredNumberOfColumns - 1) / 2;
                offsetAmount = 0.0f;
            }
            else
            {
                numberOfColumnsToDrawToTheLeft = (desiredNumberOfColumns - 2) / 2;
                offsetAmount = backgroundTextureWidth / 2;
            }
            return -((numberOfColumnsToDrawToTheLeft * backgroundTextureWidth) + offsetAmount);
        }

        private float GetStartingRenderPositionY()
        {
            bool thereIsAnOddNumberOfDesiredRows = desiredNumberOfRows % 2 == 1;
            int numberOfRowsToDrawToBelow;
            float offsetAmount;
            if (thereIsAnOddNumberOfDesiredRows)
            {
                numberOfRowsToDrawToBelow = (desiredNumberOfRows - 1) / 2;
                offsetAmount = 0.0f;
            }
            else
            {
                numberOfRowsToDrawToBelow = (desiredNumberOfRows - 2) / 2;
                offsetAmount = backgroundTextureHeight / 2;
            }
            return -((numberOfRowsToDrawToBelow * backgroundTextureHeight) + offsetAmount);
        }

        public static void SetDesiredNumberOfColumnsAndRows(
            int desiredNumberOfColumns,
            int desiredNumberOfRows
        )
        {
            instance.desiredNumberOfColumns = desiredNumberOfColumns;
            instance.desiredNumberOfRows = desiredNumberOfRows;
            instance.RenderRepeatedBackground();
        }

        public static float GetBorderLeftPositionX()
        {
            float halfScreenWidth = Camera.main.orthographicSize * Camera.main.aspect;
            return instance.GetStartingRenderPositionX()
                + halfScreenWidth
                - instance.backgroundTextureWidth / 2;
        }

        public static float GetBorderRightPositionX()
        {
            return -GetBorderLeftPositionX();
        }

        public static float GetBorderUpPositionY()
        {
            float halfScreenHeight = Camera.main.orthographicSize;
            return instance.GetStartingRenderPositionY()
                + halfScreenHeight
                - instance.backgroundTextureHeight / 2;
        }

        public static float GetBorderDownPositionY()
        {
            return -GetBorderUpPositionY();
        }

        public static Color GetBackgroundColor()
        {
            switch(LevelManager.GetCurrentWorldIndex())
            {
                case 1:
                    return instance.backgroundTintForWorld1;
                case 2:
                    return instance.backgroundTintForWorld2;
                case 3:
                    return instance.backgroundTintForWorld3;
                case 4:
                    return instance.backgroundTintForWorld4;
                case 5:
                    return instance.backgroundTintForWorld5;
                case 6:
                    return instance.backgroundTintForWorld6;
                default:
                    return Color.white;
            }
        }
    }
}
