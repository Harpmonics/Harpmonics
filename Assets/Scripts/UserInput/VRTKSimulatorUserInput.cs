using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A user input mapper that takes a [VRSimulator_CameraRig] and
/// reports the positions of its LeftHand and RightHand objects.
/// This mapper only exists as an example, it could easily be
/// replaced with the simpler GameObjectUserInput mapper which
/// could be given the two hand GameObjects directly
/// </summary>
public class VRTKSimulatorUserInput : MonoBehaviour, IUserInputSource
{
    public GameObject cameraRig;
    private GameObject VRTK_LeftHand;
    private GameObject VRTK_RightHand;

    public void Start()
    {
        VRTK_LeftHand = cameraRig.transform.Find("LeftHand").gameObject;
        VRTK_RightHand = cameraRig.transform.Find("RightHand").gameObject;
    }

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
        return VRTK_LeftHand.transform.position;
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
        return VRTK_RightHand.transform.position;
    }
}
