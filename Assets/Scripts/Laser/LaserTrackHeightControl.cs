using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class LaserTrackHeightControl : MonoBehaviour
{
    public float headsetFreezeYThreshold = 0.1f;
    public float headsetFreezeTime = 1f;
    public float offsetFromHeadset = 0.5f;
    public float trackFollowDamping = 0.8f;

    private Transform leftHandObject;
    private Transform rightHandObject;
    private Transform headsetObject;
    
    Vector3 lastLeftHandPosition;
    Vector3 lastRightHandPosition;
    Vector3 lastHeadsetPosition;

    float timeHeadsetMoved = 0;

    // Use this for initialization
    void Start()
    {
        VRTK_SDKManager.SubscribeLoadedSetupChanged(SDK_Loaded);
    }

    private void SDK_Loaded(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
    {
        VRTK_SDKSetup setup = e.currentSetup;

        // When switching scenes, we can get a non-existent SDK for some reason
        if (setup == null)
            return;

        leftHandObject = setup.actualLeftController.transform;
        rightHandObject = setup.actualRightController.transform;

        headsetObject = setup.actualHeadset.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandObject == null)
            return;

        Vector3 leftHandPosition = leftHandObject.position;
        Vector3 rightHandPosition = rightHandObject.position;
        Vector3 headsetPosition = headsetObject.position;

        if (Input.GetAxis("SqueezeLeft") > 0.5)
        {
            offsetFromHeadset += leftHandPosition.y - lastLeftHandPosition.y;
        }
        if (Input.GetAxis("SqueezeRight") > 0.5)
        {
            offsetFromHeadset += rightHandPosition.y - lastRightHandPosition.y;
        }

        if (Mathf.Abs(headsetPosition.y - lastHeadsetPosition.y) > headsetFreezeYThreshold * Mathf.Min(1, (Time.time - timeHeadsetMoved) / headsetFreezeTime))
        {
            lastHeadsetPosition = headsetPosition;
            timeHeadsetMoved = Time.time;
        }
        
        if (SteamVR.active)
            LaserParameters.TrackHeight += ((lastHeadsetPosition.y + offsetFromHeadset) - LaserParameters.TrackHeight) * Mathf.Exp(-trackFollowDamping * Time.deltaTime);

        lastLeftHandPosition = leftHandPosition;
        lastRightHandPosition = rightHandPosition;
    }

}
