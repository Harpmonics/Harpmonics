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
            if (!(point == oldPoint))
            {
                deform.Deform(point);
                oldPoint = point;
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
