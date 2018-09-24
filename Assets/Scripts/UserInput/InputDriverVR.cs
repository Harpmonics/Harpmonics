using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class InputDriverVR : AInputDriver
{
    public VRTK_ControllerEvents leftControllerEvent;
    public VRTK_ControllerEvents rightControllerEvent;

    protected VRTK_ControllerReference leftController;
    protected VRTK_ControllerReference rightController;

    public override void BindInputs()
    {
        VRTK_SDKSetup setup = VRTK_SDKManager.GetLoadedSDKSetup();

        leftController = VRTK_ControllerReference.GetControllerReference(setup.actualLeftController);
        rightController = VRTK_ControllerReference.GetControllerReference(setup.actualLeftController);

        InputManager.AddUserController(setup.modelAliasLeftController);
        InputManager.AddUserController(setup.modelAliasRightController);

        VRTK_ControllerEvents[] events = new VRTK_ControllerEvents[] { leftControllerEvent, rightControllerEvent };

        foreach(VRTK_ControllerEvents controllerEvents in events)
        {
            controllerEvents.ButtonTwoReleased += OnMenuRelease;
            controllerEvents.ButtonTwoPressed += OnMenuPress;

            controllerEvents.TouchpadPressed += OnTouchpadPress;
            controllerEvents.TouchpadReleased += OnTouchpadRelease;

            controllerEvents.TriggerAxisChanged += OnTriggerChange;
        }
    }

    private GameObject GetInputManagerController(ControllerInteractionEventArgs e)
    {
        if (e.controllerReference == leftController)
            return leftController.model;

        return rightController.model;
    }

    private InputManager.InputState GetInputState(ControllerInteractionEventArgs e)
    {
        return InputManager.GetInputState(GetInputManagerController(e));
    }

    private void OnMenuPress(object sender, ControllerInteractionEventArgs e)
    {
        GetInputState(e).AddButton(InputManager.InputState.ActiveFunction.SequenceRecord);
    }

    private void OnMenuRelease(object sender, ControllerInteractionEventArgs e)
    {
        GetInputState(e).RemoveButton(InputManager.InputState.ActiveFunction.SequenceRecord);
    }

    private void OnTouchpadPress(object sender, ControllerInteractionEventArgs e)
    {
        Vector2 axis = e.touchpadAxis;

        InputManager.InputState.ActiveFunction activeFunction = InputManager.InputState.ActiveFunction.None;

        // Only support two playbacks for now
        if (axis.y < 0.5)
            activeFunction = InputManager.InputState.ActiveFunction.SequencePlayback0;
        else
            activeFunction = InputManager.InputState.ActiveFunction.SequencePlayback1;

        GetInputState(e).AddButton(activeFunction);
    }

    private void OnTouchpadRelease(object sender, ControllerInteractionEventArgs e)
    {
        GetInputState(e).RemoveButton(InputManager.InputState.ActiveFunction.SequencePlaybacks);
    }

    private void OnTriggerChange(object sender, ControllerInteractionEventArgs e)
    {
        GetInputState(e).TriggerActuation = e.buttonPressure;
    }
}
