using UnityEngine;
using Core.Levels;

namespace Core.PlacedObjects
{
    public class EnvironmentBlockManager : MonoBehaviour
    {
        [SerializeField] private Color spriteColorForWorld1;
        [SerializeField] private Color spriteColorForWorld2;
        [SerializeField] private Color spriteColorForWorld3;
        [SerializeField] private Color spriteColorForWorld4;
        [SerializeField] private Color spriteColorForWorld5;
        [SerializeField] private Color spriteColorForWorld6;

        private void Awake()
        {
            SetSpriteColorBasedOnWorld();
        }

        private void SetSpriteColorBasedOnWorld()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            switch (LevelManager.GetCurrentWorldIndex())
            {
                case 1:
                    spriteRenderer.color = spriteColorForWorld1;
                    break;
                case 2:
                    spriteRenderer.color = spriteColorForWorld2;
                    break;
                case 3:
                    spriteRenderer.color = spriteColorForWorld3;
                    break;
                case 4:
                    spriteRenderer.color = spriteColorForWorld4;
                    break;
                case 5:
                    spriteRenderer.color = spriteColorForWorld5;
                    break;
                case 6:
                    spriteRenderer.color = spriteColorForWorld6;
                    break;
                default:
                    spriteRenderer.color = Color.white;
                    break;
            }
        }
    }
}
