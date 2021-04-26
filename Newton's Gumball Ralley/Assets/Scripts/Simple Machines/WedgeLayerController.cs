﻿using Core;
using DestructibleObject;
using UnityEngine;

namespace Wedge
{
    public class WedgeLayerController : MonoBehaviour
    {
        [SerializeField] private GameObject targetWedge;
        private LayerMask defaultLayer;
        private LayerMask wedgeLayer;
        private Rigidbody2D rb;

        private void Awake()
        {
            defaultLayer = LayerMask.NameToLayer("Default");
            wedgeLayer = LayerMask.NameToLayer("Wedge");
            rb = targetWedge.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
                targetWedge.layer = defaultLayer;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            DestructibleObstacleLayerController destructible
                = collision.GetComponentInParent<DestructibleObstacleLayerController>();
            if (destructible && GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                float localVel = 0f;

                if (gameObject.name == "AxeWedgeCollider")
                {
                    localVel = transform.InverseTransformDirection(rb.velocity).x;
                }
                else if (gameObject.name == "SpikeWedgeCollider")
                {
                    localVel = -transform.InverseTransformDirection(rb.velocity).y;
                }

                if (localVel > destructible.GetImpactThreshold())
                {
                    targetWedge.layer = wedgeLayer;
                    collision.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            targetWedge.layer = defaultLayer;
        }
    }
}