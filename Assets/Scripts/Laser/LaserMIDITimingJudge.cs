using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMIDITimingJudge : MonoBehaviour {

    LaserBehaviour laser;
    MIDIChart.Note[] notes;

    public float toleranceBeat = 0.25f;

    public MIDIChart.Note GetNoteOnBeat(float beat)
    {
        var tmpNote = new MIDIChart.Note { noteNum = -1, beginBeat = beat - toleranceBeat };
        int index = Array.BinarySearch(notes, tmpNote, Comparer<MIDIChart.Note>.Create((note1, note2) => note1.beginBeat.CompareTo(note2.beginBeat)));
        if (index < 0) index = ~index;
        while (index + 1 < notes.Length && Mathf.Abs(notes[index + 1].beginBeat - beat) <= Mathf.Abs(notes[index].beginBeat - beat)) ++index;
        if (index < notes.Length && Mathf.Abs(notes[index].beginBeat - beat) <= toleranceBeat)
            return notes[index];
        return tmpNote;
    }

    void Start()
    {
        laser = GetComponent<LaserBehaviour>();
        var pitchSet = new HashSet<int>(laser.assignedPitches);
        var rawNotes = new List<MIDIChart.Note>(laser.chart.tracks[laser.trackIndex].notes);

        // If there is no pitch distribution, then keep all notes for this track
        if (pitchSet.Count > 0)
            rawNotes.RemoveAll((MIDIChart.Note note) => !pitchSet.Contains(note.noteNum));

        notes = rawNotes.ToArray();
    }

}
