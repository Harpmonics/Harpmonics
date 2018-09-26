using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerControl_alt : MonoBehaviour
{
    public MIDITimingJudger[] judgers;
    public float judgeToleranceBeat = 0.125f;

    TrackSequencer sequencer;

    // Use this for initialization
    void Start()
    {
        sequencer = GetComponent<TrackSequencer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) JudgeTrack(0);
        if (Input.GetKeyDown(KeyCode.F)) JudgeTrack(1);
    }

    public void JudgeTrack(int trackNumber)
    {
        var note = judgers[trackNumber].GetNoteOnBeat(BeatTime.beat, judgeToleranceBeat);
        if (note != null) sequencer.PlayNow(trackNumber, note.beginBeat, note.audioEndBeat);
    }

    //public override void Callback(GameObject caller, GameObject activator, bool touching)
    //{
    //    if (touching && caller.tag.StartsWith("Track"))
    //    {
    //        int track = -1;

    //        // Extract track from callback
    //        int.TryParse(caller.tag.Substring(6), out track);

    //        if (track != -1)
    //            JudgeTrack(track);
    //    }
    //}
}
