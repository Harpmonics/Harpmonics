using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_controller : ATouchCallee
{

    ParticleSystem ps;
    bool play = false;

    /*public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        throw new System.NotImplementedException();
    }*/

    void Start ()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	void Update ()
    {
        
    }

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        if (touching && !play)
        {
            play = true;
            ps.Play();
        }
        else
        {
            play = false;
        }
    }
}
