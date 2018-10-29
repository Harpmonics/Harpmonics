using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class Feedback : MonoBehaviour {

    static public string fb { get; set; }
	static public float alpha { get; set; }

	
    TextMesh text;
	// Use this for initialization
	void Start () {
        text = GetComponent<TextMesh>();
		text.fontSize = 30;
		Color test = text.color;
		alpha = 1;
		test.a = alpha;
		
		
    }
	
	// Update is called once per frame
    void Update()
    {
        text.text = fb;
        Color test = text.color;
        test.a = alpha;
        text.color = test;

        if (alpha > 0)
        {
            alpha -= Time.deltaTime;
            test.a = alpha;
            text.color = test;
        }
    }


}
