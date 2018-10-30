using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreStat : MonoBehaviour
{

    public ScoreboardDisplay scoreboard;

    public static ScoreStat instance;

    private static int m_score;

    public static int Score
    {
        get
        {
            return m_score;
        }
        set
        {
            m_score = value;

            instance.scoreboard.SetCurrentScore(m_score);
        }
    }

    void Start()
    {
        instance = this;
    }

    /*
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
            text.text = lastScore.ToString();
        }
	}
    */

}
