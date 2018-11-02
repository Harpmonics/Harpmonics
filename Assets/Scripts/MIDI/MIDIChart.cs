using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class MIDIChart : ScriptableObject
{
    [Serializable]
    public class Note
    {
        public int noteNum;
        public float beginBeat;
        public float endBeat;
        public float audioEndBeat;
    }

    [Serializable]
    public class Track
    {
        public List<Note> notes;
    }

    public List<Track> tracks;

}
