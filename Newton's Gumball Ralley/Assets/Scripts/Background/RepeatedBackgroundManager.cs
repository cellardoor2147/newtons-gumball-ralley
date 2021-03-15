using UnityEngine;

namespace Background
{
    public class RepeatedBackgroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject backgroundTexture;
        [SerializeField] private int desiredNumberOfRows = 5;
        [SerializeField] private int desiredNumberOfColumns = 5;

        private float backgroundTextureWidth;
        private float backgroundTextureHeight;

        private void Awake()
        {
            backgroundTextureWidth =
                backgroundTexture.GetComponent<SpriteRenderer>().size.x *
                backgroundTexture.transform.localScale.x;
            backgroundTextureHeight =
                backgroundTexture.GetComponent<SpriteRenderer>().size.y *
                backgroundTexture.transform.localScale.y;
            
            RenderRepeatedBackground();
        }

        private void RenderRepeatedBackground()
        {
            bool shouldNotRenderAnything =
                desiredNumberOfRows == 0 || desiredNumberOfColumns == 0;
            if (shouldNotRenderAnything)
            {
                return;
            }
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
    }
}
