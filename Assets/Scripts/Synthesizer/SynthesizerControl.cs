using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesizerControl : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.J)) JudgeTrack(0);
        if (Input.GetKeyDown(KeyCode.F)) JudgeTrack(1);
    }

    public void JudgeTrack(int trackNumber)
    {
        var note = judgers[trackNumber].GetNoteOnBeat(BeatTime.beat, judgeToleranceBeat);
        if (note.noteNum != -1) synthesizer.PlayNow(trackNumber, note.beginBeat, note.audioEndBeat);
    }

    public void JudgeKey(int trackNumber, int keyNumber)
    {
        var note = judgers[trackNumber].GetNoteOnBeat(BeatTime.beat, judgeToleranceBeat);
        if (note.noteNum != -1) synthesizer.PlayNow(trackNumber, note.beginBeat, note.audioEndBeat);
    }

    public void TriggerWithHandle(int handleNumber)
    {
        JudgeTrack(handleNumber);
    }

}
