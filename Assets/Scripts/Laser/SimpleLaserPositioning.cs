using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaserBehaviour))]
[ExecuteInEditMode]
public class SimpleLaserPositioning : MonoBehaviour
{
    LaserBehaviour laserData;
    LaserBehaviour LaserData { get { return laserData ? laserData : laserData = GetComponent<LaserBehaviour>(); } }

    public Vector3 firstPosition;
    public Vector3 offsetPositionPerLaser;

    void ResetPosition ()
    {
        transform.localPosition = firstPosition + offsetPositionPerLaser * LaserData.laserIndex;
	}

    void Start()
    {
        ResetPosition();
    }

    void OnValidate()
    {
        ResetPosition();
    }

}
