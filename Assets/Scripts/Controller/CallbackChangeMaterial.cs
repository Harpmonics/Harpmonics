using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackChangeMaterial : ATouchCallee
{

    [Tooltip("Material to use when controller touches the trigger object.")]
    public Material touchActive;
    [Tooltip("Material to use when the controller stops touching the trigger object.")]
    public Material touchDisabled;

    public override void Callback(GameObject caller, GameObject activator, bool touching)
    {
        Renderer renderer = caller.GetComponent<Renderer>();

        if (touching)
            renderer.material = touchActive;
        else
            renderer.material = touchDisabled;
    }
}
