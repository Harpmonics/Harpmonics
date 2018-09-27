using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoteBehaviour))]
public class SimpleNote : MonoBehaviour
{
    public Vector3 judgePosition = new Vector3(0, 0, 0);
    public Vector3 offsetPerBeat = new Vector3(0, 0, 2);

    NoteBehaviour behaviour;
    bool shapeUpdated = false;
    
    void Start ()
    {
        behaviour = GetComponent<NoteBehaviour>();

        // Ensure that the note only appears at its required position rather than wherever it may have been when instantiated.
        Update();
    }

    void OnEnable()
    {
        behaviour = GetComponent<NoteBehaviour>();
        Update();
    }

    void Update ()
    {
        MIDIChart.Note noteData = behaviour.NoteData;
        if (!shapeUpdated)
        {
            transform.localRotation = Quaternion.LookRotation(offsetPerBeat);
            transform.localScale = new Vector3(1, 1, (noteData.endBeat - noteData.beginBeat) * offsetPerBeat.magnitude);
            shapeUpdated = true;
        }
        if (noteData.endBeat < BeatTime.beat)
        {
            shapeUpdated = false;
            behaviour.Recycle();
        }
        else if (noteData.beginBeat < BeatTime.beat)
        {
            transform.localPosition = judgePosition;
            transform.localScale = new Vector3(1, 1, (noteData.endBeat - BeatTime.beat) * offsetPerBeat.magnitude);
        }
        else
        {
            transform.localPosition = judgePosition + (noteData.beginBeat - BeatTime.beat) * offsetPerBeat;
        }
    }
}
