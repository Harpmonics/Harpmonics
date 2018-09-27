﻿using Melanchall.DryWetMidi.Smf.Interaction;

namespace Melanchall.DryWetMidi.Tools
{
    /// <summary>
    /// Settings according to which notes should be randomized.
    /// </summary>
    public sealed class NotesRandomizingSettings : LengthedObjectsRandomizingSettings
    {
    }

    /// <summary>
    /// Provides methods to randomize notes time.
    /// </summary>
    public sealed class NotesRandomizer : LengthedObjectsRandomizer<Note, NotesRandomizingSettings>
    {
    }
}
