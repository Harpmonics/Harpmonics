using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IUserInputSource))]
public class TriggerController : MonoBehaviour {
    public GameObject leftTouchTrigger;
    public GameObject rightTouchTrigger;

    public float zPosition = 0;

    private IUserInputSource inputSource;

	// Use this for initialization
	void Start () {
        inputSource = GetComponent<IUserInputSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (inputSource.isProvidingLeftTouch())
        {
            var worldPosition = inputSource.getLeftTouchWorldPosition();
            worldPosition.z = zPosition;
            leftTouchTrigger.transform.position = worldPosition;
        }

        if (inputSource.isProvidingRightTouch())
        {
            var worldPosition = inputSource.getRightTouchWorldPosition();
            worldPosition.z = zPosition;
            rightTouchTrigger.transform.position = worldPosition;
        }
    }
}
