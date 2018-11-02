using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaserMIDITimingJudge))]
[RequireComponent(typeof(LaserBehaviour))]
[RequireComponent(typeof(Collider))]
public class LaserSequencerControl : MonoBehaviour
{
    LaserBehaviour laser;
    TrackSequencer sequencer;
    LaserNoteVisualier visualizer;
    LaserMIDITimingJudge judger;

    ControllerRumble rumble;

    MIDIChart.Note lastPlayedNote;

    // How many seconds is too long to hold the laser for?
    public float holdTolerance = 3.0f;

    /// <summary>
    /// Object maintaining the model of the laser
    /// </summary>
    private GameObject model;

    private Vector3 modelInitialScale;

    private float startHoldTime = 0f;
    private bool isHolding = false;

    public void OnTriggerEnter(Collider other)
    {
        if (InputManager.IsUserInput(other) && judger != null)
        {
            isHolding = true;

            startHoldTime = Time.time;

            var note = judger.HitNoteOnBeat(BeatTime.beat);
            if (note != null)
            {
                sequencer.PlayNow(laser.trackIndex, note.beginBeat, note.audioEndBeat);
                if (visualizer != null)
                    visualizer.PlayHitEffect(note.endBeat);

                if (rumble != null)
                {
                    rumble.StartRumble(other);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (InputManager.IsUserInput(other))
        {
            isHolding = false;
        }
    }

    void Start()
    {
        laser = GetComponent<LaserBehaviour>();
        sequencer = GameObject.FindGameObjectWithTag("Sequencer").GetComponent<TrackSequencer>();
        judger = GetComponent<LaserMIDITimingJudge>();
        visualizer = GetComponentInChildren<LaserNoteVisualier>();

        rumble = GetComponent<ControllerRumble>();

        model = this.transform.Find("Model").gameObject;
        modelInitialScale = model.transform.localScale;
    }

    void Update()
    {
        var note = judger.NextJudgedNote;
        if (note != null && note != lastPlayedNote && note.beginBeat < BeatTime.beat)
        {
            //Debug.Log("Autoplay note " + note.beginBeat + " beginning at " + note.audioEndBeat);
            sequencer.PlaySynchronized(laser.trackIndex, note.audioEndBeat);
            lastPlayedNote = note;
        }

        if (isHolding & (startHoldTime + holdTolerance - Time.time) < 0)
        {
            model.transform.localScale += (new Vector3(0, modelInitialScale.y, 0) - model.transform.localScale) * Time.deltaTime * 3;
        }
        else
        {
            model.transform.localScale += (modelInitialScale - model.transform.localScale) * Time.deltaTime * 3;
        }
    }

}
