using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CryingTimelineEvaluator : MonoBehaviour
{
    public UnityEvent<string> OnScored;

    public string ProgressionParameter;
    public int ProgressionStepCount;
    public float ProgPercentPerStep;

    public float[] AnalogScores;
    public float[] ButtonScores;
    public float MaxAnalogError;
    public float MaxButtonError;
    public CryingTimeline Timeline;
    public CryingTimelinePlayer TimelinePlayer;
    public ScoreData Score;

    public CryingAnalogRenderer[] AnalogRenderers;

    private int[] buttonCursors;
    private float[] buttonMaxScores;
    private float maxScore;
    private int currentProgressionStep;

    private void Start()
    {
        // Setup evaluation data
        this.buttonCursors = new int[this.ButtonScores.Length];
        this.Score.AnalogScores = new float[this.Timeline.Analogs.Length];
        this.Score.TotalScore = 0;
        this.maxScore = 0;
        this.currentProgressionStep = 0;
        for (int iAnalog=0; iAnalog < this.Score.AnalogScores.Length; ++iAnalog)
        {
            this.Score.AnalogScores[iAnalog] = 0;
            this.maxScore += this.AnalogScores[iAnalog];
        }

        int buttonCount = this.Timeline.Buttons.Length;
        this.Score.ButtonScores = new float[buttonCount];
        this.buttonMaxScores = new float[buttonCount];
        for (int iButton = 0; iButton < this.Score.ButtonScores.Length; ++iButton)
        {
            this.Score.ButtonScores[iButton] = 0;

            CryingTimeline.CryingTrack_Button cryingTrack_Button = this.Timeline.Buttons[iButton];
            float buttonScore = cryingTrack_Button.Beats.Length * this.ButtonScores[iButton];
            this.buttonMaxScores[iButton] = buttonScore;
            this.maxScore += buttonScore;
        }
    }

    public void Update()
    {
        CryingTimeline tl = this.Timeline;

        float musicBeatTime = this.TimelinePlayer.MusicTimeInBeats;
        if (this.TimelinePlayer.IsPlaying)
        {
            bool updateScore = false;
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
                    updateScore = true;
                }
            }

            for (int iButton = 0; iButton < tl.Buttons.Length; ++iButton)
            {
                CryingTimeline.CryingTrack_Button cryingTrack_Button = this.Timeline.Buttons[iButton];
                CryingButton button = this.TimelinePlayer.Buttons[iButton];

                int cursorPosition = this.buttonCursors[iButton];
                if (cursorPosition < cryingTrack_Button.Beats.Length)
                {
                    if (Input.GetKeyDown(button.Input))
                    {
                        int beat = cryingTrack_Button.Beats[this.buttonCursors[iButton]];
                        if (Mathf.Abs(musicBeatTime - beat) < this.MaxButtonError)
                        {
                            // validated, add button score
                            ++this.buttonCursors[iButton];

                            float scoreToAdd = this.ButtonScores[iButton];
                            this.Score.ButtonScores[iButton] += scoreToAdd;
                            this.Score.TotalScore += scoreToAdd;
                            button.Scored.Invoke();
                            updateScore = true;
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            int beat = cryingTrack_Button.Beats[this.buttonCursors[iButton]];

                            if (musicBeatTime > beat + this.MaxButtonError)
                            {
                                ++this.buttonCursors[iButton];
                                button.Missed.Invoke();
                                if (this.buttonCursors[iButton] >= cryingTrack_Button.Beats.Length)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (updateScore)
            {
                this.OnScored.Invoke(Mathf.Floor(this.Score.TotalScore).ToString());
                if (this.currentProgressionStep < this.ProgressionStepCount)
                {
                    float nextProgScore = (this.currentProgressionStep + 1) * (this.ProgPercentPerStep / 100f) * this.maxScore;
                    if (nextProgScore < this.Score.TotalScore)
                    {
                        ++this.currentProgressionStep;
                        this.TimelinePlayer.MusicInstance.setParameterByName(this.ProgressionParameter, this.currentProgressionStep);
                    }
                }
            }
        }
    }

    public struct ScoreData
    {
        public float TotalScore;
        public float[] AnalogScores;
        public float[] ButtonScores;
    }
}
