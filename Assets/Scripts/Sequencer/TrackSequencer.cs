using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioBeatProvider))]
public class TrackSequencer : MonoBehaviour {
    
    public AudioSource[] playableTracks;
    public float volumePlayNow = 1f;
    public float volumePlaySynchronized = 0.6f;

    bool isSynchronized = false;

    public void PlayNow(int trackNum, float beginBeat, float endBeat)
    {
        if (trackNum < 0 || trackNum >= playableTracks.Length)
            return;

        double beginTime = BeatTime.timeOnBeat(beginBeat);
        double endTime = BeatTime.timeOnBeat(endBeat);
        double length = endTime - beginTime;

        AudioSource track = playableTracks[trackNum];
        track.volume = volumePlayNow;

        track.SetScheduledEndTime(0);
        track.time = (float)beginTime;
        track.Play();
        track.SetScheduledEndTime(BeatTime.dspTime + length);

        isSynchronized = false;
    }

    public void PlaySynchronized(int trackNum, float beginBeat, float endBeat = float.PositiveInfinity)
    {
        if (trackNum < 0 || trackNum >= playableTracks.Length)
            return;

        double beginTime = BeatTime.timeOnBeat(beginBeat);
        double endTime = float.IsInfinity(endBeat) ? float.PositiveInfinity : BeatTime.timeOnBeat(endBeat);
        double length = endTime - beginTime;

        AudioSource track = playableTracks[trackNum];
        track.volume = volumePlaySynchronized;

        double bgmTime = BeatTime.audioTime;
        if (beginTime > bgmTime)
        {
            track.SetScheduledEndTime(0);
            double beginDspTime = BeatTime.dspTime + (beginTime - bgmTime);
            if (isSynchronized && track.isPlaying)
            {
                track.SetScheduledEndTime(beginDspTime + length);
            }
            else
            {
                track.time = (float)beginTime;
                track.PlayScheduled(beginDspTime);
                track.SetScheduledEndTime(beginDspTime + length);
            }
        }
        else
        {
            track.SetScheduledEndTime(0);
            if (!track.isPlaying)
                track.Play();
            if (!isSynchronized)
                track.time = (float)BeatTime.audioTime;
            track.SetScheduledEndTime(BeatTime.dspTime + length);
        }

        isSynchronized = true;
    }

    private AudioBeatProvider beatProvider;

}
