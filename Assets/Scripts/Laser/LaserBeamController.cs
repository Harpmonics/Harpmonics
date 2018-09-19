using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamController : MonoBehaviour {
    public AudioClip laserSound;
    private AudioSource audioSource;
    public SequencerControl sequencer;
    public int handleNumber;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("TouchTrigger")) // just as exploration of how we could detect different types of triggers
            return;
        if (audioSource != null && laserSound != null) 
        {
            audioSource.Stop();
            audioSource.clip = laserSound;
            audioSource.Play();
        }
        if (sequencer != null)
        {
            // Handled by ControllerTouchDetection
            //sequencer.TriggerWithHandle(handleNumber);
        }
    }
}
