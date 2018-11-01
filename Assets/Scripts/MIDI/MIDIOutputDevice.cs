using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sanford.Multimedia.Midi;

/// <summary>
/// A MIDIOutputDevice instance represents a single system MIDI output device onto which clients
/// can write certain types of MIDI data by using the interface provided by the class. Currently
/// only sends MIDI data on channel 0.
/// </summary>
[ExecuteInEditMode]
public class MIDIOutputDevice : MonoBehaviour
{
    /// <summary>
    /// ID of output device to open
    /// </summary>
    public int midiOutputDeviceId = 0;

    /// <summary>
    /// A list for displaying the available output devices in the editor/inspector.
    /// Only used to inform the user of available device IDs.
    /// TODO: Make this a dropdown select in the inspector
    /// </summary>
    public List<string> availableMidiDevices = new List<string>();

    /// <summary>
    /// ID of currently open output device, or -1 if no device has been opened
    /// </summary>
    private int openedOutputDevice = -1;

    /// <summary>
    /// Actual MIDI output interface
    /// </summary>
    private Sanford.Multimedia.Midi.OutputDevice output;

    /// <summary>
    /// On awakening we populate the list of available devices
    /// </summary>
    private void Awake()
    {
        PopulateDeviceList();
    }

    /// <summary>
    /// Open the MIDI output device when entering Play mode
    /// </summary>
    private void Start()
    {
        if (Application.isPlaying)
        {
            EnsureDeviceOpen();
        }
    }

    /// <summary>
    /// Close the MIDI device (if opened) when destroying the script.
    /// </summary>
    private void OnDestroy()
    {
        if (output != null)
        {
            Debug.Log("Closing MIDI device " + openedOutputDevice);

            try
            {
                output.Close();
            }
            catch { }

            openedOutputDevice = -1;
        }
    }

    /// <summary>
    /// Populate the list of available output devices
    /// </summary>
    private void PopulateDeviceList()
    {
        if (!Application.isEditor)
        {
            return;
        }

        availableMidiDevices.Clear();

        for (int i = 0; i < OutputDevice.DeviceCount; ++i)
        {
            availableMidiDevices.Add(OutputDevice.GetDeviceCapabilities(i).name);
        }
    }

    /// <summary>
    /// Try to ensure that the selected device is opened by closing the open device (if any,
    /// and only if it is not the selected device) and opening the selected device (if not already open)
    /// </summary>
    private void EnsureDeviceOpen()
    {
        // Is the requested output device NOT currently opened?
        if (openedOutputDevice != midiOutputDeviceId)
        {
            // Is there some other output device open?
            if (openedOutputDevice != -1)
            {
                // Then close it
                try
                {
                    //output.Reset();
                    output.Close();
                    Debug.Log("Closed MIDI Output Device " + openedOutputDevice);
                }
                finally
                {
                    openedOutputDevice = -1;
                }
            }

            // Now try to open the requested device
            try
            {
                output = new OutputDevice(midiOutputDeviceId);
                openedOutputDevice = midiOutputDeviceId;
                Debug.Log("Opened MIDI Output Device " + midiOutputDeviceId);
            }
            catch
            {
                Debug.Log("Failed to open MIDI Output Device " + midiOutputDeviceId);
                output = null;
                openedOutputDevice = -1;
            }
        }
    }

    /// <summary>
    /// Send a NoteOn MIDI message with a given velocity
    /// </summary>
    public void SendNoteOn(MIDINote note, int velocity)
    {
        SendNoteOn((int)note, velocity);
    }

