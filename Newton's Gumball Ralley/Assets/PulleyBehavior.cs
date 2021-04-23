using UnityEngine;
using System.Collections.Generic;
using Audio;
using Core;

namespace SimpleMachine {
    public class PulleyBehavior : MonoBehaviour
    {
        public enum PulleyState
        {
            SimplePulley = 0,
            CompoundPulley = 1
        }

        public enum PlatformState
        {
            FlatPlatform = 0,
            SpikeWedge = 1
        }
        private PulleyState pulleyState;
        private PlatformState platformState;

        private Transform pulleyFulcrums;
        private Transform pulleyPlatforms;

        public PulleyState defaultPulleyState;
        public PlatformState defaultPlatformState;

        private bool hasDropped;
        private bool shouldRise;
        private bool shouldFall;

        private float minSpeed;
        [SerializeField] SoundMetaData PulleySound;

        [SerializeField] PlacedObjectMetaData simplePulleyMetaData;
        [SerializeField] PlacedObjectMetaData compoundPulleyMetaData;


        private void Awake()
        {
            pulleyFulcrums = transform.GetChild(0);
            pulleyPlatforms = transform.GetChild(1);
            pulleyState = defaultPulleyState;
            platformState = defaultPlatformState;
            minSpeed = 20f;
        }

        private void Update()
        {
            if (pulleyState.Equals(PulleyState.SimplePulley))
            {
                GetComponent<PlacedObjectManager>().metaData = simplePulleyMetaData;
            }
            else
            {
                GetComponent<PlacedObjectManager>().metaData = compoundPulleyMetaData;
            }
            switch (pulleyState) 
            {
                case PulleyState.SimplePulley:
                    pulleyFulcrums.GetChild(0).gameObject.SetActive(true);
                    pulleyFulcrums.GetChild(1).gameObject.SetActive(false);
                    break;
                case PulleyState.CompoundPulley:
                    pulleyFulcrums.GetChild(0).gameObject.SetActive(false);
                    pulleyFulcrums.GetChild(1).gameObject.SetActive(true);
                    break;
            }
            switch (platformState)
            {
                case PlatformState.FlatPlatform:
                    pulleyPlatforms.GetChild(0).gameObject.SetActive(true);
                    pulleyPlatforms.GetChild(1).gameObject.SetActive(false);
                    break;
                case PlatformState.SpikeWedge:
                    pulleyPlatforms.GetChild(0).gameObject.SetActive(false);
                    pulleyPlatforms.GetChild(1).gameObject.SetActive(true);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (shouldRise)
            {
                if (pulleyPlatforms.GetChild(0).gameObject.activeSelf)
                {
                    //Move Up Platform
                    Debug.Log("up");
                }
                if (pulleyPlatforms.GetChild(1).gameObject.activeSelf)
                {
                    //Move Up Wedge
                }
            }
            else if (shouldFall)
            {
                if (pulleyPlatforms.GetChild(0).gameObject.activeSelf)
                {
                    //Move Up Platform
                    Debug.Log("down");
                }
                if (pulleyPlatforms.GetChild(1).gameObject.activeSelf)
                {
                    //Move Up Wedge
                }
            }
            else if (shouldRise && !shouldFall)
            {
                if (pulleyPlatforms.GetChild(0).gameObject.activeSelf)
                {
                    //Stop Platform
                }
                if (pulleyPlatforms.GetChild(1).gameObject.activeSelf)
                {
                    //Stop and Drop Wedge
                }
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.name.Equals("Gear2")) 
            {
                Rigidbody2D gearRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
                if (gearRigidbody.angularVelocity > minSpeed 
                    && (pulleyState.Equals(PulleyState.SimplePulley) && platformState.Equals(PlatformState.FlatPlatform))
                    || pulleyState.Equals(PulleyState.CompoundPulley)) 
                {
                    shouldRise = true;
                    shouldFall = false;

                }
                else if (gearRigidbody.angularVelocity > -1 * minSpeed 
                    && platformState.Equals(PlatformState.FlatPlatform))
                {
                    shouldRise = false;
                    shouldFall = true;
                }
                else 
                {
                    shouldRise = false;
                    shouldFall = false;
                }
            }
        }

        public void OnMouseDown()
        {
            if (pulleyState.Equals(PulleyState.SimplePulley) && GameStateManager.GetGameState().Equals(GameState.Editing)) 
            {
                pulleyState = PulleyState.CompoundPulley;
            }
            else if (pulleyState.Equals(PulleyState.CompoundPulley) && GameStateManager.GetGameState().Equals(GameState.Editing)) 
            {
                pulleyState = PulleyState.SimplePulley;
            }
        }

    }
}
