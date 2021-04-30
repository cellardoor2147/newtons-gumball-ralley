using Core;
using UnityEngine;

namespace SimpleMachine
{
    public class LeverWeightBehavior : MonoBehaviour
    {
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnMouseDown()
        {
            if (GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.AddForce(new Vector2(0, -100f));
            }
        }

        public void FreezeRigidbody()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}