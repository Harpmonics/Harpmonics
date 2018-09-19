using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthControllerNotifier : MonoBehaviour
{
    public string triggerTag;
    public GameObject synth;
    public int track;
    private SequencerControl_alt synthControl;

	// Use this for initialization
	void Start ()
    {
        synthControl = synth.GetComponent<SequencerControl_alt>();
	}

    public void OnTriggerEnter(Collider other)
    {
        if (synthControl != null && other.CompareTag(triggerTag))
        {
            synthControl.JudgeTrack(track);
        }
    }
}
