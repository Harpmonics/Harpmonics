using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystemRenderer))]
public class FogLaserLitParticle : MonoBehaviour {

    public GameObject[] laserArrays;
    public LaserBehaviour[] additionalLasers;
    public Material material;

    public List<Transform> lasers;

    // Use this for initialization
    void Start ()
    {
        lasers = new List<Transform>();
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

        Material[] materials = new Material[lasers.Count + 1];
        for (int i = 0; i < lasers.Count; ++i)
        {
            int j = (i == 0 ? i : i + 1);
            materials[j] = Instantiate(material);
            materials[j].SetVector("LaserPos", lasers[i].position);
            materials[j].SetVector("LaserDir", lasers[i].up);
        }

        ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
        renderer.materials = materials;
    }

    void OnValidate()
    {
        Start();
    }

    // Update is called once per frame
    void Update () {
		
	}

}
