using Core;
using UnityEngine;

namespace DestructibleObject
{
    public class DestructibleObstacleLayerController : MonoBehaviour
    {
        private LayerMask defaultLayer;
        private LayerMask debrisLayer;

        private void Awake()
        {
            defaultLayer = LayerMask.NameToLayer("Default");
            debrisLayer = LayerMask.NameToLayer("Debris");
            UpdateAllLayers(defaultLayer);
        }

        private void Update()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                UpdateAllLayers(defaultLayer);
            }
            if (gameObject.name.Contains("(Clone)"))
            {
                UpdateAllLayers(debrisLayer);
            }
        }

        private void UpdateAllLayers(LayerMask desiredLayer)
        {
            gameObject.layer = desiredLayer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = desiredLayer;
            }
        }
    }
}
