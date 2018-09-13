using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTouchDetection : MonoBehaviour
{

    /// <summary>
    /// Material to use when controller touches the trigger object.
    /// </summary>
    public Material touchActive;
    /// <summary>
    /// Material to use when the controller stops touching the trigger object.
    /// </summary>
    public Material touchDisabled;

    private GameObject m_triggerObj;

	// Use this for initialization
	void Start ()
    {
        m_triggerObj = this.gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();

        renderer.material = touchActive;
    }

    private void OnTriggerExit(Collider other)
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();

        renderer.material = touchDisabled;
    }
}
