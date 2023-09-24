using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class MainCry : MonoBehaviour
{
    public EventReference FmodEvent;

    private FMOD.Studio.EventInstance ouinInstance;

    public FMOD.Studio.EventInstance Instance => this.ouinInstance;

    // Start is called before the first frame update
    public void Play()
    {
        this.ouinInstance = FMODUnity.RuntimeManager.CreateInstance(FmodEvent);
        this.ouinInstance.start();
    }

    public void Stop()
    {
        this.ouinInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        this.ouinInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        this.ouinInstance.release();
    }
}
