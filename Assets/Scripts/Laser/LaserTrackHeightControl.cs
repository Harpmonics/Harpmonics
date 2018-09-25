using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrackHeightControl : MonoBehaviour {
    
    public Transform leftHandObject;
    public Transform rightHandObject;
    public Transform headsetObject;

    public float headsetFreezeYThreshold = 0.1f;
    public float offsetFromHeadset = 0.5f;
    public float trackFollowDamping = 0.8f;

    Vector3 lastLeftHandPosition;
    Vector3 lastRightHandPosition;
    Vector3 lastHeadsetPosition;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 leftHandPosition = leftHandObject.position;
        Vector3 rightHandPosition = leftHandObject.position;
        Vector3 headsetPosition = headsetObject.position;

        if (Input.GetAxis("SqueezeLeft") > 0.5)
        {
            offsetFromHeadset += leftHandPosition.y - lastLeftHandPosition.y;
        }
        if (Input.GetAxis("SqueezeRight") > 0.5)
        {
            offsetFromHeadset += rightHandPosition.y - lastRightHandPosition.y;
        }

        if (Mathf.Abs(headsetPosition.y - lastHeadsetPosition.y) > headsetFreezeYThreshold)
        {
            lastHeadsetPosition = headsetPosition;
        }

        LaserParameters.TrackHeight += ((lastHeadsetPosition.y + offsetFromHeadset) - LaserParameters.TrackHeight) * Mathf.Exp(-trackFollowDamping * Time.deltaTime);

        lastLeftHandPosition = leftHandPosition;
        lastRightHandPosition = rightHandPosition;
    }

}
