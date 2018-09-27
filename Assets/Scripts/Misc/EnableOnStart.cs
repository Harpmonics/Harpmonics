using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStart : MonoBehaviour {

    public GameObject[] objects;

	void Start ()
    {
        foreach (var obj in objects)
            obj.SetActive(true);
    }

}
