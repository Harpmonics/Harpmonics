using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBeatProvider {

    /// <summary>
    /// Returns the audio time as the ruler. Future beat calls will be based on this time value.
    /// </summary>
    /// <returns>The audio time</returns>
    double GetAudioTime();

    /// <summary>
    /// Returns the coherent beat value (a.k.a. beat, ask Tianli if not clear) on specified audio time;
    /// </summary>
    /// <returns>beat value</returns>
    float GetBeatFromTime(double audioTime);
    /// <summary>
    /// Returns audio time on specified beat;
    /// </summary>
    /// <returns>audio time</returns>
    double GetTimeFromBeat(float beat);

    // Suggestions for speed-up interfaces
    // float GetCurrentBeat();
    // float GetDeltaBeat(double audioTime);

}
