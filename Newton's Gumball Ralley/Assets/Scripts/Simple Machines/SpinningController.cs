using UnityEngine;

namespace SimpleMachine {
    public class SpinningController : MonoBehaviour
    {
        [SerializeField] private int spinDirection;

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
            if (spinDirection == -1 && gearBehavior.shouldSpinRight) 
            {
               gearBehavior.spinState = GearBehavior.SpinState.SpinningRight;
            }
            if (spinDirection == 1 && gearBehavior.shouldSpinLeft)
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
