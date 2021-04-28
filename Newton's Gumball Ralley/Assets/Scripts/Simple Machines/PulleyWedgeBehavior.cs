using System.Collections;
using UnityEngine;
using Core;

namespace SimpleMachine
{    
    public class PulleyWedgeBehavior : MonoBehaviour
    {
        [HideInInspector] public bool shouldDrop;
        private PulleyBehavior pulleyBehavior;

        private Collider2D boxCollider;

        private void Awake()
        {
            pulleyBehavior = transform.parent.parent.gameObject.GetComponent<PulleyBehavior>();
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = false;
        }
        private void OnMouseDown()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Playing) 
                && pulleyBehavior.platformState.Equals(PulleyBehavior.PlatformState.SpikeWedge)
                && !pulleyBehavior.shouldFall && !pulleyBehavior.shouldRise && !shouldDrop)
            {
                boxCollider.isTrigger = true;
                shouldDrop = true;
            }
            else
            {
                shouldDrop = false;
            }
        }

        private void Update()
        {
            if (!GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                shouldDrop = false;
            }
            if (!shouldDrop && boxCollider.isTrigger && !GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                boxCollider.isTrigger = false;
            }
        }
    }
}

