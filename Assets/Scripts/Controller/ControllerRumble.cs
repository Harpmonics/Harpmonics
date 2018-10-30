using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class ControllerRumble : MonoBehaviour
{
    [Tooltip("Intensity of the rumble."), Range(0, 1)]
    public float rumbleStrength = 0.1f;

    [Tooltip("Rumble time in seconds.")]
    public float rumbleTime = 0.1f;

    // Magic variable taken from https://steamcommunity.com/app/358720/discussions/0/405693392914144440/#c357284767229628161
    // Max possible rumble duration
    private static int MAX_DURATION = 3999;

    public void StartRumble(Collider controllerCollider, float rumbleTime = -1f)
    {
        if (!InputManager.IsUserInput(controllerCollider))
            return;

        if (rumbleTime < 0)
            rumbleTime = this.rumbleTime;

        StartCoroutine(RumbleCoroutine(controllerCollider.gameObject, rumbleTime));
    }

    IEnumerator RumbleCoroutine(GameObject controller, float rumbleTime)
    {
        float remainingTime = rumbleTime;

        SteamVR_RenderModel model = GetComponent<SteamVR_RenderModel>();

        // Simulator, can't rumble
        if (model == null)
            yield break;

        while (remainingTime > 0)
        {
            // Since VRTK TriggerHapticPulse does not seem to work, we call the SteamVR interface directly
            SteamVR_Controller.Input((int)model.index).TriggerHapticPulse((ushort)(rumbleStrength * MAX_DURATION));

            remainingTime -= Time.deltaTime;

            yield return null;
        }
    }
}
