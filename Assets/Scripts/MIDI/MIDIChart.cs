using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MIDIChart : ScriptableObject
{
    public class Note : ScriptableObject
    {
        public int noteNum;
        public float beginBeat;
        public float endBeat;
    }

    public class Track : ScriptableObject
    {
        public Note[] notes;
    }

    public Track[] tracks;

}
