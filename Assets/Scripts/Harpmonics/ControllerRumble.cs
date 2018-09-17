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

    protected bool isRumbling = false;

    protected List<GameObject> rumblingControllers = new List<GameObject>();

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        isRumbling = touching;

        if (touching)
            rumblingControllers.Add(activator);
        else
            rumblingControllers.Remove(activator);
    }

    public void Update()
    {
        foreach (GameObject activator in rumblingControllers)
            SteamVR_Controller.Input((int)activator.GetComponent<SteamVR_RenderModel>().index).TriggerHapticPulse(500);
    }
}
