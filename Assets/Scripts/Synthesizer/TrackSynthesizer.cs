using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioBeatProvider))]
public class TrackSynthesizer : MonoBehaviour {
    
    public AudioSource[] playableTracks;

    public void PlayNow(int trackNum, float beginBeat, float endBeat)
    {
        if (trackNum < 0 || trackNum >= playableTracks.Length)
            return;

        double beginTime = BeatTime.timeOnBeat(beginBeat);
        double endTime = BeatTime.timeOnBeat(endBeat);
        double length = endTime - beginTime;

        AudioSource track = playableTracks[trackNum];
        
        track.SetScheduledEndTime(0);
        track.time = (float)beginTime;
        track.Play();
        track.SetScheduledEndTime(BeatTime.dspTime + length);
    }

    public void PlaySynchronized(int trackNum, float beginBeat, float endBeat)
    {
        if (trackNum < 0 || trackNum >= playableTracks.Length)
            return;
    }

    private AudioBeatProvider beatProvider;

}
