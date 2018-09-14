using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoteFactory))]
public class SimpleNoteVisualizer : MonoBehaviour
{
    public MIDIChart chart;
    public int trackToVisualize;

    public float noteShowOffsetBeat = 4;

    NoteFactory factory;

    Queue<MIDIChart.Note> pendingNotes = new Queue<MIDIChart.Note>();

    void Start()
    {
        factory = GetComponent<NoteFactory>();
        
        foreach (var note in chart.tracks[trackToVisualize].notes)
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
