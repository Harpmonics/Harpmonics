using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LaserParameterInit : MonoBehaviour {

    public float trackHeight = 2.1f;

    void OnValidate()
    {
        LaserParameters.TrackHeight = trackHeight;
    }

    void Awake()
    {
        LaserParameters.TrackHeight = trackHeight;
    }

}
