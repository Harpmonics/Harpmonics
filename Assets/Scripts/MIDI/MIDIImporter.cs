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
            var trackChunk = chunk as TrackChunk;
            if (trackChunk == null)
                continue;

            var noteList = new List<MIDIChart.Note>();

            var notesManager = new NotesManager(trackChunk.Events);
            foreach (var midiNote in notesManager.Notes)
            {
                float noteBeginBeat = midiNote.Time / beatTicks;
                float noteEndBeat = noteBeginBeat + midiNote.Length / beatTicks;
                noteList.Add(new MIDIChart.Note
                {
                    noteNum = midiNote.NoteNumber,
                    beginBeat = noteBeginBeat,
                    endBeat = noteEndBeat,
                    audioEndBeat = float.PositiveInfinity
                });
            }

            noteList.Sort((MIDIChart.Note x, MIDIChart.Note y) => x.beginBeat.CompareTo(y.beginBeat));
            
            for (int i = 0; i < noteList.Count; ++i)
            {
                for (int j = i + 1; j < noteList.Count; ++j)
                {
                    if (noteList[j].beginBeat >= noteList[i].endBeat)
                    {
                        noteList[i].audioEndBeat = noteList[j].beginBeat;
                        break;
                    }
                }
            }
            
            trackList.Add(new MIDIChart.Track{ notes = noteList });
        }

        chart.tracks = trackList;

        ctx.AddObjectToAsset("chart", chart);
        ctx.SetMainObject(chart);
    }

}
