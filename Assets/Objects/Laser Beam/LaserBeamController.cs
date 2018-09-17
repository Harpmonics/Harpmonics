using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LaserBeamController : MonoBehaviour {
    public AudioClip laserSound;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TouchTrigger")) // just as exploration of how we could detect different types of triggers
        {
            audioSource.Stop();
            audioSource.clip = laserSound;
            audioSource.Play();
        }
    }
}
