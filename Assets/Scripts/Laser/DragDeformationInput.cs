using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DragDeformation))]
public class DragDeformationInput : MonoBehaviour
{

    DragDeformation deform;
    Vector3 point, oldPoint;
    GameObject dragger;
    bool grabbed = false;

	void Start ()
    {
        deform = GetComponent<DragDeformation>();
        oldPoint = new Vector3(0f,0f,0f);
	}

    void Update ()
    {
        if (grabbed)
        {
            // (Input.GetButton("SqueezeRight") || Input.GetButton("SqueezeLeft"))
            if (!(point == oldPoint) && Input.GetButton("Fire1"))
            {
                deform.Deform(point);
                oldPoint = point;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                grabbed = false;
                //dragger = null;
            }
            point = dragger.transform.position;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (InputManager.IsUserInput(other))
        {
            if (deform)
            {
                point = GetComponent<Collider>().ClosestPoint(other.transform.position);
                dragger = other.gameObject;
                grabbed = true;
            }
        }
    }
}
