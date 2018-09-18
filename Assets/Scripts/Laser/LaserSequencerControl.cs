using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSequencerControl : ATouchCallee
{
    LaserBehaviour laser;
    SynthesizerControl sequencer;

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        if (touching)
        {
            sequencer.JudgeTrack(laser.trackIndex);
        }
    }

    void Start () {
        laser = GetComponent<LaserBehaviour>();
        sequencer = GameObject.FindGameObjectWithTag("Sequencer").GetComponent<SynthesizerControl>();
	}

}
