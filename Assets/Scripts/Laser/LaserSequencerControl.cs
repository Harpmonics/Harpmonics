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

    MIDIChart.Note lastPlayedNote;

    public void OnTriggerEnter(Collider other)
    {
        if (InputManager.IsUserInput(other) && judger != null)
        {
            var note = judger.HitNoteOnBeat(BeatTime.beat);
            if (note != null) sequencer.PlayNow(laser.trackIndex, note.beginBeat, note.audioEndBeat);
        }
    }

    void Start ()
    {
        laser = GetComponent<LaserBehaviour>();
        sequencer = GameObject.FindGameObjectWithTag("Sequencer").GetComponent<TrackSequencer>();
        judger = GetComponent<LaserMIDITimingJudge>();
	}

    void Update ()
    {
        var note = judger.NextJudgedNote;
        if (note != null && note != lastPlayedNote && note.beginBeat < BeatTime.beat)
        {
            //Debug.Log("Autoplay note " + note.beginBeat + " beginning at " + note.audioEndBeat);
            sequencer.PlaySynchronized(laser.trackIndex, note.audioEndBeat);
            lastPlayedNote = note;
        }
    }

}
