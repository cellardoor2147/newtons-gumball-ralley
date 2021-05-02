using Core;
using DestructibleObject;
using UnityEngine;
using Audio;

namespace Wedge
{
    public class WedgeLayerController : MonoBehaviour
    {
        [SerializeField] private GameObject targetWedge;
        private LayerMask defaultLayer;
        private LayerMask wedgeLayer;
        private Rigidbody2D rb;

        [SerializeField] SoundMetaData BreakSound; 
        [SerializeField] SoundMetaData BounceSound; 
        private bool bounced;

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
            bounced = false;
            DestructibleObstacleLayerController destructible
                = collision.GetComponentInParent<DestructibleObstacleLayerController>();
            if (destructible && GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                float localVel = 0f;

                if (gameObject.name == "AxeWedgeCollider")
                {
                    localVel = transform.InverseTransformDirection(rb.velocity).x;
                }
                else if (gameObject.name == "AxeWedgeInvertedCollider")
                {
                    localVel = -transform.InverseTransformDirection(rb.velocity).x;
                }
                else if (gameObject.name == "SpikeWedgeCollider")
                {
                    localVel = -transform.InverseTransformDirection(rb.velocity).y;
                }

                if (localVel > destructible.GetImpactThreshold())
                {
                    targetWedge.layer = wedgeLayer;
                    collision.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
                    if (!AudioManager.instance.isPlaying(BreakSound.name))
                    {
                        AudioManager.instance.PlaySound(BreakSound.name);
                    }
                }
                else if (GameStateManager.GetGameState().Equals(GameState.Playing))
                {
                    AudioManager.instance.PlaySound(BounceSound.name);
                    bounced = true;
                }
            }
            if (!bounced && GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                AudioManager.instance.PlaySound(BounceSound.name);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            targetWedge.layer = defaultLayer;
        }
    }
}