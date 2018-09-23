using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An input manager class that keeps track of which objects provide user input.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Tooltip("Initial controllers to track")]
    public GameObject[] userControllers;

    private static InputManager instance = null;
    
    // Ensure quick access
    private HashSet<GameObject> m_userControllers = new HashSet<GameObject>();

    public void Start()
    {
        if (instance != null)
            throw new System.Exception("There can only be one InputManager instance! Please remove any duplicates.");

        instance = this;

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
    }

    /// <summary>
    /// Adds a controller to be tracked by the InputManager
    /// </summary>
    /// <param name="controller">Controller that has a Collider component</param>
    public static void AddUserController(GameObject controller)
    {
        if (controller.GetComponent<Collider>() == null)
            throw new MissingComponentException("Controller input to InputManager must have a Collider object!");

        instance.m_userControllers.Add(controller);
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
        return instance.m_userControllers.Contains(obj);
    }
}
