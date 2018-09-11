using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, new string[] { "mid", "midi" })]
public class MIDIImporter : ScriptedImporter
{

    public override void OnImportAsset(AssetImportContext ctx)
    {
        MIDIChart chart = ScriptableObject.CreateInstance<MIDIChart>();

        MidiFile midi = MidiFile.Read(ctx.assetPath);

        float beatTicks = ((TicksPerQuarterNoteTimeDivision)midi.TimeDivision).TicksPerQuarterNote;

        var trackList = new List<MIDIChart.Track>();

        foreach (var chunk in midi.Chunks)
        {
            var track = chunk as TrackChunk;
            if (track == null)
                continue;

            var noteList = new List<MIDIChart.Note>();

            var notesManager = new NotesManager(track.Events);
            foreach (var midiNote in notesManager.Notes)
            {
                var note = ScriptableObject.CreateInstance<MIDIChart.Note>();
                note.noteNum = midiNote.NoteNumber;
                note.beginBeat = midiNote.Time / beatTicks;
                note.endBeat = midiNote.Length / beatTicks;
                noteList.Add(note);
            }

            var trackData = ScriptableObject.CreateInstance<MIDIChart.Track>();
            trackData.notes = noteList.ToArray();
            trackList.Add(trackData);
        }

        chart.tracks = trackList.ToArray();

        ctx.AddObjectToAsset("chart", chart);
        ctx.SetMainObject(chart);
    }

}
