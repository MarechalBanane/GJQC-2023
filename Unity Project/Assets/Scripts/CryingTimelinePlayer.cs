//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// timeline markers in your game code. 
//
// Timeline markers can be implicit - such as beats and bars. Or they 
// can be explicity placed by sound designers, in which case they have 
// a sound designer specified name attached to them.
//
// Timeline markers can be useful for syncing game events to sound
// events.
//
// The script starts a piece of music and then displays on the screen
// the current bar and the last marker encountered.
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
// For information on using FMOD example code in your own programs, visit
// https://www.fmod.com/legal
//
//--------------------------------------------------------------------

using FMODUnity;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Events;

public class CryingTimelinePlayer : MonoBehaviour
{
    public CryingButton[] Buttons;
    public CryingAnalog[] Analogs;
    public MainCry Cry;

    public CryingTimeline Timeline;

    public EventReference MusicEvent;

    public UnityEvent OnEnd;

    [UnityEngine.HideInInspector]
    public float MusicTime;

    [UnityEngine.HideInInspector]
    public int MusicBeat;

    public int BeatsBeforeStarting = 16;
    public int BeatOffsetForFeedback = 4;

    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    private FMOD.Studio.EventInstance musicInstance;

    private TimelineInfo timelineInfo;
    private GCHandle timelineHandle;
    private bool firstBeatReceived;
    private bool isStarted;

    public float Tempo => this.timelineInfo.Tempo;

    [UnityEngine.HideInInspector]
    public float MusicTimeInBeats
    {
        get
        {
            return this.MusicTime * this.Tempo / 60;
        }
    }

    [UnityEngine.HideInInspector]
    public float BeatLengthInSeconds
    {
        get
        {
            return 60 / this.Tempo;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return this.MusicTime > 0 && !this.IsEnded;
        }
    }

    public bool IsEnded
    {
        get
        {
            return this.MusicTimeInBeats >= this.Timeline.LengthInBeats;
        }
    }

    public bool IsFullyStarted
    {
        get
        {
            return this.isStarted && this.firstBeatReceived;
        }
    }

    public void Play()
    {
        this.SetControlsActive(true);
        this.Cry.Play();
        this.isStarted = true;
        this.timelineInfo.NewBeat = false;
        this.firstBeatReceived = false;
    }

    private void SetControlsActive(bool active)
    {
        for (int iButton = 0; iButton < this.Buttons.Length; iButton++)
        {
            this.Buttons[iButton].enabled = active;
        }

        for (int iAnalog = 0; iAnalog < this.Analogs.Length; iAnalog++)
        {
            this.Analogs[iAnalog].enabled = active;
        }
    }

    private void Start()
    {
        this.isStarted = false;
        this.firstBeatReceived = false;
        this.timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        this.beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        this.musicInstance = FMODUnity.RuntimeManager.CreateInstance(MusicEvent);

        // Pin the class that will store the data modified during the callback
        this.timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance
        this.musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        this.musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        this.musicInstance.start();
        this.MusicTime = 0;
        this.MusicBeat = -this.BeatsBeforeStarting-1;

        this.SetControlsActive(false);
    }

    private void Update()
    {
        if (!this.isStarted)
        {
            return;
        }

        if (this.firstBeatReceived)
        {
            this.MusicTime += Time.unscaledDeltaTime;
        }

        if (this.timelineInfo.NewBeat)
        {
            this.firstBeatReceived = true;
            ++this.MusicBeat;
            this.MusicTime = this.MusicBeat / this.Tempo * 60;
            this.timelineInfo.NewBeat = false;

            CryingTimeline tl = this.Timeline;
            for (int iButton = 0; iButton < tl.Buttons.Length; iButton++)
            {
                CryingTimeline.CryingTrack_Button buttontrack = tl.Buttons[iButton];
                int[] beats = buttontrack.Beats;
                for (int iEvent = 0; iEvent < buttontrack.Beats.Length; ++iEvent)
                {
                    int ev = buttontrack.Beats[iEvent];
                    if (ev == this.MusicBeat + this.BeatOffsetForFeedback)
                    {
                        if (this.Buttons != null && this.Buttons.Length > iButton)
                        {
                            CryingButton btn = this.Buttons[iButton];
                            btn.TapPreview.Invoke(this.BeatOffsetForFeedback * 60 / this.timelineInfo.Tempo);
                        }
                    }
                }
            }

            // See if we need to launch beat events
            CryingTimeline.BeatEvent[] beatEvents = tl.BeatEvents;
            for (int iEvent = 0;iEvent < beatEvents.Length; ++iEvent)
            {
                if (beatEvents[iEvent].Beat == this.MusicBeat)
                {
                    beatEvents[iEvent].OnBeatEvent.Invoke();
                }
            }

            if (this.MusicBeat == this.Timeline.LengthInBeats)
            {
                Debug.Log("Ended !");
                this.SetControlsActive(false);
                this.Cry.Stop();
                this.OnEnd.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        this.musicInstance.setUserData(IntPtr.Zero);
        this.musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        this.musicInstance.release();
        this.timelineHandle.Free();
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle tlHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)tlHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.Bar = parameter.bar;
                        timelineInfo.Beat = parameter.beat;
                        timelineInfo.Tempo = parameter.tempo;
                        timelineInfo.Position = parameter.position;
                        timelineInfo.BeatsPerBar = parameter.timesignatureupper;
                        timelineInfo.NewBeat = true;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }

    public class TimelineInfo
    {
        public int Bar = 0;
        public int Beat = 0;
        public int Position = 0;
        public float Tempo = 0;
        public int BeatsPerBar = 0;
        public bool NewBeat = false;

        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
    }
}