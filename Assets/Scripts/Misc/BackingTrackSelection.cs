using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quick hardcoded song selector for the instrument scene
/// </summary>
public class BackingTrackSelection : MonoBehaviour
{
    /// <summary>
    /// The audio source to play backing tracks on
    /// </summary>
    public AudioSource m_audioSource;

    /// <summary>
    /// The audio clip for "Track A"
    /// </summary>
    public AudioClip m_trackA;

    /// <summary>
    /// The audio clip for "Track B"
    /// </summary>
    public AudioClip m_trackB;

    public void PlayTrackA()
    {
        m_audioSource.Stop();
        m_audioSource.clip = m_trackA;
        m_audioSource.Play();
    }

    public void PlayTrackB()
    {
        m_audioSource.Stop();
        m_audioSource.clip = m_trackB;
        m_audioSource.Play();
    }

    public void PlayNothing()
    {
        m_audioSource.Stop();
    }
}
