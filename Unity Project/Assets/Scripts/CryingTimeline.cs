using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CryingTimeline : MonoBehaviour
{
    [UnityEngine.Tooltip("The duration of the level in beat units")]
    public int LengthInBeats = 1;
    public UnityEngine.AnimationCurve[] Analogs;
    public CryingTrack_Button[] Buttons;
    public BeatEvent[] BeatEvents;

    [System.Serializable]
    public struct CryingTrack_Button
    {
        public int[] Beats;
    }

    [System.Serializable]
    public struct BeatEvent
    {
        public int Beat;
        public UnityEvent OnBeatEvent;
    }
}
