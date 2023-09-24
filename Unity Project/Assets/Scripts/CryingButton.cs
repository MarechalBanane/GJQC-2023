using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class CryingButton : MonoBehaviour
{
    public KeyCode Input;
    public EventReference FmodEvent;
    public UnityEvent<float> TapPreview;
    public UnityEvent Tapped;
    public UnityEvent Scored;
    public UnityEvent Missed;

    private FMOD.Studio.EventInstance ouinInstance;

    // Start is called before the first frame update
    void Start()
    {
        this.ouinInstance = FMODUnity.RuntimeManager.CreateInstance(FmodEvent);
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(this.Input))
        {
            this.ouinInstance.start();
            this.Tapped.Invoke();
        }
    }
}
