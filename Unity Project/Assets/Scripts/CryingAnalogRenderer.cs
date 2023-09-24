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

    private UnityEngine.AnimationCurve analogCurve;
    private Unity.Collections.NativeArray<Vector3> positions;
    private int pointCount;
    private float pointWidth;

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
    }

    private void OnDestroy()
    {
        this.positions.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        float musicTime = this.Player.MusicTime;
        float musicTimeInBeats = this.Player.MusicTimeInBeats - this.OffsetInBeats;
        float timePerPoint = 1.0f / PointsPerBeat;
        Vector3 position = this.transform.position;

        float axisValue = Input.GetAxis(this.AxisName);
        float x = position.x + this.OffsetInBeats * this.PointsPerBeat * this.pointWidth;
        float y = position.y + axisValue;
        float z = this.PlayerFeedbackObject.transform.position.z;
        this.PlayerFeedbackObject.transform.position = new Vector3(x, y, z);

        for (int i = 0; i < this.pointCount; ++i)
        {
            float value = this.analogCurve.Evaluate(musicTimeInBeats / this.Timeline.LengthInBeats);

            musicTimeInBeats += timePerPoint;
            this.positions[i] = new Vector3(position.x + i * this.pointWidth, position.y + (value * this.Height / 2), position.z);
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

        this.LineRenderer.SetPositions(this.positions);
    }
}
