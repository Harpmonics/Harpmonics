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
    /// String representing the longest allowed name in the scoreboard (used for preview too).
    /// </summary>
    private const string LONGEST_ALLOWED_NAME = "A reasonably long name";

    /// <summary>
    /// Maximum number of digits to expect in the scoreboard (used for formatting).
    /// </summary>
    private const int MAX_SCORE_DIGITS = 6;

    /// <summary>
    /// Should all UI elements be recreated (for editor preview)?
    /// </summary>
    private bool dirty = false;

    /// <summary>
    /// Scoreboard headers.
    /// </summary>
    private GameObject scoreboardHeader;

    /// <summary>
    /// Scoreboard rows.
    /// </summary>
    private GameObject[] scoreboardEntries;

    /// <summary>
    /// The current user's current score display, before the scoreboard is shown.
    /// </summary>
    private GameObject userScore;

    /// <summary>
    /// The current user's row in the scoreboard, or null if the scoreboard isn't displayed yet.
    /// </summary>
    private GameObject userScoreboardEntry;
    /// <summary>
    /// The current user's high score reference, or null if the scoreboard isn't displayed yet.
    /// </summary>
    private HighScores.HighScore userHighScore;

    /// <summary>
    /// The user's current score
    /// </summary>
    private int currentScore;

    /// <summary>
    /// Is the scoreboard currently being animated in?
    /// </summary>
    private bool isAnimating = false;

    /// <summary>
    /// Is the full scoreboard currently displayed?
    /// </summary>
    public bool IsDisplayingScoreboard { get; private set; } = false;

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

            AddTextComponent(entryName, fontName, LONGEST_ALLOWED_NAME);


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

        while(padding && strNumbers.Length < MAX_SCORE_DIGITS)
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

    /// <summary>
    /// Sets the user's current score to the given value.
    /// </summary>
    /// <param name="score"></param>
    public void SetCurrentScore(int score)
    {
        currentScore = score;
        userScore.GetComponent<UnityEngine.UI.Text>().text = FormatScore(score, false);

        userScore.transform.localScale = Vector3.one * 2f;
    }

    /// <summary>
    /// Sets the user's current name in the scoreboard to the given value.
    /// </summary>
    /// <param name="name"></param>
    public void UpdateUserName(string name)
    {
        if (userScoreboardEntry != null)
        {
            // Don't allow names longer than this
            if (name.Length > LONGEST_ALLOWED_NAME.Length)
                name = name.Substring(0, LONGEST_ALLOWED_NAME.Length);

            UnityEngine.UI.Text text = userScoreboardEntry.transform.Find("Name").gameObject.GetComponent<UnityEngine.UI.Text>();

            text.text = name;

            userHighScore.Name = name;

            highScores.Save();
        }
    }

    /// <summary>
    /// Animate the user's score moving into the scoreboard.
    /// </summary>
    public void StartAnimation()
    {
        isAnimating = true;

        StartCoroutine(AnimationCoroutine());
    }

    private void SetTextAlpha(UnityEngine.UI.Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    private void LerpTextAlpha(UnityEngine.UI.Text text, float target, float lerpFactor)
    {
        // Already reached target
        if ((target - text.color.a) < 10e-3)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, target);
            return;
        }

        float diff = (target - text.color.a) / Mathf.Abs(target - text.color.a);

        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a + diff * lerpFactor, 0, 1));
    }

    IEnumerator AnimationCoroutine()
    {
        // Assumes the current score is the final score for this user
        int userIndex = highScores.Add(currentScore, "Player " + (highScores.Scores.Count + 1));

        for (int i = 0; i < 10; i++)
        {
            GameObject scoreboardEntry = scoreboardEntries[i];

            GameObject index = scoreboardEntry.transform.Find("Index").gameObject;

            GameObject name = scoreboardEntry.transform.Find("Name").gameObject;

            GameObject score = scoreboardEntry.transform.Find("Score").gameObject;

            // If there is no high score, we leave it blank (users will always get a place at this time)
            if (i < highScores.Scores.Count)
            {
                HighScores.HighScore highScore = highScores.Scores[i];

                name.GetComponent<UnityEngine.UI.Text>().text = highScore.Name;

                score.GetComponent<UnityEngine.UI.Text>().text = FormatScore(highScore.Score);
            }
            else
            {
                index.GetComponent<UnityEngine.UI.Text>().text = "";
                name.GetComponent<UnityEngine.UI.Text>().text = "";
                score.GetComponent<UnityEngine.UI.Text>().text = "";
            }
        }

        GameObject userEntry;

        if (userIndex >= scoreboardEntries.Length)
            userEntry = scoreboardEntries[scoreboardEntries.Length - 1];
        else
            userEntry = scoreboardEntries[userIndex];

        userScoreboardEntry = userEntry;

        userHighScore = highScores.Scores[userIndex];

        GameObject scoreboardName = userEntry.transform.Find("Name").gameObject;

        GameObject scoreboardScore = userEntry.transform.Find("Score").gameObject;

        GameObject scoreboardIndex = userEntry.transform.Find("Index").gameObject;

        scoreboardName.GetComponent<UnityEngine.UI.Text>().text = "YOU";
        scoreboardScore.GetComponent<UnityEngine.UI.Text>().text = FormatScore(currentScore);
        scoreboardIndex.GetComponent<UnityEngine.UI.Text>().text = "" + highScores.Scores.Count;

        // We will move the header and user from the bottom to their real positions as we animate
        float headerHeight = scoreboardHeader.transform.localPosition.y;
        float userHeight = userEntry.transform.localPosition.y;

        // Same as in SetupUI
        float verticalSpacing = resultScale * 1.1f;

        scoreboardHeader.transform.localPosition = new Vector3(scoreboardHeader.transform.localPosition.x, scoreboardEntries[scoreboardEntries.Length - 1].transform.localPosition.y + verticalSpacing, scoreboardHeader.transform.localPosition.z);

        userEntry.transform.localPosition = new Vector3(userEntry.transform.localPosition.x, scoreboardEntries[scoreboardEntries.Length - 1].transform.localPosition.y, userEntry.transform.localPosition.z);

        // TODO: Fix hard transition from center-formatting to right-aligned

        UnityEngine.UI.Text userText = userScore.GetComponent<UnityEngine.UI.Text>();

        Vector2 targetSize = scoreboardScore.GetComponent<UnityEngine.UI.Text>().rectTransform.sizeDelta;

        userText.text = FormatScore(currentScore);

        // Shrink large score and move it towards the scoreboard
        while (true)
        {
            // lerpFactor is redefined for every lerp as Time.deltaTime might change between yields
            float lerpFactor = Time.deltaTime * 2;

            userText.rectTransform.sizeDelta += (targetSize - userText.rectTransform.sizeDelta) * lerpFactor;

            Vector3 diff = (scoreboardScore.transform.position - userScore.transform.position);

            userScore.transform.position += diff * lerpFactor;

            if (Mathf.Abs(diff.y) < 10e-4)
                break;

            yield return null;
        }

        userScore.SetActive(false);

        scoreboardHeader.SetActive(true);
        userEntry.SetActive(true);

        // Replace shrunk score with scoreboard score (identical appearance)
        SetTextAlpha(scoreboardScore.GetComponent<UnityEngine.UI.Text>(), 1f);

        float shownIndex = highScores.Scores.Count;

        // Slowly move the user's entry as it stands relative the scoreboard
        while (true)
        {
            float lerpFactor = Time.deltaTime * 2;
            float lerpFactorIndex = Time.deltaTime * 0.5f;

            // Fade in headers and user's entry
            LerpTextAlpha(scoreboardIndex.GetComponent<UnityEngine.UI.Text>(), 1f, lerpFactor);
            LerpTextAlpha(scoreboardName.GetComponent<UnityEngine.UI.Text>(), 1f, lerpFactor);

            foreach (UnityEngine.UI.Text text in scoreboardHeader.GetComponentsInChildren<UnityEngine.UI.Text>())
            {
                LerpTextAlpha(text, 1f, lerpFactor);
            }

            SetTextAlpha(scoreboardIndex.GetComponent<UnityEngine.UI.Text>(), 1f);

            float diffHeader = headerHeight - scoreboardHeader.transform.localPosition.y;

            float mixedLerp = Mathf.Min(diffHeader * 5, 100f) * lerpFactorIndex * 2;

            scoreboardHeader.transform.localPosition += new Vector3(0, mixedLerp, 0);

            // TODO: Sound effect every time the number shown changes?

            float posDiff = userHeight - userEntry.transform.localPosition.y;

            if (shownIndex - 0.5f < 10)
            {
                shownIndex += Mathf.Max((userIndex - shownIndex), -40f) * lerpFactorIndex;

                if (shownIndex < userIndex)
                    shownIndex = userIndex;

                userEntry.transform.localPosition += new Vector3(0, Mathf.Min(posDiff * 5, 100f) * lerpFactorIndex, 0);
            }
            else
            {
                shownIndex += (userIndex - shownIndex) * lerpFactorIndex;
            }

            scoreboardIndex.GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:f0}", shownIndex + 0.5f);

            if (Mathf.Abs(shownIndex - userIndex) < 0.8 && Mathf.Abs(diffHeader) < 10e-1 && Mathf.Abs(posDiff) < 10e-1)
            {
                userEntry.transform.localPosition += new Vector3(0, userHeight - userEntry.transform.localPosition.y, 0);
                break;
            }

            yield return null;
        }

        // Show all other entries now that we know where the user's is
        for (int i2 = 0; i2 < 10; i2++)
        {
            GameObject scoreboardEntry = scoreboardEntries[i2];

            scoreboardEntry.SetActive(true);
        }

        // Flash continuously for 10 000 frames
        for (int i = 0; i < 10000; i++)
        {
            float lerpFactor = Time.deltaTime * 2;

            // Show all other entries now that we know where the user's is
            if (!IsDisplayingScoreboard)
            {
                for (int i2 = 0; i2 < 10; i2++)
                {
                    GameObject scoreboardEntry = scoreboardEntries[i2];

                    if (scoreboardEntry == userEntry)
                        continue;

                    scoreboardEntry.SetActive(true);

                    foreach (UnityEngine.UI.Text text in scoreboardEntry.GetComponentsInChildren<UnityEngine.UI.Text>())
                    {
                        LerpTextAlpha(text, 1f, lerpFactor);

                        if (Mathf.Abs(1 - text.color.a) < 10e-3)
                        {
                            IsDisplayingScoreboard = true;
                        }
                    }
                }
            }

            foreach (UnityEngine.UI.Text text in userEntry.GetComponentsInChildren<UnityEngine.UI.Text>())
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (((Mathf.Sin(Time.time * 5) + 1) * 0.25f + 0.5f) - text.color.a) * lerpFactor * 5);
            }

            yield return null;
        }

        // Set full alpha when stopping the flash
        foreach (UnityEngine.UI.Text text in userEntry.GetComponentsInChildren<UnityEngine.UI.Text>())
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }

        isAnimating = false;
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            SetupUI();

            foreach (UnityEngine.UI.Text text in scoreboardHeader.GetComponentsInChildren<UnityEngine.UI.Text>())
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            }

            scoreboardHeader.SetActive(false);

            foreach (GameObject entry in scoreboardEntries)
            {
                foreach(UnityEngine.UI.Text text in entry.GetComponentsInChildren<UnityEngine.UI.Text>())
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                }

                entry.SetActive(false);
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
        if (Application.isPlaying)
        {
            // Return score to normal size after it has been enlarged by score updates
            userScore.transform.localScale += (Vector3.one - userScore.transform.localScale) * Time.deltaTime * 10;
        }

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
