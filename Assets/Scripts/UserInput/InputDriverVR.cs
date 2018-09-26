using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class InputDriverVR : AInputDriver
{
    public VRTK_ControllerEvents leftControllerEvent;
    public VRTK_ControllerEvents rightControllerEvent;

    protected GameObject leftController;
    protected GameObject rightController;

    /// <summary>
    /// Used to trigger the menu button only when both controllers are gripped.
    /// </summary>
    protected int numControllersGripping = 0;

    [Header("Configurable options")]

    /// <summary>
    /// Time to wait (in seconds) before considering a grip hold to be a call to return to the menu.
    /// </summary>
    [Tooltip("Time to wait (in seconds) before considering a grip hold to be a call to return to the menu"), Range(0f, 10f)]
    public float menuGrippingTime = 2.0f;

    /// <summary>
    /// Coroutine checking whether the above time has passed since two grip buttons were pressed
    /// </summary>
    protected Coroutine cWaitForGrip = null;

    public override void BindInputs()
    {
        // SteamVR isn't loaded by the time this method is called, so we use this callback instead
        VRTK_SDKManager.SubscribeLoadedSetupChanged(SDK_Loaded);
    }

    private void SDK_Loaded(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
    {
        VRTK_SDKSetup setup = e.currentSetup;

        // TODO: Obtain actual controller references somehow (VRTK_ControllerReference.GetControllerReference does not get the actual reference)
        leftController = setup.modelAliasLeftController;
        rightController = setup.modelAliasRightController;

        InputManager.AddUserController(setup.modelAliasLeftController);
        InputManager.AddUserController(setup.modelAliasRightController);

        VRTK_ControllerEvents[] events = new VRTK_ControllerEvents[] { leftControllerEvent, rightControllerEvent };

        foreach(VRTK_ControllerEvents controllerEvents in events)
        {
            controllerEvents.ButtonTwoReleased += OnMenuRelease;
            controllerEvents.ButtonTwoPressed += OnMenuPress;

            controllerEvents.TouchpadPressed += OnTouchpadPress;
            controllerEvents.TouchpadReleased += OnTouchpadRelease;

            controllerEvents.GripClicked += OnGripPress;
            controllerEvents.GripUnclicked += OnGripRelease;

            controllerEvents.TriggerAxisChanged += OnTriggerChange;
        }
    }

    private GameObject GetInputManagerController(ControllerInteractionEventArgs e)
    {
        // Could technically use controllerReference.model directly, but we may refer something else in the future
        if (e.controllerReference.model == leftController)
            return leftController;

        return rightController;
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

    private IEnumerator WaitForGrip(ControllerInteractionEventArgs e)
    {
        float timeStart = Time.time;

        while(numControllersGripping >= 2)
        {
            float diff = Time.time - timeStart;

            if (diff >= menuGrippingTime)
            {
                GetInputState(e).AddButton(InputManager.InputState.ActiveFunction.OpenMenu);
                GetInputState(e).RemoveButton(InputManager.InputState.ActiveFunction.OpenMenu);
                break;
            }

            if ((menuGrippingTime - diff) > 0.1f)
                yield return new WaitForSecondsRealtime(menuGrippingTime - diff - 0.1f);
            else
                yield return null;
        }

        cWaitForGrip = null;
    }

    private void OnGripPress(object sender, ControllerInteractionEventArgs e)
    {
        numControllersGripping++;

        if (numControllersGripping >= 2)
        {
            cWaitForGrip = StartCoroutine(WaitForGrip(e));
        }
    }

    private void OnGripRelease(object sender, ControllerInteractionEventArgs e)
    {
        numControllersGripping--;

        if (cWaitForGrip != null)
        {
            StopCoroutine(cWaitForGrip);
            cWaitForGrip = null;
        }
    }
}
