using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIDITimingJudger : MonoBehaviour {

    public MIDIChart chart;
    public int trackNumber;

    MIDIChart.Note[] notes;

    public MIDIChart.Note GetNoteOnBeat(float beat, float tolerance)
    {
        var tmpNote = new MIDIChart.Note { noteNum = -1, beginBeat = beat - tolerance };
        int index = Array.BinarySearch(notes, tmpNote, Comparer<MIDIChart.Note>.Create((note1, note2) => note1.beginBeat.CompareTo(note2.beginBeat)));
        if (index < 0) index = ~index;
        Debug.Log(trackNumber + " " + (index < notes.Length ?
            notes[index].beginBeat.ToString() + " " + (Mathf.Abs(notes[index].beginBeat - beat) <= tolerance ?
                "Hit " + (notes[index].audioEndBeat - notes[index].beginBeat) :
                "Missed") :
            "Null Missed"));
        if (index < notes.Length && Mathf.Abs(notes[index].beginBeat - beat) <= tolerance)
            return notes[index];
        return tmpNote;
    }

    void Start()
    {
        notes = chart.tracks[trackNumber].notes.ToArray();
    }

}
