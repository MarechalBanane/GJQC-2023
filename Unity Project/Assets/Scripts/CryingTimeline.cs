using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryingTimeline : MonoBehaviour
{
    [UnityEngine.Tooltip("The duration of the level in beat units")]
    public int LengthInBeats = 1;
    public UnityEngine.AnimationCurve[] Analogs;
    public CryingTrack_Button[] Buttons;

    [System.Serializable]
    public struct CryingTrack_Button
    {
        public int[] Beats;
    }
}
