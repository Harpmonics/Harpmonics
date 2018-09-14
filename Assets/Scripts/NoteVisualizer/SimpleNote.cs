using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoteBehaviour))]
public class SimpleNote : MonoBehaviour
{
    public Vector3 judgePosition = new Vector3(0, 0, 0);
    public Vector3 offsetPerBeat = new Vector3(0, 2, 0);

    NoteBehaviour behaviour;
    
    void Start ()
    {
        behaviour = GetComponent<NoteBehaviour>();
    }
	
	void Update ()
    {
        MIDIChart.Note noteData = behaviour.NoteData;
        transform.localScale = new Vector3(1, (noteData.endBeat - noteData.beginBeat) * offsetPerBeat.magnitude, 1);
        transform.localPosition = judgePosition + (noteData.beginBeat - BeatTime.beat) * offsetPerBeat;
    }
}
