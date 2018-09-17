using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look_around : MonoBehaviour
{
	
	void Update ()
	{
		Camera mycam = GetComponent<Camera>();
         transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);
	}
}
