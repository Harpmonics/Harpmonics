using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTouchDetection : MonoBehaviour
{
    
    [Tooltip("Material to use when controller touches the trigger object.")]
    public Material touchActive;
    [Tooltip("Material to use when the controller stops touching the trigger object.")]
    public Material touchDisabled;

    [Tooltip("GameObject that should be receiving input triggers.")]
    public SynthesizerControl controller;

    [Tooltip("Int handle to pass to callee object."), Range(0, 100)]
    public int handle;

	// Use this for initialization
	void Start ()
    {
	}

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();

        renderer.material = touchActive;

        if (controller != null)
        {
            controller.TriggerWithHandle(handle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();

        renderer.material = touchDisabled;
    }
}
