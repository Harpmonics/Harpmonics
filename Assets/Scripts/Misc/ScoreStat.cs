using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class ScoreStat : MonoBehaviour {

    static public int Score { get; set; }

    int lastScore;
    TextMesh text;

	// Use this for initialization
	void Start () {
        Score = 0;
        lastScore = -1;
        text = GetComponent<TextMesh>();
    }
	
	// Update is called once per frame
	void Update () {
		if (lastScore != Score)
        {
            lastScore = Score;
            text.text = lastScore.ToString() + "00";
        }
	}

}
