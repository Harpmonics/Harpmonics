using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[ExecuteInEditMode]
[RequireComponent(typeof(PlayableDirector))]
public class BeatDirector : MonoBehaviour {

    PlayableDirector director;
    
    void Start ()
    {
        director = GetComponent<PlayableDirector>();
        director.timeUpdateMode = DirectorUpdateMode.Manual;
    }
	
	void Update ()
    {
        if (BeatTime.beat > director.duration)
        {
            if (director.extrapolationMode == DirectorWrapMode.Hold)
                director.time = director.duration;
            else if (director.extrapolationMode == DirectorWrapMode.Loop)
                director.time = BeatTime.beat % director.duration;
            else if (director.extrapolationMode == DirectorWrapMode.None)
                director.time = BeatTime.beat;
        }
        else
        {
            director.time = BeatTime.beat;
        }
	}

}
