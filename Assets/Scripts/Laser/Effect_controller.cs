﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Effect_controller : MonoBehaviour
{
    [Tooltip("Object with attached particles to control.")]
    public GameObject particleObject;

    ParticleSystem ps;

    GameObject laserObj, touchObj;

    // How many seconds is too long to hold the laser for?
    public float holdTolerance = 3.0f;

    private float startHoldTime = 0f;
    private bool isHolding = false;

    void Start()
    {
        if (particleObject == null) particleObject = gameObject;
        ps = particleObject.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	void Update()
    {
        if (ps.isPlaying)
        {
            // Playback objects may be destroyed while still touching the laser
            if (touchObj == null || (isHolding && (startHoldTime + holdTolerance - Time.time) < 0))
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                return;
            }

            ps.transform.position = laserObj.GetComponent<Collider>().ClosestPoint(touchObj.transform.position);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (InputManager.IsUserInput(other))
        {
            isHolding = true;

            startHoldTime = Time.time;

            laserObj = this.gameObject;
            touchObj = other.gameObject;

            ps.Play();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (InputManager.IsUserInput(other))
        {
            isHolding = false;

            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
