using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDisplayer : MonoBehaviour
{
    [Tooltip("Laser harp to hide when showing the results.")]
    public GameObject laserHarp;

    [Tooltip("Scoreboard to show in results.")]
    public ScoreboardDisplay scoreboard;

    [Tooltip("Accuracy graph to show in results.")]
    public AccuracyGraph graph;

    [Tooltip("Virtual keyboard to show in results.")]
    public VirtualKeyboard keyboard;

    [Tooltip("Text objects to show in results.")]
    public UnityEngine.UI.Text[] textObjects;

    [Tooltip("Canvas renderers to show in results (e.g. buttons).")]
    public CanvasRenderer[] canvasObjects;

    [Tooltip("VRTK UI Pointer to activate when showing the results.")]
    public VRTK.VRTK_UIPointer controllerPointer;

    /// <summary>
    /// Time at which the song ends.
    /// </summary>
    private float endTime = -1f;

    /// <summary>
    /// Have the results already been displayed?
    /// </summary>
    private bool hasDisplayedResults = false;

    void Start()
    {
        graph.gameObject.SetActive(false);

        foreach(UnityEngine.UI.Text text in textObjects)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            text.gameObject.SetActive(false);
        }

        foreach (CanvasRenderer canvasRenderer in canvasObjects)
        {
            canvasRenderer.SetAlpha(0);

            canvasRenderer.gameObject.SetActive(false);
        }

        LaserBehaviour laser = laserHarp.GetComponentInChildren<LaserBehaviour>();

        foreach(MIDIChart.Track track in laser.chart.tracks)
        {
            // For some reason, we keep some tracks empty...
            if (track.notes.Count == 0)
                continue;

            MIDIChart.Note lastNote = track.notes[track.notes.Count - 1];

            if (endTime < lastNote.endBeat)
                endTime = lastNote.endBeat;
        }

        // Wait a little bit more before showing the results
        endTime += 1f;
    }

    IEnumerator AnimationCoroutine()
    {
        // Hide the lasers
        while(true)
        {
            float lerpFactor = Time.deltaTime * 2;

            laserHarp.transform.localScale += (new Vector3(1, 0, 1) - laserHarp.transform.localScale) * lerpFactor;

            if (laserHarp.transform.localScale.y < 10e-3)
            {
                laserHarp.SetActive(false);
                break;
            }

            yield return null;
        }

        // Wait for the scoreboard to show the top 10
        while (!scoreboard.IsDisplayingScoreboard)
        {
            yield return null;
        }

        // Wait for scoreboard to have shown for a while before showing other things
        yield return new WaitForSeconds(1);

        Vector3[] graphChildScales = new Vector3[graph.gameObject.transform.childCount];

        for(int i = 0; i < graphChildScales.Length; i++)
        {
            graphChildScales[i] = graph.gameObject.transform.GetChild(i).transform.localScale;

            graph.gameObject.transform.GetChild(i).transform.localScale = Vector3.zero;
        }

        // Avoids a weird pop-up of the graph before it scales in
        yield return new WaitForEndOfFrame();
        
        graph.gameObject.SetActive(true);

        graph.transform.localScale = new Vector3(0, 0, 0);

        // Make graph pop out
        while (true)
        {
            float lerpFactor = Time.deltaTime * 3;

            graph.transform.localScale += (Vector3.one - graph.transform.localScale) * lerpFactor;

            // Due to some Unity oddity, the immediate children to the scaled parent are also scaled (but wrongly so), so we reset them to their original scale while lerping the graph object
            for (int i = 0; i < graphChildScales.Length; i++)
            {
                graph.gameObject.transform.GetChild(i).transform.localScale = graphChildScales[i];
            }

            if (Mathf.Abs(1 - graph.transform.localScale.y) < 10e-4)
            {
                for (int i = 0; i < graphChildScales.Length; i++)
                {
                    graph.gameObject.transform.GetChild(i).transform.localScale = graphChildScales[i];
                }

                graph.transform.localScale = Vector3.one;
                break;
            }

            yield return null;
        }

        // Display accuracy animation
        graph.StartAnimation();

        // Wait for graph animation
        yield return new WaitForSeconds(3);

        // Display keyboard
        keyboard.StartAnimation();

        // Enable pointer for keyboard & menus
        controllerPointer.enabled = true;
        controllerPointer.gameObject.GetComponent<VRTK.VRTK_Pointer>().enabled = true;
        controllerPointer.gameObject.GetComponent<VRTK.VRTK_StraightPointerRenderer>().enabled = true;

        // Show all miscellaneous objects

        foreach (UnityEngine.UI.Text text in textObjects)
        {
            text.gameObject.SetActive(true);
        }

        foreach (CanvasRenderer canvasRenderer in canvasObjects)
        {
            canvasRenderer.gameObject.SetActive(true);
        }

        float alpha = 0;

        while (true)
        {
            float lerpFactor = Time.deltaTime * 2;

            alpha += lerpFactor;

            if (alpha > 1)
                alpha = 1;

            foreach (UnityEngine.UI.Text text in textObjects)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            }

            foreach (CanvasRenderer canvasRenderer in canvasObjects)
            {
                canvasRenderer.SetAlpha(alpha);
            }

            if (Mathf.Abs(1 - alpha) < 10e-3)
                break;

            yield return null;
        }
    }

    void Update()
    {
        if (!hasDisplayedResults && BeatTime.beat > endTime)
        {
            hasDisplayedResults = true;

            ShowResults();
        }
    }

    public void ShowResults()
    {
        scoreboard.StartAnimation();

        StartCoroutine(AnimationCoroutine());
    }
}