    /// <summary>
    /// Send a NoteOn MIDI message with a given velocity
    /// Valid notes: see https://cdn-images-1.medium.com/max/1200/1*CDXHKG0-4QO9Y-DCTAcqPg.png
    /// Valid velocity: 0-127
    /// </summary>
    public void SendNoteOn(int note, int velocity)
    {
        if (output == null)
        {
            Debug.Log("sendNoteOn on unopened device");
            return;
        }

        //Debug.Log("MIDI NoteOn " + note + ", velocity: " + velocity);
        output.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, note, Mathf.Clamp(velocity, 0, 127)));
    }

    /// <summary>
    /// Send a NoteOff MIDI message
    /// </summary>
    public void SendNoteOff(MIDINote note)
    {
        SendNoteOff((int)note);
    }

    /// <summary>
    /// Send a NoteOff MIDI message with zero velocity
    /// Valid notes: see https://cdn-images-1.medium.com/max/1200/1*CDXHKG0-4QO9Y-DCTAcqPg.png
    /// </summary>
    public void SendNoteOff(int note)
    {
        SendNoteOff(note, 0);
    }

    /// <summary>
    /// Send a NoteOff MIDI message with a given velocity
    /// Valid velocity: 0-127
    /// </summary>
    public void SendNoteOff(MIDINote note, int velocity)
    {
        SendNoteOff((int)note, velocity);
    }

    /// <summary>
    /// Send a NoteOff MIDI message with a given velocity
    /// Valid notes: see https://cdn-images-1.medium.com/max/1200/1*CDXHKG0-4QO9Y-DCTAcqPg.png
    /// Valid velocity: 0-127
    /// </summary>
    public void SendNoteOff(int note, int velocity)
    {
        if (output == null)
        {
            Debug.Log("sendNoteOff on unopened device");
            return;
        }

        //Debug.Log("MIDI NoteOff " + note + ", velocity: " + velocity);
        output.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, note, Mathf.Clamp(velocity, 0, 127)));
    }

    /// <summary>
    /// Send a Pitch Wheel (Pitch Bend) MIDI message
    /// Valid range for bend is [-8192, 8191] with 0 being neutral/none
    /// </summary>
    public void SendPitchBend(int bend)
    {
        if (output == null)
        {
            Debug.Log("SendPitchBend on unopened device");
            return;
        }

        if (bend < -8192 || bend > 8191)
        {
            Debug.Log("SendPitchBend bend argument out of range");
            return;
        }

        // See http://midi.teragonaudio.com/tech/midispec/wheel.htm
        // bend comes in the range [-8192, 8191], we want to produce a 14-bit value in the range [0, 16383]
        bend += 8192;
        
        // mask the lower 14 bits
        // 0000 0000 0000 0000   0000 0000 0000 0000   0000 0000 0000 0000   0011 1111 1111 1111
        bend &= 0x3fff;

        // The first data byte bits 0 to 6 are bits 0 to 6 of the 14-bit value.
        // 0000 0000 0000 0000   0000 0000 0000 0000   0000 0000 0000 0000   0011 1111 1111 1111
        // 0000 0000 0000 0000   0000 0000 0000 0000   0000 0000 0000 0000   0011 1111 1000 0000
        int first = (bend & 0x3f80) >> 7;

        // The second data byte bits 0 to 6 are bits 7 to 13 of the 14-bit value.
        // 0000 0000 0000 0000   0000 0000 0000 0000   0000 0000 0000 0000   0011 1111 1111 1111
        // 0000 0000 0000 0000   0000 0000 0000 0000   0000 0000 0000 0000   0000 0000 0111 1111
        int second = bend & 0x7f;
        
        //Debug.Log("MIDI Pitch Wheel " + bend);

        // we send the bytes in reverse order since MIDI is big-endian
        output.Send(new ChannelMessage(ChannelCommand.PitchWheel, 0, second, first));
    }

    /// <summary>
    /// Send a Controller Change MIDI message
    /// Valid range for cc (the controller number) is [0, 127]
    /// Valid range for value is [0, 127]
    /// see: https://www.midi.org/specifications-old/item/table-3-control-change-messages-data-bytes-2
    /// </summary>
    public void SendController(int cc, int value)
    {
        if (output == null)
        {
            Debug.Log("SendController on unopened device");
            return;
        }

        output.Send(new ChannelMessage(ChannelCommand.Controller, 0, cc, value));
    }
}
