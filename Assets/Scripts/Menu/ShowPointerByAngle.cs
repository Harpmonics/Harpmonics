using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

public class ShowPointerByAngle : MonoBehaviour
{
    /// <summary>
    /// The pointer renderer to enable/disable
    /// </summary>
    public VRTK_BasePointerRenderer m_renderer;

    /// <summary>
    /// The angle is calculated between the headset forward vector and the vector from
    /// the headset to the reference object
    /// </summary>
    public GameObject m_referenceObject;

    /// <summary>
    /// Maximum angle (degrees) within which the pointer will be shown
    /// </summary>
    public float m_angleLimit = 25.0f;
    
    private Transform m_headsetObject;
    
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
        {
            return;
        }

        m_headsetObject = setup.actualHeadset.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_headsetObject == null)
        {
            Debug.Log("null headset");
            return;
        }
        
        Vector3 headsetForward = m_headsetObject.transform.forward;
        float angle = Vector3.Angle(headsetForward, m_referenceObject.transform.position - m_headsetObject.transform.position);

        if (angle < m_angleLimit)
        {
            m_renderer.enabled = true;
        }
        else
        {
            m_renderer.enabled = false;
        }
    }

}
