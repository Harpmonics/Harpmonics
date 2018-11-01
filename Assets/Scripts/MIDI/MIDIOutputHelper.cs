using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A helper for other scripts wanting to produce MIDI output
/// </summary>
public class MIDIOutputHelper : MonoBehaviour
{
    /// <summary>
    /// The MIDI output device script to use
    /// </summary>
    public MIDIOutputDevice outputDevice;

    /// <summary>
    /// The MIDI note to send when calling PlayNote() and StopNote()
    /// </summary>
    public MIDINote note = MIDINote.C_4;

    /// <summary>
    /// True if a NoteOn has been sent but no NoteOff has been sent yet. False otherwise
    /// </summary>
    private bool noteIsActive = false;

    /// <summary>
    /// Returns true if a note is currently playing
    /// </summary>
    public bool IsPlaying()
    {
        return noteIsActive;
    }

    /// <summary>
    /// Play the note (set in the inspector) if it is not already playing
    /// </summary>
    public void PlayNote()
    {
        if (!noteIsActive)
        {
            outputDevice.SendNoteOn(note, 127);
            noteIsActive = true;
        }
    }

    /// <summary>
    /// Stop the note (set in the inspector) if it is playing
    /// </summary>
    public void StopNote()
    {
        if (noteIsActive)
        {
            outputDevice.SendNoteOff(note, 127);
            noteIsActive = false;
        }
    }

    /// <summary>
    /// Send a pitch bend value in the range [-8192, 8191] with 0 meaning no pitch bend
    /// </summary>
    public void SendPitchBend(int bend)
    {
        //Debug.Log("======================= MIDI BEND " + bend);
        outputDevice.SendPitchBend(bend);
    }

    public void SendController(int cc, int value)
    {
        //Debug.Log("======================= MIDI CC " + cc + " -> " + value);
        outputDevice.SendController(cc, value);
    }
}
