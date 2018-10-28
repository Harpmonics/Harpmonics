using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Canvas))]
public class ScoreboardDisplay : MonoBehaviour
{
    [Tooltip("Font size of score when playing."), Range(0, 2000f)]
    public float playScale = 1000f;

    [Tooltip("Font size of scoreboard when showing the results."), Range(0, 2000f)]
    public float resultScale = 300f;

    [Tooltip("Position offset of score when playing")]
    public Vector3 scorePosition = Vector3.zero;

    [Tooltip("Object keeping persistent high scores.")]
    public HighScores highScores;

    /// <summary>
    /// Should all UI elements be recreated (for editor preview)?
    /// </summary>
    private bool dirty = false;

    private GameObject scoreboardHeader;
    private GameObject[] scoreboardEntries;
    private GameObject userScore;

    private void AddTextComponent(GameObject obj, Font font, string strText, float scale = -1)
    {
        UnityEngine.UI.Text text = obj.AddComponent<UnityEngine.UI.Text>();

        if (scale < 0)
            scale = resultScale;

        text.font = font;

        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.sizeDelta = new Vector2(scale * 7, scale);

        text.resizeTextMinSize = 10;
        text.resizeTextMaxSize = 1000;
        text.resizeTextForBestFit = true;

        text.text = strText;
    }

    private void SetupUI()
    {
        // Remove old elements
        List<GameObject> oldElements = new List<GameObject>();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            oldElements.Add(this.transform.GetChild(i).gameObject);
        }

        foreach (GameObject obj in oldElements)
        {
            DestroyImmediate(obj);
        }

        Canvas canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        Font fontIndex = Font.CreateDynamicFontFromOSFont("Kaiti", 32);
        Font fontName = Font.CreateDynamicFontFromOSFont("Arial", 32);
        Font fontScore = Font.CreateDynamicFontFromOSFont("Kaiti", 32);

        float leftHorizontalOffset = -resultScale * 5;
        float rightHorizontalOffset = resultScale * 6;
        float verticalSpacing = resultScale * 1.1f;

        float heightStart = resultScale * 5;

        scoreboardHeader = new GameObject("Scoreboard Header");

        scoreboardHeader.transform.parent = this.transform;
        scoreboardHeader.transform.localPosition = new Vector3(0, heightStart, 0);
        scoreboardHeader.transform.localScale = new Vector3(1, 1, 1);


        GameObject headerIndex = new GameObject("Index");
        headerIndex.transform.parent = scoreboardHeader.transform;
        headerIndex.transform.localPosition = new Vector3(leftHorizontalOffset, 0, 0);
        headerIndex.transform.localScale = new Vector3(1, 1, 1);

        AddTextComponent(headerIndex, fontName, "#");


        GameObject headerName = new GameObject("Name");
        headerName.transform.parent = scoreboardHeader.transform;
        headerName.transform.localPosition = new Vector3(0, 0, 0);
        headerName.transform.localScale = new Vector3(1, 1, 1);

        AddTextComponent(headerName, fontName, "Name");


        GameObject headerScore = new GameObject("Score");
        headerScore.transform.parent = scoreboardHeader.transform;
        headerScore.transform.localPosition = new Vector3(rightHorizontalOffset, 0, 0);
        headerScore.transform.localScale = new Vector3(1, 1, 1);

        AddTextComponent(headerScore, fontName, "Score");

        scoreboardEntries = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            GameObject scoreboardEntry = new GameObject("Entry " + i);
            scoreboardEntry.transform.parent = this.transform;
            scoreboardEntry.transform.localPosition = new Vector3(0, heightStart - (i+1) * verticalSpacing, 0);
            scoreboardEntry.transform.localScale = new Vector3(1, 1, 1);

            scoreboardEntries[i] = scoreboardEntry;


            GameObject entryIndex = new GameObject("Index");
            entryIndex.transform.parent = scoreboardEntry.transform;
            entryIndex.transform.localPosition = new Vector3(leftHorizontalOffset, 0, 0);
            entryIndex.transform.localScale = new Vector3(1, 1, 1);

            AddTextComponent(entryIndex, fontIndex, "" + (i+1));


            GameObject entryName = new GameObject("Name");
            entryName.transform.parent = scoreboardEntry.transform;
            entryName.transform.localPosition = new Vector3(0, 0, 0);
            entryName.transform.localScale = new Vector3(1, 1, 1);

            AddTextComponent(entryName, fontName, "A reasonably long name");


            GameObject entryScore = new GameObject("Score");
            entryScore.transform.parent = scoreboardEntry.transform;
            entryScore.transform.localPosition = new Vector3(rightHorizontalOffset, 0, 0);
            entryScore.transform.localScale = new Vector3(1, 1, 1);

            AddTextComponent(entryScore, fontScore, FormatScore((int)Mathf.Pow(10, 5-i)));
        }

        userScore = new GameObject("User Score");
        userScore.transform.parent = this.transform;
        userScore.transform.localScale = new Vector3(1, 1, 1);
        userScore.transform.localPosition = scorePosition;

        AddTextComponent(userScore, fontScore, "0", playScale);
    }

    /// <summary>
    /// Formats the score according to Swedish punctuation (1000 > 1 000)
    /// and adds blankspaces to align all scores
    /// </summary>
    /// <param name="score"></param>
    /// <param name="padding">Pad score with spaces for alignment</param>
    /// <returns></returns>
    protected string FormatScore(int score, bool padding = true)
    {
        string strNumbers = "" + score;

        while(padding && strNumbers.Length < 6)
        {
            // This is a unicode "nut" space (U+2002), since the font was otherwise monospaced with exception of blankspace
            strNumbers = " " + strNumbers;
        }

        string strScore = "" + strNumbers[strNumbers.Length - 1];

        for(int i = 1; i < strNumbers.Length; i++)
        {
            // This is a normal space
            if (i % 3 == 0)
                strScore = " " + strScore;

            strScore = strNumbers[strNumbers.Length-1-i] + strScore;
        }

        return strScore;
    }

    void SetCurrentScore(int score)
    {
        userScore.GetComponent<UnityEngine.UI.Text>().text = FormatScore(score, false);
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            SetupUI();

            for (int i = 0; i < 10; i++)
            {
                HighScores.HighScore highScore = highScores.Scores[i];

                GameObject scoreboardEntry = scoreboardEntries[i];

                GameObject name = scoreboardEntry.transform.Find("Name").gameObject;

                GameObject score = scoreboardEntry.transform.Find("Score").gameObject;

                name.GetComponent<UnityEngine.UI.Text>().text = highScore.Name;

                score.GetComponent<UnityEngine.UI.Text>().text = FormatScore(highScore.Score);
            }
        }
    }

    void Reset()
    {
        dirty = true;
    }

    void OnValidate()
    {
        dirty = true;
    }

    void Update()
    {
        // Rebuilding is only done in the editor
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (dirty)
            {
                dirty = false;
                SetupUI();
            }
        }
#endif
    }
}
