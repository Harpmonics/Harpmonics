using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoteFactory))]
public class LaserNoteVisualier : MonoBehaviour
{
    public LaserBehaviour laser;
    MIDIChart chart;
    int trackToVisualize;
    int[] keysToVisualize;

    public float noteShowOffsetBeat = 4;

    NoteFactory factory;

    Queue<MIDIChart.Note> pendingNotes = new Queue<MIDIChart.Note>();

    void Start()
    {
        chart = laser.chart;
        trackToVisualize = laser.trackIndex;
        keysToVisualize = laser.assignedPitches;

        factory = GetComponent<NoteFactory>();
        
        if (keysToVisualize == null || keysToVisualize.Length == 0)
        {
            foreach (var note in chart.tracks[trackToVisualize].notes)
                pendingNotes.Enqueue(note);
        }
        else
        {
            var keySet = new HashSet<int>(keysToVisualize);
            foreach (var note in chart.tracks[trackToVisualize].notes)
                if (keySet.Contains(note.noteNum))
                    pendingNotes.Enqueue(note);
        }
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

