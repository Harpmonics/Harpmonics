using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class Feedback : MonoBehaviour {

    static public string fb { get; set; }

    TextMesh text;

	// Use this for initialization
	void Start () {
        text = GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
        text.text = fb;
	}

}
