using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IUserInputSource))]
public class TriggerController : MonoBehaviour {
    public GameObject leftTouchTrigger;
    public GameObject rightTouchTrigger;

    private IUserInputSource inputSource;

	// Use this for initialization
	void Start () {
        inputSource = GetComponent<IUserInputSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (inputSource.isProvidingLeftTouch())
        {
            leftTouchTrigger.transform.position = inputSource.getLeftTouchWorldPosition();
        }

        if (inputSource.isProvidingRightTouch())
        {
            rightTouchTrigger.transform.position = inputSource.getRightTouchWorldPosition();
        }
    }
}
