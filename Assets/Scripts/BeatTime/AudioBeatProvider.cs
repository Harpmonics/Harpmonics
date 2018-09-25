using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(BeatTime))]
public class AudioBeatProvider : MonoBehaviour, IBeatProvider
{
    public AudioSource audioSource;

    public float offsetBeforePlay;
    public double offsetFirstBeat;

    public bool debugStart = false;
    public float debugStartBeat = 0;

    public bool loop = false;
    public Vector2 loopRangeBeat;

    private AudioSource audioSourceCopy;

    private double loopStartTime;
    private double loopEndTime;
    private double loopLengthTime;
    private float loopLengthBeat;

    public int loopCount
    {
        get
        {
            return (int)Math.Floor((BeatTime.dspTime - scheduledStartTime - loopStartTime) / loopLengthTime);
        }
    }

    private int scheduledLoopCount = 0;

    /// <summary>
    /// BPM list. X value for beat, Y value for BPM.
    /// </summary>
    public Vector2[] bpmList = { new Vector2(0, 120) };

    private float[] beatList;
    private double[] offsetList;

    private double scheduledStartTime;

    public double GetAudioTime()
    {
        if (!audioSource.isPlaying && (audioSourceCopy == null || !audioSourceCopy.isPlaying))
            return -offsetBeforePlay;
        if (BeatTime.dspTime < scheduledStartTime)
            return BeatTime.dspTime - scheduledStartTime;
        return BeatTime.dspTime - scheduledStartTime; // this makes the audio time continues to increase when looped
        //return audioSource.time;
    }

    public float GetBeatFromTime(double audioTime)
    {
        int loopCount = 0;
        if (loop && audioTime > loopEndTime)
        {
            loopCount = (int)Math.Floor((audioTime - loopStartTime) / loopLengthTime);
            audioTime -= loopCount * loopLengthTime;
        }
        int i = Array.BinarySearch(offsetList, audioTime);
        if (i < 0) i = Math.Max(~i - 1, 0);
        float result = beatList[i] + (float)(audioTime - offsetList[i]) * bpmList[i].y / 60;
        if (loop && loopCount > 0)
        {
            result += loopCount * loopLengthBeat;
        }
        return result;
    }

    public double GetTimeFromBeat(float beat)
    {
        int loopCount = 0;
        if (loop && beat > loopRangeBeat.y)
        {
            loopCount = (int)Math.Floor((beat - loopRangeBeat.x) / loopLengthBeat);
            beat -= loopCount * loopLengthBeat;
        }
        int i = Array.BinarySearch(beatList, beat);
        if (i < 0) i = Math.Max(~i - 1, 0);
        double result = offsetList[i] + (beat - beatList[i]) / bpmList[i].y * 60;
        if (loop && loopCount > 0)
        {
            result += loopCount * loopLengthTime;
        }
        return result;
    }

    void OnValidate()
    {
        if (bpmList.Length == 0)
        {
            Debug.LogWarning("Bpm List should contain at least 1 element.");
            bpmList = new Vector2[]{ new Vector2(120, 0) };
        }
        if (loopRangeBeat.x < 0)
            loopRangeBeat.x = 0;
        if (loopRangeBeat.y <= loopRangeBeat.x)
            loopRangeBeat.y = loopRangeBeat.x + 1;
    }
    
    void Start()
    {
        if (bpmList != null)
        {
            offsetList = new double[bpmList.Length];
            beatList = new float[bpmList.Length];
            double lastOffset = offsetFirstBeat;
            double lastBeat = 0;
            double lastBPM = bpmList[0].y;
            for (int i = 0; i < bpmList.Length; ++i)
            {
                beatList[i] = bpmList[i].x;
                offsetList[i] = lastOffset + (bpmList[i].x - lastBeat) / lastBPM * 60;
                lastOffset = offsetList[i];
                lastBeat = bpmList[i].x;
                lastBPM = bpmList[i].y;
            }
        }

        if (loop)
        {
            loopStartTime = GetTimeFromBeat(loopRangeBeat.x);
            loopEndTime = GetTimeFromBeat(loopRangeBeat.y);
            loopLengthTime = loopEndTime - loopStartTime;
            loopLengthBeat = loopRangeBeat.y - loopRangeBeat.x;
            audioSourceCopy = gameObject.AddComponent<AudioSource>();
            //ComponentUtility.CopyComponent(audioSource);
            //ComponentUtility.PasteComponentValues(audioSourceCopy);
            PropertyInfo[] props = typeof(AudioSource).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo prop in props)
            {
                if (!prop.CanWrite || !prop.CanRead ||
                    Array.Find(prop.GetCustomAttributes(false), o => o is ObsoleteAttribute) != null)
                    continue;
                prop.SetValue(audioSourceCopy, prop.GetValue(audioSource, null), null);
            }
        }

        var beatTime = GetComponent<BeatTime>();
        beatTime.beatProvider = this;

        if (debugStart)
        {
            double startAudioTime = GetTimeFromBeat(debugStartBeat);
            audioSource.time = (float)startAudioTime;
            scheduledStartTime = BeatTime.dspTime + offsetBeforePlay;
            audioSource.PlayScheduled(scheduledStartTime);
            scheduledStartTime -= startAudioTime;
        }
        else
        {
            scheduledStartTime = BeatTime.dspTime + offsetBeforePlay;
            audioSource.PlayScheduled(scheduledStartTime);
        }

    }

    void Update()
    {
        if (loop && scheduledLoopCount <= loopCount)
        {
            scheduledLoopCount = loopCount + 1;

            double scheduledLoopTime = scheduledStartTime + loopCount * loopLengthTime + loopEndTime;

            audioSourceCopy.time = (float)loopStartTime;

            audioSource.SetScheduledEndTime(scheduledLoopTime);
            audioSourceCopy.PlayScheduled(scheduledLoopTime);

            var tmp = audioSource;
            audioSource = audioSourceCopy;
            audioSourceCopy = tmp;

        }
    }

    void OnDisable()
    {
        if (audioSource != null) audioSource.Pause();
        if (audioSourceCopy != null) audioSourceCopy.Pause(); // todo: how to unpause the looped music?
    }

}
