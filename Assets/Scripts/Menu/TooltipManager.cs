using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TooltipManager : MonoBehaviour
{

    private VRTK_ControllerTooltips tooltips;

    void Start()
    {
        InputManager.AddCallback(OnControllerCallback);

        tooltips = GetComponent<VRTK_ControllerTooltips>();

        tooltips.UpdateText(VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip, "Record a motion sequence");
    }

    private void OnControllerCallback(InputManager.InputState inputState)
    {
        if (inputState.Buttons.HasFlag(InputManager.InputState.ActiveFunction.SequenceRecord))
        {
            tooltips.UpdateText(VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip, "");
            tooltips.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, "Save sequence into top or bottom slot");
        }

        if (tooltips.buttonTwoText.Length == 0 && (inputState.Buttons & InputManager.InputState.ActiveFunction.SequencePlaybacks) > 0)
        {
            tooltips.UpdateText(VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip, "Record a motion sequence");
            tooltips.UpdateText(VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip, "Replay sequence saved into top or bottom slot");
        }
    }
}
