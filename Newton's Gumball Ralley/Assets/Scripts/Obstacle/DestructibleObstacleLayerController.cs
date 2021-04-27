using UnityEngine;
using System.Collections.Generic;
using Destructible2D;

namespace DestructibleObject
{
    public class DestructibleObstacleLayerController : MonoBehaviour
    {
        public static LayerMask defaultLayer;
        public static LayerMask debrisLayer;
        private D2dDestructible destructible;
        private D2dImpactFissure d2DImpactFissure;
        private Rigidbody2D rb;
        [SerializeField] private float impactThreshold = 5f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            destructible = GetComponent<D2dDestructible>();
            d2DImpactFissure = GetComponent<D2dImpactFissure>();
            d2DImpactFissure.Threshold = impactThreshold;
            destructible.OnSplitStart += Destructible_OnSplitStart;
            destructible.OnSplitEnd += Destructible_OnSplitEnd;
            defaultLayer = LayerMask.NameToLayer("Default");
            debrisLayer = LayerMask.NameToLayer("Debris");
        }

        private void Destructible_OnSplitStart()
        {
            rb.constraints = RigidbodyConstraints2D.None;
        }

        private void Destructible_OnSplitEnd(List<D2dDestructible> destructibles, D2dDestructible.SplitMode splitMode)
        {
            foreach (D2dDestructible destructible in destructibles)
            {
                UpdateAllLayers(debrisLayer);
            }
        }

        public void UpdateAllLayers(LayerMask desiredLayer)
        {
            gameObject.layer = desiredLayer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = desiredLayer;
            }
        }

        public float GetImpactThreshold()
        {
            return impactThreshold;
        }

        public void FreezeRigidbody()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
