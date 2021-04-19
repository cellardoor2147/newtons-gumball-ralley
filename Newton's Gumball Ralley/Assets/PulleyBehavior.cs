using UnityEngine;
using System.Collections.Generic;
using Audio;
using Core;

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
    [SerializeField] SoundMetaData PulleySound;

    private void Awake()
    {
        pulleyFulcrums = transform.GetChild(0);
        pulleyPlatforms = transform.GetChild(1);
    }
    private void Update()
    {
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

    public void OnMouseDown()
    {
        if (pulleyState.Equals(PulleyState.SimplePulley) && GameStateManager.GetGameState().Equals(GameState.Playing)) 
        {
            pulleyState = PulleyState.CompoundPulley;
        }
        else if (pulleyState.Equals(PulleyState.CompoundPulley) && GameStateManager.GetGameState().Equals(GameState.Playing)) 
        {
            pulleyState = PulleyState.SimplePulley;
        }
    }

}
