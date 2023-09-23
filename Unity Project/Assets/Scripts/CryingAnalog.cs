using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class CryingAnalog : MonoBehaviour
{
    public string AxisName;
    public EventReference FmodEvent;

    private FMOD.Studio.EventInstance ouinInstance;

    // Start is called before the first frame update
    void Start()
    {
        this.ouinInstance = FMODUnity.RuntimeManager.CreateInstance(FmodEvent);
        this.ouinInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
