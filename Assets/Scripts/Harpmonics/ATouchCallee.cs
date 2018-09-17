using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATouchCallee : MonoBehaviour
{
    /// <summary>
    /// Callback from when controller touches a game object with callbacks to other game objects.
    /// </summary>
    /// <param name="caller">The object which directly caused this callback to trigger.</param>
    /// <param name="activator">The object that started the chain of callbacks.</param>
    /// <param name="touching">Whether the caller is still being touched.</param>
    public abstract void Callback(GameObject caller, GameObject activator, bool touching);
}
