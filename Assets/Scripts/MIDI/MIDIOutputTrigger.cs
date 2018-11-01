using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A very simple InputManager-aware trigger for sending a single MIDI note
/// </summary>
public class MIDIOutputTrigger : MonoBehaviour
{
    /// <summary>
    /// The MIDI output device script to use
    /// </summary>
    public MIDIOutputDevice outputDevice;

    /// <summary>
    /// The MIDI note to send
    /// </summary>
    public MIDINote note = MIDINote.C_4;

    /// <summary>
    /// True if a NoteOn has been sent but no NoteOff has been sent yet. False otherwise
    /// </summary>
    private bool noteIsActive = false;

    /// <summary>
    /// Send NoteOn (only if not already active) when the trigger is entered
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (!noteIsActive && InputManager.IsUserInput(other))
        {
            outputDevice.SendNoteOn(note, 127);
            noteIsActive = true;
        }
    }

    /// <summary>
    /// Send NoteOff (only if note is active) when the trigger is exited
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (noteIsActive && InputManager.IsUserInput(other))
        {
            outputDevice.SendNoteOff(note, 127);
            noteIsActive = false;
        }
    }
}
