using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryingTimelineEvaluator : MonoBehaviour
{
    public float[] AnalogScores;
    public float[] ButtonScores;
    public float MaxAnalogError;
    public CryingTimeline Timeline;
    public CryingTimelinePlayer TimelinePlayer;
    public ScoreData Score;

    public CryingAnalogRenderer[] AnalogRenderers;

    private void Start()
    {
        // Setup evaluation data
        this.Score.AnalogScores = new float[this.Timeline.Analogs.Length];
        this.Score.TotalScore = 0;
        for (int iAnalog=0; iAnalog < this.Score.AnalogScores.Length; ++iAnalog)
        {
            this.Score.AnalogScores[iAnalog] = 0;
        }

        this.Score.ButtonScores = new float[this.Timeline.Buttons.Length][];
        for (int iButton = 0; iButton < this.Score.ButtonScores.Length; ++iButton)
        {
            CryingTimeline.CryingTrack_Button cryingTrack_Button = this.Timeline.Buttons[iButton];
            float[] scoreTable = new float[cryingTrack_Button.Beats.Length];
            for (int iBeat = 0; iBeat < scoreTable.Length; ++iBeat)
            {
                scoreTable[iBeat] = 0;
            }

            this.Score.ButtonScores[iButton] = scoreTable;
        }
    }

    public void Update()
    {
        CryingTimeline tl = this.Timeline;

        float musicBeatTime = this.TimelinePlayer.MusicTimeInBeats;
        float delta = UnityEngine.Time.deltaTime;
        float lengthInBeats = tl.LengthInBeats;

        for (int iAnalog = 0; iAnalog < tl.Analogs.Length; ++iAnalog)
        {
            CryingAnalog analog = this.TimelinePlayer.Analogs[iAnalog];
            float abscissa = musicBeatTime / tl.LengthInBeats;
            float tlValue = tl.Analogs[iAnalog].Evaluate(abscissa);
            float axisValue = analog.AxisValue;
            float scorePerBeat = this.AnalogScores[iAnalog] / lengthInBeats;
            float beatLengthInSeconds = this.TimelinePlayer.BeatLengthInSeconds;
            float scorePerSecond = scorePerBeat / beatLengthInSeconds;
            float diff = Mathf.Abs(tlValue - axisValue);

            bool isScoring = diff < this.MaxAnalogError;

            Debug.Log($"abs {abscissa} tl {tlValue} axis {axisValue} diff {diff} isScoring {isScoring}");

            // Update renderer feedback flag
            CryingAnalogRenderer renderer = this.AnalogRenderers[iAnalog];
            if (renderer != null)
            {
                renderer.IsScoring = isScoring;
            }

            if (isScoring)
            {
                float scoreToAdd = delta * scorePerSecond;
                this.Score.AnalogScores[iAnalog] += scoreToAdd;
                this.Score.TotalScore += scoreToAdd;
            }
        }

        for (int iButton = 0; iButton < tl.Buttons.Length; ++iButton)
        {
            CryingTimeline.CryingTrack_Button cryingTrack_Button = this.Timeline.Buttons[iButton];
            
            // TODO : check if a button has been pushed correctly. If so, add points
        }
    }

    public struct ScoreData
    {
        public float TotalScore;
        public float[] AnalogScores;
        public float[][] ButtonScores;
    }
}
