using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class AccuracyGraph : MonoBehaviour
{
    [Tooltip("Root element of the laser harp, containing all individual lasers.")]
    public GameObject laserHarp;

    /// <summary>
    /// References to graph tracks.
    /// </summary>
    private GameObject[] tracks;

    /// <summary>
    /// List of track accuracies' corresponding vertices used to update to set graph height.
    /// </summary>
    private List<List<int>>[] tracksVertices;
    
    [Tooltip("Result track height."), Range(0, 3)]
    public float trackHeight = 1f;
    [Tooltip("Result track length."), Range(0, 10)]
    public float trackLength = 2f;
    [Tooltip("Result track width."), Range(0, 2)]
    public float trackWidth = 0.2f;
    [Tooltip("Spacing between result tracks."), Range(0, 2)]
    public float trackSpacing = 0.1f;

    [Tooltip("Size of label showing accuracy axis"), Range(0, 200)]
    public float axisLabelSize = 20f;

    /// <summary>
    /// Should the graph meshes be rebuilt? (only used in editor)
    /// </summary>
    private bool dirty = false;

    /// <summary>
    /// Should the graph start to appear with an animation?
    /// </summary>
    private bool isAnimating = false;

    /// <summary>
    /// Set automatically when animating to stop animating when interpolation changes are miniscule
    /// </summary>
    private bool shouldStopAnimating = false;

    private GameObject axisLabel;

    private static Dictionary<GameObject, List<float>> accuracies;

    private static Dictionary<int, GameObject> accuracyIndices;

    /// <summary>
    /// Track unaccumulated track accuracy using the track associated with obj.
    /// </summary>
    /// <param name="obj">Track associated with the accuracy.</param>
    /// <param name="accuracy">Non-accumulated accuracy at this note.</param>
    /// <param name="index">This note's index in the track.</param>
    public static void TrackAccuracy(GameObject obj, float accuracy, int index)
    {
        accuracies[obj][index] = accuracy;
    }

    /// <summary>
    /// Gets the accumulated track accuracies over time for the given track.
    /// </summary>
    /// <param name="track">The desired track (1-indexed)</param>
    /// <returns></returns>
    private float[] GetTrackAccuracies(int track)
    {
        float[] trackAccuracies;

        // Use placeholder for editor (since we won't have any accuracies at this point)
        if (!Application.isPlaying)
        {
            Random.InitState(42 + track);

            trackAccuracies = new float[10];

            trackAccuracies[0] = 1;

            for (int i2 = 1; i2 < trackAccuracies.Length; i2++)
            {
                trackAccuracies[i2] = trackAccuracies[i2 - 1] * 0.95f + Random.value * 0.05f;
            }
        }
        else
        {
            GameObject trackObject = accuracyIndices[track-1];

            List<float> rawAccuracies = accuracies[trackObject];
            
            trackAccuracies = new float[rawAccuracies.Count];

            // This shouldn't normally happen.
            if (rawAccuracies.Count == 0)
                return new float[2] { 1, 0.5f };

            trackAccuracies[0] = rawAccuracies[0];

            for(int i = 1; i < trackAccuracies.Length; i++)
            {
                trackAccuracies[i] = (rawAccuracies[i] + trackAccuracies[i-1])/2;
            }
        }

        return trackAccuracies;
    }
    
    /// <summary>
    /// Builds a mesh for a track
    /// </summary>
    /// <param name="track">The track to build the mesh for.</param>
    /// <param name="trackAccuracies">The overall accuracy achieved over time on that track.</param>
    /// <param name="trackVertices">A multidimensional list where each index corresponds to an accuracy that points to a list of vertices representing that value.</param>
    private void BuildMesh(GameObject track, float[] trackAccuracies, List<List<int>> trackVertices)
    {
        List<Vector3> vertices = new List<Vector3>(trackAccuracies.Length * 4 * 2);

        float trackLengthHalf = trackLength / 2;
        float trackWidthHalf = trackWidth / 2;

        // Start side

        float firstAcc = trackAccuracies[0];
        float lastAcc = trackAccuracies[trackAccuracies.Length - 1];


        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight * firstAcc, trackWidthHalf)); trackVertices[0].Add(vertices.Count);
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight * firstAcc, -trackWidthHalf)); trackVertices[0].Add(vertices.Count);

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight * firstAcc, trackWidthHalf)); trackVertices[0].Add(vertices.Count);


        // End side

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight * lastAcc, -trackWidthHalf)); trackVertices[trackVertices.Count - 1].Add(vertices.Count);
        vertices.Add(new Vector3(trackLengthHalf, trackHeight * lastAcc, trackWidthHalf)); trackVertices[trackVertices.Count - 1].Add(vertices.Count);

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight * lastAcc, trackWidthHalf)); trackVertices[trackVertices.Count - 1].Add(vertices.Count);
        vertices.Add(new Vector3(trackLengthHalf, 0, trackWidthHalf));


        // Bottom side

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));


        // Intermediate accuracies

        for(int i = 1; i < trackAccuracies.Length; i++)
        {
            float prevAcc = trackAccuracies[i - 1] * trackHeight;
            float curAcc = trackAccuracies[i] * trackHeight;

            float prevLength = (trackLength / (trackAccuracies.Length - 1)) * (i - 1) - trackLengthHalf;
            float curLength = (trackLength / (trackAccuracies.Length - 1)) * i - trackLengthHalf;

            // Top

            vertices.Add(new Vector3(prevLength, prevAcc, -trackWidthHalf)); trackVertices[i - 1].Add(vertices.Count);
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf)); trackVertices[i].Add(vertices.Count);
            vertices.Add(new Vector3(curLength, curAcc, -trackWidthHalf)); trackVertices[i].Add(vertices.Count);

            vertices.Add(new Vector3(prevLength, prevAcc, -trackWidthHalf)); trackVertices[i - 1].Add(vertices.Count);
            vertices.Add(new Vector3(prevLength, prevAcc, trackWidthHalf)); trackVertices[i - 1].Add(vertices.Count);
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf)); trackVertices[i].Add(vertices.Count);

            // Left

            vertices.Add(new Vector3(prevLength, 0, -trackWidthHalf));
            vertices.Add(new Vector3(prevLength, prevAcc, -trackWidthHalf)); trackVertices[i - 1].Add(vertices.Count);
            vertices.Add(new Vector3(curLength, curAcc, -trackWidthHalf)); trackVertices[i].Add(vertices.Count);

            vertices.Add(new Vector3(prevLength, 0, -trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, -trackWidthHalf)); trackVertices[i].Add(vertices.Count);
            vertices.Add(new Vector3(curLength, 0, -trackWidthHalf));

            // Right

            vertices.Add(new Vector3(prevLength, 0, trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf)); trackVertices[i].Add(vertices.Count);
            vertices.Add(new Vector3(prevLength, prevAcc, trackWidthHalf)); trackVertices[i - 1].Add(vertices.Count);

            vertices.Add(new Vector3(prevLength, 0, trackWidthHalf));
            vertices.Add(new Vector3(curLength, 0, trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf)); trackVertices[i].Add(vertices.Count);
        }


        List<int> triangles = new List<int>(vertices.Count);
        List<Vector2> uvs = new List<Vector2>(vertices.Count);

        for(int i = 0; i < vertices.Count; i++)
        {
            triangles.Add(i);
        }

        for (int i = 0; i < vertices.Count; i += 3)
        {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
        }

        Mesh mesh = new Mesh();

        mesh.MarkDynamic();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0, true);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        track.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Separate function for the graph axis for manipulating the mesh in some way (unused)
    private void BuildGraphAxis(GameObject track, float zEnd)
    {
        List<Vector3> vertices = new List<Vector3>(2 * 4 * 2);

        float trackLengthHalf = trackLength / 2;
        float trackWidthHalf = trackWidth / 2;

        // Start side


        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, -trackWidthHalf));

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, trackWidthHalf));


        // End side

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, trackWidthHalf));

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, trackWidthHalf));


        // Label

        float size = axisLabelSize * 0.002f;

        vertices.Add(new Vector3(trackLengthHalf, trackHeight - size, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf + zEnd - track.transform.localPosition.z));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight - size, -trackWidthHalf + zEnd - track.transform.localPosition.z));

        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf + zEnd - track.transform.localPosition.z));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight - size, -trackWidthHalf));


        // Bottom side

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));

        // Top

        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf));

        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, trackWidthHalf));

        // Left

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf));

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));

        // Right

        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight, trackWidthHalf));

        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight, trackWidthHalf));


        List<int> triangles = new List<int>(vertices.Count);
        List<Vector2> uvs = new List<Vector2>(vertices.Count);

        for (int i = 0; i < vertices.Count; i++)
        {
            triangles.Add(i);
        }

        for (int i = 0; i < vertices.Count; i += 3)
        {
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
        }

        Mesh mesh = new Mesh();

        mesh.MarkDynamic();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0, true);
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        track.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void BuildTracks()
    {
        // Remove old meshes

        List<GameObject> oldMeshes = new List<GameObject>();

        for(int i = 0; i < this.transform.childCount; i++)
        {
            oldMeshes.Add(this.transform.GetChild(i).gameObject);
        }

        foreach(GameObject obj in oldMeshes)
        {
            DestroyImmediate(obj);
        }

        // No laser harp to extrapolate tracks from
        if (laserHarp == null)
            return;

        int noteTracks = 0;

        accuracies = new Dictionary<GameObject, List<float>>();

        accuracyIndices = new Dictionary<int, GameObject>();

        foreach (LaserBehaviour laser in laserHarp.GetComponentsInChildren<LaserBehaviour>())
        {
            LaserMIDITimingJudge judger = laser.gameObject.GetComponent<LaserMIDITimingJudge>();

            List<float> trackAccuracies = new List<float>();

            for(int i = 0; i < judger.notes.Length; i++)
            {
                trackAccuracies.Add(0f);
            }

            accuracyIndices.Add(accuracies.Count, laser.gameObject);

            accuracies.Add(laser.gameObject, trackAccuracies);

            noteTracks++;
        }

        tracksVertices = new List<List<int>>[noteTracks+1];

        tracks = new GameObject[noteTracks+1];

        // The graph scale is added as a track to make it move with the others
        tracks[0] = new GameObject("Graph Scale");
        tracksVertices[0] = new List<List<int>>();

        for (int i = 1; i < tracks.Length; i++)
        {
            tracks[i] = new GameObject("Track " + i);

            tracksVertices[i] = new List<List<int>>();
        }

        float zPos = -((trackWidth + trackSpacing) * tracks.Length) / 2 + trackSpacing/2;

        float finalAvgAcc = 0;

        for(int i = 0; i < tracks.Length; i++)
        {
            GameObject track = tracks[i];

            List<List<int>> trackVertices = tracksVertices[i];

            track.transform.parent = this.transform;
            track.transform.rotation = this.transform.rotation;

            track.transform.localPosition = new Vector3(0, 0, zPos);

            zPos += trackWidth + trackSpacing;

            MeshFilter mesh = track.AddComponent<MeshFilter>();
            MeshRenderer renderer = track.AddComponent<MeshRenderer>();

            // Copy values to each individual mesh
            renderer.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;

            // Use second material for the graph scale to differentiate it from the others
            if (track == tracks[0] && GetComponent<MeshRenderer>().sharedMaterials.Length > 1)
            {
                renderer.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterials[1];

                trackVertices.Add(new List<int>());
                trackVertices.Add(new List<int>());

                BuildMesh(track, new float[] { 1f, 1f }, trackVertices);
            }
            else
            {
                float[] trackAccuracies = GetTrackAccuracies(i);

                for (int i2 = 0; i2 < trackAccuracies.Length; i2++)
                {
                    trackVertices.Add(new List<int>());
                }

                finalAvgAcc += trackAccuracies[trackAccuracies.Length - 1];

                BuildMesh(track, trackAccuracies, trackVertices);
            }
        }

        //BuildGraphAxis(tracks[0], zPos);

        float graphCenterZ = -((trackWidth + trackSpacing) * tracks.Length) / 2 + trackSpacing / 2 + trackWidth + ((trackWidth + trackSpacing) * (tracks.Length-1)) / 2;

        finalAvgAcc /= tracks.Length - 1;

        BuildGUI(finalAvgAcc, graphCenterZ);
    }

    private void BuildGUI(float finalAvgAcc, float centerZ)
    {
        GameObject labelAxis = new GameObject("Axis label");

        labelAxis.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        labelAxis.transform.parent = this.transform;
        labelAxis.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        labelAxis.transform.localPosition = new Vector3(trackLength / 2 + 0.02f, trackHeight * finalAvgAcc - axisLabelSize * 0.002f/2, centerZ);
        labelAxis.transform.localRotation = Quaternion.Euler(0, -90, 0);

        UnityEngine.UI.Text axisLabelText = labelAxis.AddComponent<UnityEngine.UI.Text>();

        axisLabelText.resizeTextMinSize = 50;
        axisLabelText.resizeTextMaxSize = 1000;
        axisLabelText.resizeTextForBestFit = true;

        axisLabelText.alignment = TextAnchor.MiddleCenter;

        axisLabelText.text = string.Format("{0:f0}%", finalAvgAcc*100 + 0.5f);

        axisLabelText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);

        axisLabelText.rectTransform.sizeDelta = new Vector2(axisLabelSize*10f, axisLabelSize*10f);

        axisLabelText.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        axisLabel = labelAxis;

        GameObject labelGraph = new GameObject("Graph label");

        labelGraph.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        labelGraph.transform.parent = this.transform;
        labelGraph.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        labelGraph.transform.localPosition = new Vector3(-trackLength / 2, trackHeight + 0.4f, 0);
        labelGraph.transform.localRotation = Quaternion.Euler(0, -90, 0);

        UnityEngine.UI.Text graphLabelText = labelGraph.AddComponent<UnityEngine.UI.Text>();

        graphLabelText.resizeTextMinSize = 50;
        graphLabelText.resizeTextMaxSize = 1000;
        graphLabelText.resizeTextForBestFit = true;

        graphLabelText.alignment = TextAnchor.MiddleCenter;

        graphLabelText.text = "Accuracy over time";

        graphLabelText.font = Font.CreateDynamicFontFromOSFont("Arial", 16);

        graphLabelText.rectTransform.sizeDelta = new Vector2(axisLabelSize*25f, axisLabelSize*25f);

        graphLabelText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Flattens the graph so that only the axis is seen
    /// </summary>
    public void Flatten()
    {
        for(int i = 1; i < tracks.Length; i++)
        {
            GameObject track = tracks[i];

            List<List<int>> trackVertices = tracksVertices[i];

            Mesh mesh = track.GetComponent<MeshFilter>().mesh;

            Vector3[] vertices = mesh.vertices;

            // Update every vertex in the tracked list
            foreach(List<int> trackedVertices in trackVertices)
            {
                foreach(int index in trackedVertices)
                {
                    Vector3 vertex = vertices[index-1];
                    vertices[index-1].Set(vertex.x, 0, vertex.z);
                }
            }

            mesh.vertices = vertices;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        Vector3 curPos = axisLabel.transform.localPosition;

        axisLabel.transform.localPosition = new Vector3(-trackLength/2, trackHeight - axisLabelSize * 0.002f / 2, curPos.z);
        axisLabel.GetComponent<UnityEngine.UI.Text>().text = "";
    }

    protected void Animate()
    {
        float finalAvgAcc = 0;

        float xPos = -trackLength / 2;

        // Base lerp speed
        float labelLerpFactor = Time.deltaTime * 5;

        // If we should stop animating, then we set all values to their final ones before stopping the animation
        if (shouldStopAnimating)
            labelLerpFactor = 1;

        for (int i = 1; i < tracks.Length; i++)
        {
            GameObject track = tracks[i];

            List<List<int>> trackVertices = tracksVertices[i];

            float[] trackAccuracies = GetTrackAccuracies(i);

            // Lerp according to how many accuracies we're showing (using 10 as a base speed)
            float lerpFactor = labelLerpFactor * (trackAccuracies.Length / 10f);

            if (shouldStopAnimating)
                lerpFactor = 1;

            Mesh mesh = track.GetComponent<MeshFilter>().mesh;

            Vector3[] vertices = mesh.vertices;

            // Find out how far we've progressed in the animation
            int maxAcc = 0;

            for(int i2 = 0; i2 < trackVertices.Count; i2++)
            {
                Vector3 vertex = vertices[trackVertices[i2][0] - 1];

                maxAcc = i2;

                if (vertex.y / (trackAccuracies[i2] * trackHeight) < 0.5f)
                    break;
            }

            finalAvgAcc += trackAccuracies[maxAcc];

            xPos = vertices[trackVertices[maxAcc][0] - 1].x + 0.02f;

            // Update every vertex in the tracked list
            for(int i2 = 0; i2 <= maxAcc; i2++)
            {
                foreach (int index in trackVertices[i2])
                {
                    Vector3 vertex = vertices[index - 1];

                    vertices[index - 1].Set(vertex.x, vertex.y + (trackAccuracies[i2] * trackHeight - vertex.y) * lerpFactor, vertex.z);
                }
            }

            mesh.vertices = vertices;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        finalAvgAcc /= tracks.Length - 1;

        Vector3 curPos = axisLabel.transform.localPosition;

        axisLabel.GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:f0}%", finalAvgAcc * 100 + 0.5f);

        // We lerp the x-axis more linearly to avoid "stopping" between values
        axisLabel.transform.localPosition = new Vector3(curPos.x + Mathf.Min((xPos - curPos.x) * labelLerpFactor * 2, 0.2f * labelLerpFactor), curPos.y + (trackHeight * finalAvgAcc - axisLabelSize * 0.002f / 2 - curPos.y) * labelLerpFactor, curPos.z);

        if (shouldStopAnimating)
        {
            isAnimating = false;
            shouldStopAnimating = false;
        }
        else
        {
            float xPosDiff = (xPos - curPos.x);

            if (xPosDiff < 10e-5)
                shouldStopAnimating = true;
        }
    }

    public void StartAnimation()
    {
        isAnimating = true;
    }

    // Start is not called if the object is deactivated immediately
    public void Initialize()
    {
        // References are lost when playing the scene, so we just rebuild from the same settings
        BuildTracks();
        Flatten();

        dirty = false;
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
        if (isAnimating)
            Animate();

        // Only need to build mesh in editor, we only animate in the game
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (dirty)
            {
                dirty = false;
                BuildTracks();
            }
            if (tracks != null && tracks.Length > 1 && tracks[1].GetComponent<MeshRenderer>().sharedMaterial != GetComponent<MeshRenderer>().sharedMaterial)
                dirty = true;
        }
#endif
    }
}
