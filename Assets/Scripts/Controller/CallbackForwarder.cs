using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Forwards callbacks received to other objects, purpose: minimize repetitive callback triggers to one manager.
/// </summary>
public class CallbackForwarder : ATouchCallee
{
    [Tooltip("GameObjects that should be receiving input triggers.")]
    public ATouchCallee[] callees;

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        foreach (ATouchCallee callee in callees)
            callee.Callback(caller, activator, touching);
    }
}
