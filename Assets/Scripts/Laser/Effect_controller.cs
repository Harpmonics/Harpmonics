using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Effect_controller : MonoBehaviour
{
    [Tooltip("Object with attached particles to control.")]
    public GameObject particleObject;

    ParticleSystem ps;

    GameObject laserObj, touchObj;

    void Start()
    {
        ps = particleObject.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	void Update()
    {
        if (ps.isPlaying)
        {
            // Playback objects may be destroyed while still touching the laser
            if (!touchObj.activeSelf)
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
            laserObj = this.gameObject;
            touchObj = other.gameObject;

            ps.Play();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (InputManager.IsUserInput(other))
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
