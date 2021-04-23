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

        private int currPulley;
        private int currPlatform;

        private Transform pulleyFulcrums;
        private Transform pulleyPlatforms;

        public PulleyState defaultPulleyState;
        public PlatformState defaultPlatformState;

        private bool hasDropped;
        private bool shouldRise;
        private bool shouldFall;

        private float minSpeed;
        private float[] minHeight = new float[2];
        private float[] maxHeight = new float[2];
        private float raiseDistance;
        private float platformSpeed;
        [SerializeField] SoundMetaData PulleySound;

        [SerializeField] PlacedObjectMetaData simplePulleyMetaData;
        [SerializeField] PlacedObjectMetaData compoundPulleyMetaData;


        private void Awake()
        {
            pulleyFulcrums = transform.GetChild(0);
            pulleyPlatforms = transform.GetChild(1);
            pulleyState = defaultPulleyState;
            platformState = defaultPlatformState;
            raiseDistance = 2.5f;
            minHeight[0] = pulleyPlatforms.GetChild(0).position.y;
            maxHeight[0] = pulleyPlatforms.GetChild(0).position.y + raiseDistance;
            minHeight[1] = pulleyPlatforms.GetChild(1).position.y;
            maxHeight[1] = pulleyPlatforms.GetChild(1).position.y + raiseDistance;
            minSpeed = 20f;  
            platformSpeed = 0.5f;
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
                    currPulley = 0;
                    break;
                case PulleyState.CompoundPulley:
                    pulleyFulcrums.GetChild(0).gameObject.SetActive(false);
                    pulleyFulcrums.GetChild(1).gameObject.SetActive(true);
                    currPulley = 1;
                    break;
            }
            switch (platformState)
            {
                case PlatformState.FlatPlatform:
                    pulleyPlatforms.GetChild(0).gameObject.SetActive(true);
                    pulleyPlatforms.GetChild(1).gameObject.SetActive(false);
                    currPlatform = 0;
                    break;
                case PlatformState.SpikeWedge:
                    pulleyPlatforms.GetChild(0).gameObject.SetActive(false);
                    pulleyPlatforms.GetChild(1).gameObject.SetActive(true);
                    currPlatform = 1;
                    break;
            }
        }

        private void FixedUpdate()
        {
            Transform activePlatform = pulleyPlatforms.GetChild(currPlatform);
            if (shouldRise)
            {
                if (activePlatform.position.y <= maxHeight[currPlatform])
                {
                    activePlatform.transform.Translate(Vector3.up * platformSpeed * Time.deltaTime);
                }
            }
            else if (shouldFall)
            {
                if (currPlatform == 0 && activePlatform.position.y >= minHeight[currPlatform])
                {
                    activePlatform.transform.Translate(Vector3.down * platformSpeed * Time.deltaTime);
                }
                if (currPlatform == 1)
                {
                   DropWedge();
                   shouldFall = false;
                }
            }
            else if (!shouldRise && !shouldFall)
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

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.name.Equals("Gear2(Clone)")) 
            {
                Rigidbody2D gearRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
                if (gearRigidbody.angularVelocity < -1 * minSpeed 
                    && (pulleyState.Equals(PulleyState.SimplePulley) && platformState.Equals(PlatformState.FlatPlatform))
                    || pulleyState.Equals(PulleyState.CompoundPulley)) 
                {
                    shouldRise = true;
                    shouldFall = false;

                }
                else if (gearRigidbody.angularVelocity > minSpeed 
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

        private void DropWedge()
        {
            Debug.Log("DropWedge");
        }
    }
}
