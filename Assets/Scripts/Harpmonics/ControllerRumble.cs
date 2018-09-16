using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class ControllerRumble : ATouchCallee
{
    [Tooltip("Intensity of the rumble."), Range(0, 1)]
    public float rumbleStrength = 0.1f;
    [Tooltip("How long to rumble for."), Range(0, 5)]
    public float rumbleDuration = 0.1f;
    [Tooltip("Interval to wait before triggering next rumble in succession."), Range(0, 10)]
    public float rumbleInterval = 0.1f;

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        if (touching)
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(activator), rumbleStrength, rumbleDuration, rumbleInterval);
    }
}
