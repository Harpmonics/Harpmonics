using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LaserArrayAssigner : MonoBehaviour
{
    public LaserBehaviour prefabLaser;
    public MIDIChart chart;

    public int trackIndex;

    public int laserCount = 12;
    public int laserBeginNoteNumber = 0;
    public int laserEndNoteNumber = 127;

    void DestroyLaser(LaserBehaviour laser)
    {
        /*if (Application.isPlaying)
            Destroy(laser.gameObject);
        else*/
            DestroyImmediate(laser.gameObject);
    }

    void SetupLasers()
    {
        var childLasers = GetComponentsInChildren<LaserBehaviour>();
        foreach (var laser in childLasers)
            DestroyLaser(laser);

        if (prefabLaser == null)
            return;
        
        for (int i = 0; i < laserCount; ++i)
        {
            var laser = Instantiate(prefabLaser, transform);
            laser.chart = chart;
            laser.trackIndex = trackIndex;
            laser.laserIndex = i;
            var assignedPitches = new List<int>();
            for (int j = i; j <= laserEndNoteNumber; j += laserCount)
                assignedPitches.Add(j);
            laser.assignedPitches = assignedPitches.ToArray();
        }

    }

    void Start()
    {
        if (!Application.isPlaying)
            SetupLasers();
    }

    bool needRebuild = false;
    void OnValidate()
    {
        if (!Application.isPlaying)
            needRebuild = true;
    }

    void Update()
    {
        if (needRebuild)
        {
            needRebuild = false;
            SetupLasers();
        }
    }

}
