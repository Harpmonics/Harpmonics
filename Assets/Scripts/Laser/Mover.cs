using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{

	
	void Update ()
	{
		if(Input.GetKey("right"))
		{
			transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
		}
		else if(Input.GetKey("left"))
		{
			transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
		}
		else if(Input.GetKey("up"))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		}
		else if(Input.GetKey("down"))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
		}
	}
}
