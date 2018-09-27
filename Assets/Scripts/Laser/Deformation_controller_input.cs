using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformation_controller_input : ATouchCallee
{

    public float force = 10f;
    public float offset = 0.1f;

    void Update ()
    {
		
	}

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        if (touching)
        {
            Deformation deform = caller.GetComponent<Deformation>();

            if (deform)
            {
                Vector3 point = caller.GetComponent<Collider>().ClosestPoint(activator.transform.position);
                //point += hit.normal * offset;
                // need to extract the normal of the laser surface
                // might be a bit of a hack, create a raycast from the positions of the caller and activator and extract the normal from it
                Ray ray = new Ray(activator.transform.position, caller.transform.position);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                point += hit.normal * offset;
                deform.AddDeformForce(point, force);
            }
        }
    }
}
