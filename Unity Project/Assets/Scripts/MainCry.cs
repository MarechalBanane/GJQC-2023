using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class MainCry : MonoBehaviour
{
    public KeyCode Secret = KeyCode.JoystickButton4;
    public EventReference FmodEvent;

    private FMOD.Studio.EventInstance ouinInstance;
    private bool secretOn = false;

    public FMOD.Studio.EventInstance Instance => this.ouinInstance;

    // Start is called before the first frame update
    public void Play()
    {
        this.ouinInstance = FMODUnity.RuntimeManager.CreateInstance(FmodEvent);
        this.ouinInstance.start();
    }

    public void Update()
    {
        if (Input.GetKeyDown(this.Secret))
        {
            this.secretOn = !this.secretOn;
            if (this.secretOn)
            {
                this.ouinInstance.setParameterByName("Tune", 1);
            }
            else
            {
                this.ouinInstance.setParameterByName("Tune", 0);
            }
        }
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
