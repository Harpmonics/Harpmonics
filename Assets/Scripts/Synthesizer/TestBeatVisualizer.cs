using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeatVisualizer : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        float subBeat = BeatTime.subBeat;
        transform.localScale = Vector3.one * (BeatTime.beat > 0 ? Mathf.Exp(-subBeat * 10) * 0.5f + 1 : 1);
	}

}
