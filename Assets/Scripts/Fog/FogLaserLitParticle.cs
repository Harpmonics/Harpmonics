using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystemRenderer))]
public class FogLaserLitParticle : MonoBehaviour {

    public GameObject[] laserArrays;
    public LaserBehaviour[] additionalLasers;
    public Material materialNormal;
    public Material materialLit;

    [SerializeField]
    List<Transform> lasers = new List<Transform>();
    Material[] materials;

    void UpdateMaterial(int laserIndex)
    {
        int j = laserIndex + 2;
        if (materials[j] == null) materials[j] = Instantiate(materialLit);
        materials[j].SetVector("LaserPos", lasers[laserIndex].position);
        materials[j].SetVector("LaserDir", lasers[laserIndex].up);
        lasers[laserIndex].hasChanged = false;
    }
        
    void FullUpdate()
    {
        lasers.Clear();
        foreach (GameObject arr in laserArrays)
        {
            foreach (LaserBehaviour laser in arr.GetComponentsInChildren<LaserBehaviour>())
            {

                lasers.Add(laser.transform);
            }
        }
        foreach (LaserBehaviour laser in additionalLasers)
        {
            lasers.Add(laser.transform);
        }

        Debug.Log("Full Update, Laser Count: " + lasers.Count);

        materials = new Material[lasers.Count + 2];
        materials[0] = materialNormal;

        for (int i = 0; i < lasers.Count; ++i)
            UpdateMaterial(i);

        ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
        renderer.materials = materials;
    }

    void OnValidate()
    {
        FullUpdate();
    }

    // Use this for initialization
    void Start()
    {
        FullUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        bool needFullUpdate = false;

        foreach (Transform laser in lasers)
        {
            if (laser == null)
            {
                needFullUpdate = true;
                break;
            }
        }

        if (needFullUpdate)
        {
            FullUpdate();
            return;
        }

        for (int i = 0; i < lasers.Count; ++i)
        {
            if (lasers[i].hasChanged)
            {
                Debug.Log("Laser #" + i + " Changed");
                UpdateMaterial(i);
            }
        }
    }

}
