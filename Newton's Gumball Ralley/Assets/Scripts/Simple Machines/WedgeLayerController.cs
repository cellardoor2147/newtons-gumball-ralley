using Core;
using UnityEngine;
using Audio;

namespace Wedge
{
    public class WedgeLayerController : MonoBehaviour
    {
        [SerializeField] private GameObject targetWedge;
        private LayerMask defaultLayer;
        private LayerMask wedgeLayer;
        [SerializeField] SoundMetaData BreakSound;
        [SerializeField] SoundMetaData BounceSound;

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
            if ((collision.CompareTag("Destructible") || collision.CompareTag("Debris"))
                && GameStateManager.GetGameState().Equals(GameState.Playing))
                targetWedge.layer = wedgeLayer;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            targetWedge.layer = defaultLayer;
        }
    }
}