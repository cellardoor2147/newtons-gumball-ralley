using Core;
using UnityEngine;
using System.Collections.Generic;
using Destructible2D;

namespace DestructibleObject
{
    public class DestructibleObstacleLayerController : MonoBehaviour
    {
        public static LayerMask defaultLayer;
        private LayerMask debrisLayer;
        private D2dDestructible destructible;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            destructible = GetComponent<D2dDestructible>();
            destructible.OnSplitStart += Destructible_OnSplitStart;
            defaultLayer = LayerMask.NameToLayer("Default");
            debrisLayer = LayerMask.NameToLayer("Debris");
        }

        private void Destructible_OnSplitStart()
        {
            rb.constraints = RigidbodyConstraints2D.None;
            UpdateAllLayers(debrisLayer);
        }

        public void UpdateAllLayers(LayerMask desiredLayer)
        {
            gameObject.layer = desiredLayer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = desiredLayer;
            }
        }
    }
}
