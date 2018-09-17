using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoteFactory))]
public class SimpleNoteVisualizer : MonoBehaviour
{
    public MIDIChart chart;
    public int[] tracksToVisualize;

    public float noteShowOffsetBeat = 4;

    NoteFactory factory;

    Queue<MIDIChart.Note> pendingNotes = new Queue<MIDIChart.Note>();

    void Start()
    {
        factory = GetComponent<NoteFactory>();

        var noteList = new List<MIDIChart.Note>();
        foreach (int trackNumber in tracksToVisualize)
            foreach (var note in chart.tracks[trackNumber].notes)
                noteList.Add(note);
        noteList.Sort((MIDIChart.Note x, MIDIChart.Note y) => x.beginBeat.CompareTo(y.beginBeat));
        foreach (var note in noteList)
            pendingNotes.Enqueue(note);
    }

    void Update()
    {
        while (pendingNotes.Count != 0 && pendingNotes.Peek().beginBeat < BeatTime.beat + noteShowOffsetBeat)
        {
            var note = pendingNotes.Dequeue();
            factory.CreateNote(note);
        }
    }


}
