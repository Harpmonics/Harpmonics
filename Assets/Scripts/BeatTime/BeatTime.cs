using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTime : MonoBehaviour {
    
    /********** Static Members **********/

    public static float beat { get; private set; }
    public static float deltaBeat { get; private set; }

    public static int numBeat { get; private set; }
    public static float subBeat { get; private set; }

    public static float accumulatedBeat { get; set; }

    public static double nextBeatTime { get; private set; }
    public static double lastBeatTime { get; private set; }
    public static double timeTillNextBeat { get; private set; }
    public static double timeFromLastBeat { get; private set; }

    public static double audioTime { get; private set; }
    public static double dspTime { get { return AudioSettings.dspTime; } }

    public static double deltaAudioTime { get; private set; }

    public static double timeOnBeat(float beat) { return instance.beatProvider.GetTimeFromBeat(beat); }
    public static float beatOnTime(double time) { return instance.beatProvider.GetBeatFromTime(time); }

    /********** Singleton Logic **********/

    /// <summary>
    /// The singleton instance.
    /// </summary>
    private static BeatTime instance = null;

    void OnDisable()
    {
        // Singleton class logic. There should be only one instance that updates static values.
        if (instance == this)
            instance = null;
    }

    /********** Beat Provider **********/
    
    private class DefaultBeatProvider : IBeatProvider
    {
        double startAudioTime = AudioSettings.dspTime;
        public double GetAudioTime() { return AudioSettings.dspTime - startAudioTime; }
        public float GetBeatFromTime(double audioTime) { return (float)(audioTime * 2); } // 120BPM for default.
        public double GetTimeFromBeat(float beat) { return audioTime * 0.5; }
    }

    [SerializeField]
    public IBeatProvider beatProvider
    {
        get
        {
            return m_beatProvider;
        }
        set
        {
            if (value == null)
                value = new DefaultBeatProvider();
            float offsetBeat = 0;
            if (m_beatProvider != null)
            {
                var currAudioTime = m_beatProvider.GetAudioTime();
                offsetBeat = m_beatProvider.GetBeatFromTime(currAudioTime) - m_lastBeat;
            }
            m_beatProvider = value;
            m_lastAudioTime = value.GetAudioTime();
            m_lastBeat = value.GetBeatFromTime(m_lastAudioTime) - offsetBeat;
            m_lastNumBeat = int.MinValue;
        }
    }

    private IBeatProvider m_beatProvider = null;
    private double m_lastAudioTime;
    private float m_lastBeat;
    private float m_lastNumBeat = int.MinValue;

    /********** Monobehaviour **********/

    void Awake()
    {
        beatProvider = m_beatProvider; // Reset benchmark data (and possibly init default beat provider). See beatProvider.set
    }

    void Start()
    {
        Update();
    }

    void Update()
    {
        // Singleton class logic. Checks if last instance is disabled/removed and replace it
        if (BeatTime.instance == null)
            BeatTime.instance = this;

        var currAudioTime = beatProvider.GetAudioTime();
        var currBeat = beatProvider.GetBeatFromTime(currAudioTime);

        // Static update. Only the singleton instance do this.
        if (BeatTime.instance == this)
        {
            BeatTime.deltaAudioTime = currAudioTime - BeatTime.audioTime;
            BeatTime.audioTime = currAudioTime;

            BeatTime.beat = currBeat;
            BeatTime.deltaBeat = currBeat - m_lastBeat;
            BeatTime.numBeat = currBeat < 0 ? -1 : (int)Math.Floor(currBeat);
            BeatTime.subBeat = currBeat - BeatTime.numBeat;

            if (m_lastNumBeat != BeatTime.numBeat)
            {
                BeatTime.lastBeatTime = m_lastNumBeat < 0 ? double.MinValue : beatProvider.GetTimeFromBeat(currBeat);
                BeatTime.nextBeatTime = beatProvider.GetTimeFromBeat(BeatTime.numBeat + 1);
            }
            BeatTime.timeFromLastBeat = BeatTime.audioTime - BeatTime.lastBeatTime;
            BeatTime.timeTillNextBeat = BeatTime.nextBeatTime - BeatTime.audioTime;

            BeatTime.accumulatedBeat += deltaBeat;
        }

        // Update benchmark data
        m_lastAudioTime = currAudioTime;
        m_lastBeat = currBeat;
        m_lastNumBeat = BeatTime.numBeat;

    }

}
