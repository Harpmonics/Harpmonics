using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTouchDetection : MonoBehaviour
{
    [Tooltip("GameObjects that should be receiving input triggers.")]
    public ATouchCallee[] callees;

    private void OnTriggerEnter(Collider other)
    {
        foreach(ATouchCallee callee in callees)
            callee.Callback(this.gameObject, other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (ATouchCallee callee in callees)
            callee.Callback(this.gameObject, other.gameObject, false);
    }
}
