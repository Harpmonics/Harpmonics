using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An input manager class that keeps track of which objects provide user input.
/// </summary>
public class InputManager : MonoBehaviour
{
    // TODO: Refactor

    public delegate void InputCallbackDelegate(InputState inputState);
    
    // TODO: One callback for each function?
    private static InputCallbackDelegate callbacks;

    public class InputState
    {
        // TODO: Put enum definition outside the class to avoid long names like InputManager.InputState.ActiveFunction.SequenceRecord

        /// <summary>
        /// Flags representing functions that can be activated by e.g. a button.
        /// </summary>
        [System.Flags]
        public enum ActiveFunction : int
        {
            None                = 0,
            SequenceRecord      = 1 << 0,
            SequencePlayback0   = 1 << 1,
            SequencePlayback1   = 1 << 2,
            SequencePlayback2   = 1 << 3,
            SequencePlayback3   = 1 << 4,

            SequencePlaybacks   = SequencePlayback0 | SequencePlayback1 | SequencePlayback2 | SequencePlayback3,
        }

        /// <summary>
        /// Currently pressed buttons.
        /// </summary>
        public ActiveFunction Buttons { get; protected set; } = ActiveFunction.None;

        /// <summary>
        /// The controller associated with this input state.
        /// </summary>
        public GameObject Controller { get; private set; } = null;

        public InputState(GameObject controller)
        {
            Controller = controller;
        }

        public void AddButton(ActiveFunction flag)
        {
            Buttons |= flag;

            callbacks.Invoke(this);
        }

        public void RemoveButton(ActiveFunction flag)
        {
            Buttons &= ~flag;
        }

        /// <summary>
        /// Current trigger actuation, between 0 and 1
        /// </summary>
        public float TriggerActuation { get; set; } = 0f;
    }

    public static void AddCallback(InputCallbackDelegate callbackDelegate)
    {
        callbacks += callbackDelegate;
    }


    [Tooltip("Initial controllers to track")]
    public GameObject[] userControllers;

    [Tooltip("Drivers that trigger input events")]
    public AInputDriver[] inputDrivers;

    private static InputManager instance = null;
    
    private Dictionary<GameObject, InputState> m_userControllers = new Dictionary<GameObject, InputState>();

    public void Awake()
    {
        if (instance != null)
            throw new System.Exception("There can only be one InputManager instance! Please remove any duplicates.");

        instance = this;
    }

    public void Start()
    {
        foreach (GameObject controller in userControllers)
        {
            try
            {
                AddUserController(controller);
            }
            catch(MissingComponentException e)
            {
                Debug.LogError(string.Format("{0} does not have a Collider component, will not be used to track input!", controller));
            }
        }

        foreach(AInputDriver driver in inputDrivers)
        {
            driver.BindInputs();
        }
    }

    /// <summary>
    /// Adds a controller to be tracked by the InputManager
    /// </summary>
    /// <param name="controller">Controller that has a Collider component</param>
    public static void AddUserController(GameObject controller)
    {
        if (controller.GetComponent<Collider>() == null)
            throw new MissingComponentException("Controller input to InputManager must have a Collider object!");

        instance.m_userControllers.Add(controller, new InputState(controller));
    }

    public static void RemoveUserController(GameObject controller)
    {
        instance.m_userControllers.Remove(controller);
    }

    // TODO: Provide delegates that e.g. trigger when a button is clicked on the controller.

    /// <summary>
    /// Check whether the specified Collider is connected to a registered GameObject input source.
    /// </summary>
    /// <param name="collider">Collider to check user input status of</param>
    /// <returns>true if the Collider's GameObject is registered as an user input source, or false otherwise</returns>
    public static bool IsUserInput(Collider collider)
    {
        return IsUserInput(collider.gameObject);
    }

    /// <summary>
    /// Check whether the specified GameObject is registered as an user input source.
    /// </summary>
    /// <param name="obj">GameObject to check user input status of.</param>
    /// <returns>true if the GameObject is registered as an user input source, or false otherwise</returns>
    public static bool IsUserInput(GameObject obj)
    {
        return instance.m_userControllers.ContainsKey(obj);
    }

    /// <summary>
    /// Gets the input state of the controller connected to this collider.
    /// </summary>
    /// <param name="obj">The collider connected to a controller with an input state</param>
    /// <returns></returns>
    public static InputState GetInputState(Collider collider)
    {
        return GetInputState(collider.gameObject);
    }

    /// <summary>
    /// Gets the input state of the given controller
    /// </summary>
    /// <param name="obj">The controller to check input state of</param>
    /// <returns></returns>
    public static InputState GetInputState(GameObject obj)
    {
        InputState state = null;

        instance.m_userControllers.TryGetValue(obj, out state);

        if (state == null)
            throw new System.Exception("Specified GameObject is not registered as a controller!");

        return state;
    }
}
