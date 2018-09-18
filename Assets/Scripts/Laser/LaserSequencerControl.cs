using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaserMIDITimingJudge))]
[RequireComponent(typeof(LaserBehaviour))]
public class LaserSequencerControl : ATouchCallee
{
    LaserBehaviour laser;
    TrackSynthesizer sequencer;
    LaserMIDITimingJudge judger;

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        if (touching && judger != null)
        {
            var note = judger.GetNoteOnBeat(BeatTime.beat);
            if (note.noteNum != -1) sequencer.PlayNow(laser.trackIndex, note.beginBeat, note.audioEndBeat);
        }
    }

    void Start () {
        laser = GetComponent<LaserBehaviour>();
        sequencer = GameObject.FindGameObjectWithTag("Sequencer").GetComponent<TrackSynthesizer>();
        judger = GetComponent<LaserMIDITimingJudge>();
	}

}
