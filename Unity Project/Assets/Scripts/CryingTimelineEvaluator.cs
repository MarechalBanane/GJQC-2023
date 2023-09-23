using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryingTimelineEvaluator : MonoBehaviour
{
    public CryingTimeline Timeline;
    public CryingTimelinePlayer TimelinePlayer;
    public ScoreData Score;

    private void Start()
    {
        // Setup evaluation data
        this.Score.analogScores = new float[this.Timeline.Analogs.Length];
        for (int iAnalog=0; iAnalog < this.Score.analogScores.Length; ++iAnalog)
        {
            this.Score.analogScores[iAnalog] = 0;
        }

        this.Score.buttonScores = new float[this.Timeline.Buttons.Length][];
        for (int iButton = 0; iButton < this.Score.buttonScores.Length; ++iButton)
        {
            CryingTimeline.CryingTrack_Button cryingTrack_Button = this.Timeline.Buttons[iButton];
            float[] scoreTable = new float[cryingTrack_Button.Beats.Length];
            for (int iBeat = 0; iBeat < scoreTable.Length; ++iBeat)
            {
                scoreTable[iBeat] = 0;
            }

            this.Score.buttonScores[iButton] = scoreTable;
        }
    }

    public void Update()
    {
        // TODO : - get where we are
        // - send feedback events if necessary
        // - check input against each track

    }

    public struct ScoreData
    {
        public float[] analogScores;
        public float[][] buttonScores;
    }
}
