using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AInputDriver : MonoBehaviour
{
    /// <summary>
    /// Bind necessary inputs to the input manager
    /// </summary>
    public abstract void BindInputs();
}
