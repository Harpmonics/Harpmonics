using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchWobble_alt : MonoBehaviour
{
    public Transform model;
    public float wobbleMagnitude = 1.0f;
    public float wobbleDuration = 1.0f;
    private float wobbleStopTime;
    private Vector3 initialScale;
    private float phase = 0.0f;
    public float wobbleSpeed = 0.05f;

    // Use this for initialization
    void Start()
    {
        if (model == null)
        {
            model = transform;
        }
        initialScale = model.localScale;
        wobbleStopTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (wobbleStopTime > Time.time)
        {
            float remainingTime = Time.time - wobbleStopTime;
            Vector3 ls = initialScale;
            float magn = wobbleMagnitude * (remainingTime / wobbleDuration);
            model.localScale = new Vector3(ls.x + magn * Mathf.Sin(phase), ls.y, ls.z + magn * Mathf.Cos(phase));
        }
        else
        {
            model.localScale = initialScale;
        }

        phase += wobbleSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TouchTrigger"))
        {
            phase = 0.0f;
            wobbleStopTime = Time.time + wobbleDuration;
        }
    }
}
