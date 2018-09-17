using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteFactory : MonoBehaviour
{
    public NoteBehaviour assetNote;

    Queue<NoteBehaviour> recycleList = new Queue<NoteBehaviour>();

    public NoteBehaviour CreateNote(MIDIChart.Note noteData)
    {
        NoteBehaviour note;
        if (recycleList.Count == 0)
        {
            note = Instantiate(assetNote, transform);
            note.Factory = this;
            note.NoteData = noteData;
        }
        else
        {
            note = recycleList.Dequeue();
            note.Factory = this;
            note.NoteData = noteData;
            note.gameObject.SetActive(true);
        }
        return note;
    }

    public void RecycleNote(NoteBehaviour note)
    {
        note.gameObject.SetActive(false);
        note.NoteData = null;
        recycleList.Enqueue(note);
    }

}
