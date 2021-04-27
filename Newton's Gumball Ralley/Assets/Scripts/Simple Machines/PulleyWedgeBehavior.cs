using System.Collections;
using UnityEngine;
using Core;

namespace SimpleMachine
{    
    public class PulleyWedgeBehavior : MonoBehaviour
    {
        [HideInInspector] public bool shouldDrop;
        private PulleyBehavior pulleyBehavior;

        private void Awake()
        {
            pulleyBehavior = transform.parent.parent.gameObject.GetComponent<PulleyBehavior>();
        }
        private void OnMouseDown()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Playing) 
                && pulleyBehavior.platformState.Equals(PulleyBehavior.PlatformState.SpikeWedge)
                && !pulleyBehavior.shouldFall && !pulleyBehavior.shouldFall && !shouldDrop)
            {
                shouldDrop = true;
            }
            else
            {
                shouldDrop = false;
            }
        }
    }
}

