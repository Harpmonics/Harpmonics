using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_controller : ATouchCallee
{

    ParticleSystem ps;

    GameObject laserObj, touchObj;

    void Start ()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	void Update ()
    {
        if (ps.isPlaying)
        {
            ps.transform.position = laserObj.GetComponent<Collider>().ClosestPoint(touchObj.transform.position);
        }
    }

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        if (touching)
        {
            laserObj = caller;
            touchObj = activator;

            ps.Play();
        }
        else
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
