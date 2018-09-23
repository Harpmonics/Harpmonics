using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaserMIDITimingJudge))]
[RequireComponent(typeof(LaserBehaviour))]
[RequireComponent(typeof(Collider))]
public class LaserSequencerControl : MonoBehaviour
{
    LaserBehaviour laser;
    TrackSequencer sequencer;
    LaserMIDITimingJudge judger;

    public void OnTriggerEnter(Collider other)
    {
        if (InputManager.IsUserInput(other) && judger != null)
        {
            var note = judger.GetNoteOnBeat(BeatTime.beat);

            if (note.noteNum != -1) sequencer.PlayNow(laser.trackIndex, note.beginBeat, note.audioEndBeat);
        }
    }

    void Start () {
        laser = GetComponent<LaserBehaviour>();
        sequencer = GameObject.FindGameObjectWithTag("Sequencer").GetComponent<TrackSequencer>();
        judger = GetComponent<LaserMIDITimingJudge>();
	}

}
