using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryingTimeline : MonoBehaviour
{
    public CryingTrack_Analog[] Analogs;
    public CryingTrack_Button[] Buttons;

    [System.Serializable]
    public struct CryingTrack_Analog
    {
        public float[] Values;
    }

    [System.Serializable]
    public struct CryingTrack_Button
    {
        public int[] Beats;
    }
}
