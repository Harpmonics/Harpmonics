using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUserInput : MonoBehaviour, IUserInputSource {
    public Camera cam;

    private bool isTracking = false;
    private int whichButton = -1;

    public void Update()
    {
        if (!isTracking && Input.GetMouseButtonDown(0))
        {
            isTracking = true;
            whichButton = 0;
        }
        else if (isTracking && whichButton == 0 && Input.GetMouseButtonUp(0))
        {
            isTracking = false;
            whichButton = -1;
        }
        else if (!isTracking && Input.GetMouseButtonDown(1))
        {
            isTracking = true;
            whichButton = 1;
        }
        else if (isTracking && whichButton == 1 && Input.GetMouseButtonUp(1))
        {
            isTracking = false;
            whichButton = -1;
        }
    }

    // IUserInputSource implementation

    public bool isProvidingLeftTouch()
    {
        return isTracking && whichButton == 0;
    }

    public Vector3 getLeftTouchWorldPosition()
    {
        if (isProvidingLeftTouch())
        {
            Vector3 reqPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z);
            return cam.ScreenToWorldPoint(reqPos);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public bool isProvidingRightTouch()
    {
        return isTracking && whichButton == 1;
    }

    public Vector3 getRightTouchWorldPosition()
    {
        if (isProvidingRightTouch())
        {
            Vector3 reqPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z);
            return cam.ScreenToWorldPoint(reqPos);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
}
