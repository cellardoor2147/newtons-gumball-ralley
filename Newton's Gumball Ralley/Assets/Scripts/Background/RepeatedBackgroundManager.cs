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
            for (int currCol = 0; currCol < desiredNumberOfRows; currCol++)
            {
                float nextRenderPositionY = GetStartingRenderPositionY();
                for (int currRow = 0; currRow < desiredNumberOfColumns; currRow++)
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
            if (thereIsAnOddNumberOfDesiredColumns)
            {
                return -((desiredNumberOfColumns - 1) * backgroundTextureWidth) / 2;
            }
            return -((((desiredNumberOfColumns - 2) * backgroundTextureWidth) / 2) + backgroundTextureWidth / 2);
        }

        private float GetStartingRenderPositionY()
        {
            bool thereIsAnOddNumberOfDesiredRows = desiredNumberOfRows % 2 == 1;
            if (thereIsAnOddNumberOfDesiredRows)
            {
                return -((desiredNumberOfRows - 1) * backgroundTextureHeight) / 2;
            }
            return -((((desiredNumberOfRows - 2) * backgroundTextureWidth) / 2) + backgroundTextureWidth / 2);
        }
    }
}
