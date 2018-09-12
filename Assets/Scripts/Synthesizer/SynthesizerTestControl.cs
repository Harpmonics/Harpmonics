using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesizerTestControl : MonoBehaviour
{
    public MIDITimingJudger[] judgers;

    TrackSynthesizer synthesizer;

	// Use this for initialization
	void Start ()
    {
        synthesizer = GetComponent<TrackSynthesizer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        int currBeat = Mathf.RoundToInt(BeatTime.beat);
        if (Input.GetKeyDown(KeyCode.F))
        {
            var note = judgers[0].GetNoteOnBeat(BeatTime.beat, 0.125f);
            if (note.noteNum != -1) synthesizer.PlayNow(0, note.beginBeat, note.audioEndBeat);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            var note = judgers[1].GetNoteOnBeat(BeatTime.beat, 0.125f);
            if (note.noteNum != -1) synthesizer.PlayNow(1, note.beginBeat, note.audioEndBeat);
        }
    }

}
