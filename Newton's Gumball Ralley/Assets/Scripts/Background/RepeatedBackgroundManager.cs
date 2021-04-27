using UnityEngine;

namespace Background
{
    public class RepeatedBackgroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject backgroundTexture;

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
    }
}
