using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserInputSource {
    /// <returns>
    /// True if the input source is currently providing position data for the left "touch" trigger, false otherwise
    /// </returns>
    bool isProvidingLeftTouch();

    /// <returns>
    /// World position of left "touch" trigger. If the input source is not providing this data, returns the origin (0,0,0)
    /// </returns>
    Vector3 getLeftTouchWorldPosition();

    /// <returns>
    /// True if the input source is currently providing position data for the right "touch" trigger, false otherwise
    /// </returns>
    bool isProvidingRightTouch();

    /// <returns>
    /// World position of right "touch" trigger. If the input source is not providing this data, returns the origin (0,0,0)
    /// </returns>
    Vector3 getRightTouchWorldPosition();
}
