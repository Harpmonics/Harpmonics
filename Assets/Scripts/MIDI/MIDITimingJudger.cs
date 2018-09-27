using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIDITimingJudger : MonoBehaviour {

    public MIDIChart chart;
    public int trackNumber;
    public int[] pitchNumbers;

    MIDIChart.Note[] notes;

    public MIDIChart.Note GetNoteOnBeat(float beat, float tolerance)
    {
        var tmpNote = new MIDIChart.Note { noteNum = -1, beginBeat = beat - tolerance };
        int index = Array.BinarySearch(notes, tmpNote, Comparer<MIDIChart.Note>.Create((note1, note2) => note1.beginBeat.CompareTo(note2.beginBeat)));
        if (index < 0) index = ~index;
        while (index + 1 < notes.Length && Mathf.Abs(notes[index + 1].beginBeat - beat) <= Mathf.Abs(notes[index].beginBeat - beat)) ++index;
        Debug.Log(trackNumber + " " +
            (index < notes.Length ? (beat - notes[index].beginBeat).ToString() + " " + 
            (Mathf.Abs(notes[index].beginBeat - beat) <= tolerance ? "Hit" : "Missed") : "Null Missed"));
        if (index < notes.Length && Mathf.Abs(notes[index].beginBeat - beat) <= tolerance)
            return notes[index];
        return null;
    }

    void Start()
    {
        notes = chart.tracks[trackNumber].notes.ToArray();
    }

}
