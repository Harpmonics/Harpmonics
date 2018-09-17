using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_script : MonoBehaviour
{

	LineRenderer laser;
	Ray laserRay;
	ParticleSystem particle;
	Vector3 beam_base;
	bool play = false;

	void Start ()
	{
		laser = gameObject.GetComponent<LineRenderer>();
		laser.enabled = true;

		laserRay = new Ray(transform.position, transform.forward);
		laser.SetPosition(0, laserRay.origin);
		laser.SetPosition(1, laserRay.GetPoint(100));

		particle = gameObject.GetComponent<ParticleSystem>();
		beam_base = particle.transform.position;
		particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}
	
	void Update ()
	{
		/*Renderer rend;
		rend = gameObject.GetComponent<Renderer>();
		rend.material.mainTextureOffset = new Vector2(0, Time.time);*/

		RaycastHit hit;
		if (Physics.Raycast(laserRay, out hit, 100))
		{
			laser.SetPosition(1, hit.point);
			particle.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
			if(!play)
			{
				play = true;
				particle.Play(true);
			}
			
		}
		else
		{
			laser.SetPosition(1, laserRay.GetPoint(100));
			particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			particle.transform.position = beam_base;
			play = false;
		}
	}
}
