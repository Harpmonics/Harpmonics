using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviour : MonoBehaviour {
    
    public MIDIChart.Note NoteData { get; set; }
    public NoteFactory Factory { get; set; }

    public void Recycle()
    {
        if (Factory == null)
        {
            Destroy(gameObject);
        }
        else
        {
            Factory.RecycleNote(this);
        }
    }

}
