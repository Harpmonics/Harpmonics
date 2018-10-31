using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMIDITimingJudge : MonoBehaviour {

    LaserBehaviour laser;
    public MIDIChart.Note[] notes;
    
    int nextJudgedNote = 0;

    public float toleranceBeatOK = 0.5f;
    public float toleranceBeatGood = 0.25f;
    public float toleranceBeatPerfect = 0.125f;

    public int scoreOK = 20;
    public int scoreGood = 50;
    public int scorePerfect = 100;

    private bool alreadyInitialized = false;

    public MIDIChart.Note HitNoteOnBeat(float beat)
    {
        var tmpNote = new MIDIChart.Note { noteNum = -1, beginBeat = beat - toleranceBeatOK };
        int index = Array.BinarySearch(notes, tmpNote, Comparer<MIDIChart.Note>.Create((note1, note2) => note1.beginBeat.CompareTo(note2.beginBeat)));
        if (index < 0) index = ~index;
        while (index + 1 < notes.Length && Mathf.Abs(notes[index + 1].beginBeat - beat) <= Mathf.Abs(notes[index].beginBeat - beat)) ++index;
		if (index < notes.Length && index - 1 != nextJudgedNote && Mathf.Abs(notes[index].beginBeat - beat) <= toleranceBeatOK && !notes[index].played)
        {
            float diffBeat = Mathf.Abs(notes[index].beginBeat - beat);
            float accuracy = 1 - diffBeat / toleranceBeatOK;

            AccuracyGraph.TrackAccuracy(this.gameObject, accuracy, index);
            
            if (diffBeat <= toleranceBeatPerfect)
			{
				Feedback.alpha = 1f;
				ScoreStat.Score += scorePerfect;
				//print(Mathf.Abs(notes[index].beginBeat - beat) + " should reward 100 points");
				Feedback.fb = "Perfect";
			}
			else if (diffBeat <= toleranceBeatGood)
			{
				Feedback.alpha = 1f;
				ScoreStat.Score += scoreGood;
				//print(Mathf.Abs(notes[index].beginBeat - beat) + " should reward 50 points");
				Feedback.fb = "Good";
			}
			else
			{
				Feedback.alpha = 1f;
				ScoreStat.Score += scoreOK;
				//print(Mathf.Abs(notes[index].beginBeat - beat) + " should reward 20 points");
				Feedback.fb = "Ok";
			}

            notes[index].played = true;
			nextJudgedNote = index + 1;
			return notes[index];

        }

        return null;
    }

    public void Initialize()
    {
        if (alreadyInitialized)
            return;

        alreadyInitialized = true;

        laser = GetComponent<LaserBehaviour>();
        var pitchSet = new HashSet<int>(laser.assignedPitches);
        var rawNotes = new List<MIDIChart.Note>(laser.chart.tracks[laser.trackIndex].notes);

        // If there is no pitch distribution, then keep all notes for this track
        if (pitchSet.Count > 0)
            rawNotes.RemoveAll((MIDIChart.Note note) => !pitchSet.Contains(note.noteNum));

        notes = rawNotes.ToArray();
        nextJudgedNote = 0;
    }

    void Start()
    {
        Initialize();
    }

    public MIDIChart.Note NextJudgedNote
    {
        get
        {
            return notes != null && nextJudgedNote < notes.Length && notes[nextJudgedNote].beginBeat < BeatTime.beat + toleranceBeatOK ? notes[nextJudgedNote] : null;
        }
    }

}
