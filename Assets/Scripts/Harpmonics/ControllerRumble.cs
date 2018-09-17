using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class ControllerRumble : ATouchCallee
{
    [Tooltip("Intensity of the rumble."), Range(0, 1)]
    public float rumbleStrength = 0.1f;

    // Magic variable taken from https://steamcommunity.com/app/358720/discussions/0/405693392914144440/#c357284767229628161
    // Max possible rumble duration
    private static int MAX_DURATION = 3999;

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
        // Since VRTK TriggerHapticPulse does not seem to work, we call the SteamVR interface directly
        foreach (GameObject activator in rumblingControllers)
            SteamVR_Controller.Input((int)activator.GetComponent<SteamVR_RenderModel>().index).TriggerHapticPulse((ushort)(rumbleStrength * MAX_DURATION));
    }
}
