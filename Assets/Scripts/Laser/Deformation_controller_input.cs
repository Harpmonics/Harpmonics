using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deformation_controller_input : MonoBehaviour
{
    Deformation deform;

    public float force = 10f;
    public float offset = 0.1f;

    void Start ()
    {
        deform = GetComponentInChildren<Deformation>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (InputManager.IsUserInput(other))
        {

            if (deform)
            {
                Debug.Log("hit");
                Vector3 point = GetComponent<Collider>().ClosestPoint(other.transform.position);
                //point += hit.normal * offset;
                // need to extract the normal of the laser surface
                // might be a bit of a hack, create a raycast from the positions of the caller and activator and extract the normal from it
                Ray ray = new Ray(other.transform.position, this.transform.position);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);

                point += hit.normal * offset;
                deform.AddDeformForce(point, force);
            }
        }
    }
}
