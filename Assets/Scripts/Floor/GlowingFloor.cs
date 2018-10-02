using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlowingFloor : MonoBehaviour
{
    public float rippleSpeed = 1f;
    public int beatPerRipple = 2;

    public const int maxRippleCount = 10;

    Material material;
    
    float[] ripplePositions = new float[maxRippleCount];
    int nextRippleIndex = 0;
    int lastBeat = -1;

    public void AddRipple(float timeOffset = 0)
    {
        ripplePositions[nextRippleIndex] = timeOffset * rippleSpeed;
        nextRippleIndex = (nextRippleIndex + 1) % maxRippleCount;
    }

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < maxRippleCount; ++i)
            ripplePositions[i] = float.PositiveInfinity;

        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float deltaTime = Time.deltaTime;
        for (int i = 0; i < maxRippleCount; ++i)
            ripplePositions[i] += deltaTime * rippleSpeed;

        int currBeat = BeatTime.numBeat;
        if (currBeat >= 0 && currBeat != lastBeat)
        {
            if (currBeat == lastBeat + 1 && currBeat % beatPerRipple == 0)
                AddRipple((float)(BeatTime.audioTime - BeatTime.timeOnBeat(currBeat)));
            lastBeat = currBeat;
        }

        material.SetFloatArray("_RipplePositions", ripplePositions);
    }

}
