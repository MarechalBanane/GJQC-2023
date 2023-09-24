using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class CryingAnalogRenderer : MonoBehaviour
{
    public int AnalogId;
    public string AxisName;
    public UnityEngine.GameObject PlayerFeedbackObject;
    public int PointsPerBeat;
    public int LengthInBeats;
    public float Width;
    public float Height;
    public float OffsetInBeats;
    public UnityEngine.LineRenderer LineRenderer;
    public CryingTimeline Timeline;
    public CryingTimelinePlayer Player;
    public Color WinColor;
    public Color LoseColor;
    public UnityEvent OnScoring;
    public UnityEvent OnStopScoring;

    private UnityEngine.AnimationCurve analogCurve;
    private Unity.Collections.NativeArray<Vector3> positions;
    private int pointCount;
    private float pointWidth;

    private bool wasScoring;

    [UnityEngine.HideInInspector]
    public bool IsScoring
    {
        get;
        set;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.pointCount = this.PointsPerBeat * this.LengthInBeats;
        this.pointWidth = this.Width / this.pointCount;
        this.analogCurve = this.Timeline.Analogs[this.AnalogId];
        this.positions = new Unity.Collections.NativeArray<Vector3>(this.pointCount, Unity.Collections.Allocator.Persistent);
        this.LineRenderer.positionCount = this.pointCount;
        this.IsScoring = true;
        this.wasScoring = this.IsScoring;
    }

    private void OnDestroy()
    {
        this.positions.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        float musicTimeInBeats = this.Player.MusicTimeInBeats - this.OffsetInBeats;
        float timePerPoint = 1.0f / this.PointsPerBeat;
        Vector3 position = this.transform.position;

        // show player feedback
        float x = position.x + this.OffsetInBeats * this.PointsPerBeat * this.pointWidth;
        float z = this.PlayerFeedbackObject.transform.position.z;
        if (this.Player.IsFullyStarted && !this.Player.IsEnded)
        {
            float axisValue = Input.GetAxis(this.AxisName);
            float y = position.y + axisValue;
            this.PlayerFeedbackObject.transform.position = new Vector3(x, y, z);
        }
        else
        {
            float y = position.y;
            this.PlayerFeedbackObject.transform.position = new Vector3(x, y, z);
        }

        if (this.Player.IsFullyStarted)
        {
            for (int i = 0; i < this.pointCount; ++i)
            {
                float abscissa = musicTimeInBeats / this.Timeline.LengthInBeats;
                musicTimeInBeats += timePerPoint;
                float value = this.analogCurve.Evaluate(abscissa);
                this.positions[i] = new Vector3(position.x + i * this.pointWidth, position.y + (value * this.Height / 2), position.z);
            }
        }
        else
        {
            for (int i = 0; i < this.pointCount; ++i)
            {
                this.positions[i] = new Vector3(position.x + i * this.pointWidth, position.y, position.z);
            }
        }

        if (this.IsScoring)
        {
            this.LineRenderer.startColor = this.WinColor;
            this.LineRenderer.endColor = this.WinColor;
        }
        else
        {
            this.LineRenderer.startColor = this.LoseColor;
            this.LineRenderer.endColor = this.LoseColor;
        }

        if (this.wasScoring != this.IsScoring)
        {
            if (this.IsScoring)
            {
                this.OnScoring.Invoke();
            }
            else
            {
                this.OnStopScoring.Invoke();
            }

            this.wasScoring = this.IsScoring;
        }

        this.LineRenderer.SetPositions(this.positions);
    }
}
