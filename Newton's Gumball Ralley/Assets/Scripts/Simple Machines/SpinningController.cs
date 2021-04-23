using UnityEngine;

namespace SimpleMachine {
    public class SpinningController : MonoBehaviour
    {
        [SerializeField] private float spinDirection;

        private GameObject objectToSpin;
        private GearBehavior gearBehavior;

        private void Awake()
        {
            SetObjectToSpin(gameObject);    
        }

        public void SetObjectToSpin(GameObject objectToSpin)
        {
            this.objectToSpin = objectToSpin;
            gearBehavior = objectToSpin.GetComponent<GearBehavior>();
        }

        private void OnMouseDown()
        {
            if (spinDirection == -1f) 
            {
               gearBehavior.spinState = GearBehavior.SpinState.SpinningRight;
            }
            if (spinDirection == 1f)
            {
                gearBehavior.spinState = GearBehavior.SpinState.SpinningLeft;
            }
            if (spinDirection == 0)
            {
                gearBehavior.spinState = GearBehavior.SpinState.NotSpinning;
            }
        }
    }
}
