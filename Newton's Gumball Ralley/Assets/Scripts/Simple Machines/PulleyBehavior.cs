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

        public enum PlatformPosition
        {
            Down = 0,
            Up = 1
        }

        public enum BallRollDirection
        {
            Left = 0,
            Right = 1
        }

        private PulleyState pulleyState;
        [HideInInspector] public PlatformState platformState;
        private PlatformPosition platformPosition;
        [SerializeField] public BallRollDirection ballRollDirection;

        private int currPulley;
        private int currPlatform;

        private Transform pulleyFulcrums;
        private Transform pulleyPlatforms;

        private Transform activePlatform;
        private GearBehavior gearBehavior;
        private PulleyWedgeBehavior pulleyWedgeBehavior;

        [SerializeField] PulleyState defaultPulleyState;
        [SerializeField] PlatformState defaultPlatformState;
        [SerializeField] PlatformPosition defaultPlatformPosition;

        [HideInInspector] public bool shouldRise;
        [HideInInspector] public bool shouldFall;
        private bool hasReset;

        private float minSpeed;
        private Vector3 orginalPosition;
        private float minHeight;
        private float maxHeight;

        [HideInInspector] public bool grounded;
        private float raiseDistance;
        private float platformSpeed;
        [SerializeField] SoundMetaData PulleySound;

        [SerializeField] PlacedObjectMetaData simplePulleyMetaData;
        [SerializeField] PlacedObjectMetaData compoundPulleyMetaData;
        [SerializeField] PlacedObjectMetaData gear2MetaData;


        private void Awake()
        {
            pulleyFulcrums = transform.GetChild(0);
            pulleyPlatforms = transform.GetChild(1);
            pulleyState = defaultPulleyState;
            platformState = defaultPlatformState;
            SwitchPlatformState(defaultPlatformState);
            raiseDistance = 3.5f;
            SwitchPlatformPosition(defaultPlatformPosition);
            minSpeed = 20f;  
            platformSpeed = 0.5f;
            grounded = true;
            pulleyWedgeBehavior = pulleyPlatforms.GetChild(1).gameObject.GetComponent<PulleyWedgeBehavior>();
            orginalPosition = pulleyPlatforms.GetChild(currPlatform).position;
            ResetPlatformHeight();
        }

        private void SwitchPlatformState(PlatformState platformState)
        {
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

        private void SwitchPlatformPosition(PlatformPosition platformPosition)
        {
            switch (platformPosition)
            {
                case PlatformPosition.Down:
                    minHeight = pulleyPlatforms.GetChild(currPlatform).position.y;
                    maxHeight = pulleyPlatforms.GetChild(currPlatform).position.y + raiseDistance;
                    break;
                case PlatformPosition.Up:
                    pulleyPlatforms.GetChild(currPlatform).transform.position =
                        pulleyPlatforms.GetChild(currPlatform).transform.position
                        + new Vector3(0, raiseDistance, 0);
                    minHeight = pulleyPlatforms.GetChild(currPlatform).position.y - raiseDistance;
                    maxHeight = pulleyPlatforms.GetChild(currPlatform).position.y;
                    break;
            }
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
        }

        private void FixedUpdate()
        {
            activePlatform = pulleyPlatforms.GetChild(currPlatform);

            if (GameStateManager.GetGameState().Equals(GameState.Editing))
            {
                AudioManager.instance.StopSound(PulleySound.name);
                grounded = true;
                if (!hasReset) {
                    ResetSpikeWedge();
                    ResetPlatformHeight();
                    hasReset = true;
                }
            }
            else if (GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                hasReset = false;
            }

            if (gearBehavior != null)
            {

                if (activePlatform.position.y <= minHeight)
                {
                    gearBehavior.shouldSpinRight = false;
                    if (platformState.Equals(PlatformState.FlatPlatform) 
                        && platformPosition.Equals(PlatformPosition.Up))
                    {
                        grounded = false;
                    }
                    else
                    {
                        grounded = true;
                    }
                }
                else if (activePlatform.position.y >= maxHeight)
                {
                    gearBehavior.shouldSpinLeft = false;
                    if (platformState.Equals(PlatformState.FlatPlatform)
                         && platformPosition.Equals(PlatformPosition.Down))
                    {
                        grounded = false;
                    }
                    else
                    {
                        grounded = true;
                    }
                }
                else
                {
                    grounded = true;
                    gearBehavior.shouldSpinRight = true;
                    gearBehavior.shouldSpinLeft = true;
                }
            }
            if (shouldRise)
            {
                activePlatform.transform.Translate(Vector3.up * platformSpeed * Time.deltaTime);
                if (!AudioManager.instance.isPlaying(PulleySound.name))
                {
                    AudioManager.instance.PlaySound(PulleySound.name);
                }
            }
            else if (shouldFall)
            {
                activePlatform.transform.Translate(Vector3.down * platformSpeed * Time.deltaTime);
                if (!AudioManager.instance.isPlaying(PulleySound.name))
                {
                    AudioManager.instance.PlaySound(PulleySound.name);
                }
            }
            else if (!shouldRise && !shouldFall)
            {
                AudioManager.instance.StopSound(PulleySound.name);
                if (currPlatform == 1 && activePlatform.position.y >= minHeight && pulleyWedgeBehavior.shouldDrop)
                {
                    DropWedge();
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("SimpleMachine"))
            {
                if (other.gameObject.GetComponent<PlacedObjectManager>().metaData.Equals(gear2MetaData)
                && GameStateManager.GetGameState().Equals(GameState.Playing))
                {
                    Rigidbody2D gearRigidbody = other.gameObject.GetComponent<Rigidbody2D>();
                    gearBehavior = other.gameObject.GetComponent<GearBehavior>();

                    bool isSimplePulley = pulleyState.Equals(PulleyState.SimplePulley);
                    bool isCompoundPulley = pulleyState.Equals(PulleyState.CompoundPulley);
                    bool isFlatPlatform = platformState.Equals(PlatformState.FlatPlatform);
                    bool isSpikeWedge = platformState.Equals(PlatformState.SpikeWedge);

                    if (isSimplePulley && isSpikeWedge)
                    {
                        gearBehavior.shouldSpinRight = false;
                        gearBehavior.shouldSpinLeft = false;
                    } 
                    else
                    {
                        gearBehavior.shouldSpinRight = true;
                        gearBehavior.shouldSpinLeft = true;
                    }

                    if (gearBehavior.spinState.Equals(GearBehavior.SpinState.SpinningLeft)
                        && activePlatform.position.y <= maxHeight
                        && ((isSimplePulley && isFlatPlatform) || isCompoundPulley))
                    {
                        shouldRise = true;
                        shouldFall = false;
                    }
                    else if (gearBehavior.spinState.Equals(GearBehavior.SpinState.SpinningRight)
                            && activePlatform.position.y >= minHeight
                            && ((isSimplePulley && isFlatPlatform) || isCompoundPulley))
                    {
                        shouldRise = false;
                        shouldFall = true;
                    }
                    else
                    {
                        gearBehavior.spinState = GearBehavior.SpinState.NotSpinning;
                        shouldRise = false;
                        shouldFall = false;
                    }
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
            pulleyPlatforms.GetChild(1).GetChild(1).gameObject.SetActive(false);
            pulleyPlatforms.GetChild(1).GetChild(2).gameObject.SetActive(true);
        }

        private void ResetSpikeWedge()
        {
            pulleyPlatforms.GetChild(1).GetChild(2).localPosition = pulleyPlatforms.GetChild(1).GetChild(1).localPosition;
            pulleyPlatforms.GetChild(1).GetChild(2).gameObject.SetActive(false);
            pulleyPlatforms.GetChild(1).GetChild(1).gameObject.SetActive(true);
        }

        private void ResetPlatformHeight()
        {
            pulleyPlatforms.GetChild(currPlatform).position = orginalPosition;
        }
    }
}
