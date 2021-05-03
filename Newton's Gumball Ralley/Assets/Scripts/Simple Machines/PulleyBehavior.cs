using UnityEngine;
using Audio;
using Core.PlacedObjects;
using Core;
using GUI.EditMode;

namespace SimpleMachine {

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

    public class PulleyBehavior : MonoBehaviour
    {

        [SerializeField] public PulleyState pulleyState;
        [SerializeField] public PlatformState platformState;
        [SerializeField] public PlatformPosition platformPosition;
        [SerializeField] public BallRollDirection ballRollDirection;

        private int currPlatform;

        private Transform pulleyFulcrums;
        private Transform pulleyPlatforms;

        private Transform activePlatform;
        private GearBehavior gearBehavior;
        private PulleyWedgeBehavior pulleyWedgeBehavior;

        [HideInInspector] public bool shouldRise;
        [HideInInspector] public bool shouldFall;
        private bool hasReset;
        private bool canSpin;
        private bool stopped;

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
            raiseDistance = 3.5f;
            minSpeed = 20f;  
            platformSpeed = 0.5f;
            grounded = true;
            pulleyWedgeBehavior = pulleyPlatforms.GetChild(1).gameObject.GetComponent<PulleyWedgeBehavior>();
            SwitchPlatformState(platformState);
            SwitchPlatformPosition(platformPosition);
            SetOrginalPosition();
            ResetPlatformHeight();
        }

        public void SetOrginalPosition()
        {
            orginalPosition = pulleyPlatforms.GetChild(currPlatform).localPosition;
        }
        
        public void SwitchPlatformPosition(PlatformPosition platformPosition)
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

        public void SwitchPlatformState(PlatformState platformState)
        {
            pulleyFulcrums = transform.GetChild(0);
            pulleyPlatforms = transform.GetChild(1);
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

        private void Update()
        {
            pulleyPlatforms.GetChild(currPlatform);
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
                if (activePlatform.position.y <= minHeight && canSpin)
                {
                    gearBehavior.shouldSpinLeft = true;
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
                else if (activePlatform.position.y >= maxHeight && canSpin)
                {
                    gearBehavior.shouldSpinRight = true;
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
                else if (canSpin)
                {
                    grounded = true;
                    gearBehavior.shouldSpinRight = true;
                    gearBehavior.shouldSpinLeft = true;
                }
                else
                {
                    gearBehavior.shouldSpinRight = false;
                    gearBehavior.shouldSpinLeft = false;
                }
            }
            if (shouldRise && GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                activePlatform.transform.Translate(Vector3.up * platformSpeed * Time.deltaTime);
                if (!AudioManager.instance.isPlaying(PulleySound.name))
                {
                    AudioManager.instance.PlaySound(PulleySound.name);
                    stopped = false;
                }
            }
            else if (shouldFall && GameStateManager.GetGameState().Equals(GameState.Playing))
            {
                activePlatform.transform.Translate(Vector3.down * platformSpeed * Time.deltaTime);
                if (!AudioManager.instance.isPlaying(PulleySound.name))
                {
                    AudioManager.instance.PlaySound(PulleySound.name);
                    stopped = false;
                }
            }
            else if (!shouldRise && !shouldFall)
            {
                if (!stopped)
                {
                    AudioManager.instance.StopSound(PulleySound.name);
                    stopped = true;
                }
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
                        canSpin = false;
                    } 
                    else
                    {
                        canSpin = true;
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
            if (pulleyState.Equals(PulleyState.SimplePulley) && GameStateManager.GetGameState().Equals(GameState.Editing) 
                && ScrapManager.ScrapRemaining > 30) 
            {
                pulleyState = PulleyState.CompoundPulley;
                ScrapManager.ChangeScrapRemaining(-30);
            }
            else if (pulleyState.Equals(PulleyState.CompoundPulley) && GameStateManager.GetGameState().Equals(GameState.Editing)) 
            {
                pulleyState = PulleyState.SimplePulley;
                ScrapManager.ChangeScrapRemaining(30);
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
            pulleyPlatforms.GetChild(1).GetChild(2).rotation = Quaternion.identity;
            pulleyPlatforms.GetChild(1).GetChild(2).gameObject.SetActive(false);
            pulleyPlatforms.GetChild(1).GetChild(1).gameObject.SetActive(true);
        }

        public void ResetPlatformHeight()
        {
            pulleyPlatforms.GetChild(currPlatform).localPosition = orginalPosition;
        }
    }
}
