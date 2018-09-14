using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesizerTestControl : MonoBehaviour
{
    public MIDITimingJudger[] judgers;
    public float judgeToleranceBeat = 0.125f;

    TrackSynthesizer synthesizer;

	// Use this for initialization
	void Start ()
    {
        synthesizer = GetComponent<TrackSynthesizer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F)) JudgeTrack(0);
        if (Input.GetKeyDown(KeyCode.J)) JudgeTrack(1);
    }

    public void JudgeTrack(int trackNumber)
    {
        var note = judgers[trackNumber].GetNoteOnBeat(BeatTime.beat, judgeToleranceBeat);
        if (note.noteNum != -1) synthesizer.PlayNow(trackNumber, note.beginBeat, note.audioEndBeat);
    }

}
