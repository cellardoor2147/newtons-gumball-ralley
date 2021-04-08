using UnityEngine;
using Core;

namespace Gear {
    public class GearBehavior : MonoBehaviour
    {
        private Rigidbody2D rigidbody2D;

        private void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            
        }
    }
}

