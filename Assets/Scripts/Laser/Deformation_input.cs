using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformation_input : MonoBehaviour
{
	public float force = 10f;
    public float offset = 0.1f;
	
	void Update ()
	{
        if (Input.GetMouseButton(0))
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
                // this below is the important part
				Deformation deform = hit.collider.GetComponent<Deformation>();
				if (deform)
				{
					Vector3 point = hit.point;
                    point += hit.normal * offset;
					deform.AddDeformForce(point, force);
				}
			}
		}
	}
}
