using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A user input mapper that simply reports the positions of two given GameObjects
/// </summary>
public class GameObjectUserInput : MonoBehaviour, IUserInputSource
{
    public GameObject leftHand;
    public GameObject rightHand;

    /// <returns>
    /// True if the input source is currently providing position data for the left "touch" trigger, false otherwise
    /// </returns>
    public bool isProvidingLeftTouch()
    {
        return true;
    }

    /// <returns>
    /// World position of left "touch" trigger. If the input source is not providing this data, returns the origin (0,0,0)
    /// </returns>
    public Vector3 getLeftTouchWorldPosition()
    {
        return leftHand.transform.position;
    }

    /// <returns>
    /// True if the input source is currently providing position data for the right "touch" trigger, false otherwise
    /// </returns>
    public bool isProvidingRightTouch()
    {
        return true;
    }

    /// <returns>
    /// World position of right "touch" trigger. If the input source is not providing this data, returns the origin (0,0,0)
    /// </returns>
    public Vector3 getRightTouchWorldPosition()
    {
        return rightHand.transform.position;
    }
}
