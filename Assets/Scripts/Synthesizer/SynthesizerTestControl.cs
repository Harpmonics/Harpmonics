using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesizerTestControl : MonoBehaviour {

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
            synthesizer.PlayNow(0, currBeat, currBeat + 1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            synthesizer.PlayNow(1, currBeat, currBeat + 1);
        }
    }

}
