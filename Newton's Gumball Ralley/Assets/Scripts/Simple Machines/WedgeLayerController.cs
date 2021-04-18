﻿using Core;
using UnityEngine;

namespace Wedge
{
    public class WedgeLayerController : MonoBehaviour
    {
        [SerializeField] private GameObject targetWedge;
        private LayerMask defaultLayer;
        private LayerMask wedgeLayer;

        private void Awake()
        {
            defaultLayer = LayerMask.NameToLayer("Default");
            wedgeLayer = LayerMask.NameToLayer("Wedge");
        }

        private void Update()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
                targetWedge.layer = defaultLayer;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Destructible") && GameStateManager.GetGameState().Equals(GameState.Playing))
                targetWedge.layer = wedgeLayer;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            targetWedge.layer = defaultLayer;
        }
    }
}