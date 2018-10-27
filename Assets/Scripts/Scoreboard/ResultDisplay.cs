using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class ResultDisplay : MonoBehaviour
{

    // TODO: Populate automatically
    [Tooltip("Number of tracks the player can hit."), Range(0, 20)]
    public int noteTracks = 3;

    // TODO: Placeholder, need some way to read these from the actual results
    private float[] trackAccuracies = new float[] { 1.0f, 0.8f, 0.9f, 0.5f, 0.6f, 0.7f, 0.8f, 0.4f };

    private GameObject[] tracks;
    
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
    /// Should the graph meshes be rebuilt?
    /// </summary>
    private bool dirty = false;
    
    private void BuildMesh(GameObject track, float[] trackAccuracies)
    {
        List<Vector3> vertices = new List<Vector3>(trackAccuracies.Length * 4 * 2);

        float trackLengthHalf = trackLength / 2;
        float trackWidthHalf = trackWidth / 2;

        // Start side

        float firstAcc = trackAccuracies[0];
        float lastAcc = trackAccuracies[trackAccuracies.Length - 1];


        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight * firstAcc, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight * firstAcc, -trackWidthHalf));

        vertices.Add(new Vector3(-trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, 0, trackWidthHalf));
        vertices.Add(new Vector3(-trackLengthHalf, trackHeight * firstAcc, trackWidthHalf));


        // End side
        
        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight * lastAcc, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight * lastAcc, trackWidthHalf));

        vertices.Add(new Vector3(trackLengthHalf, 0, -trackWidthHalf));
        vertices.Add(new Vector3(trackLengthHalf, trackHeight * lastAcc, trackWidthHalf));
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

            vertices.Add(new Vector3(prevLength, prevAcc, -trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, -trackWidthHalf));

            vertices.Add(new Vector3(prevLength, prevAcc, -trackWidthHalf));
            vertices.Add(new Vector3(prevLength, prevAcc, trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf));

            // Left

            vertices.Add(new Vector3(prevLength, 0, -trackWidthHalf));
            vertices.Add(new Vector3(prevLength, prevAcc, -trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, -trackWidthHalf));

            vertices.Add(new Vector3(prevLength, 0, -trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, -trackWidthHalf));
            vertices.Add(new Vector3(curLength, 0, -trackWidthHalf));

            // Right

            vertices.Add(new Vector3(prevLength, 0, trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf));
            vertices.Add(new Vector3(prevLength, prevAcc, trackWidthHalf));

            vertices.Add(new Vector3(prevLength, 0, trackWidthHalf));
            vertices.Add(new Vector3(curLength, 0, trackWidthHalf));
            vertices.Add(new Vector3(curLength, curAcc, trackWidthHalf));
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

    private void BuildGraphAxis(GameObject track, float zEnd)
    {
        List<Vector3> vertices = new List<Vector3>(trackAccuracies.Length * 4 * 2);

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

        tracks = new GameObject[noteTracks+1];

        // The graph scale is added as a track to make it move with the others
        tracks[0] = new GameObject("Graph Scale");

        for (int i = 1; i < tracks.Length; i++)
        {
            tracks[i] = new GameObject("Track " + i);
        }

        float zPos = -((trackWidth + trackSpacing) * tracks.Length) / 2 + trackSpacing/2;

        int randIndex = 0;

        foreach (GameObject track in tracks)
        {
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
            }
            else
            {
                Random.InitState(randIndex++);

                float[] trackAccuracies = new float[10];

                trackAccuracies[0] = 1;

                for (int i = 1; i < trackAccuracies.Length; i++)
                {
                    trackAccuracies[i] = trackAccuracies[i-1] * 0.95f + Random.value*0.05f;
                }

                BuildMesh(track, trackAccuracies);
            }
        }

        BuildGraphAxis(tracks[0], zPos);

        BuildGUI(zPos);
    }

    private void BuildGUI(float graphZ)
    {
        GameObject labelAxis = new GameObject("Axis label");

        labelAxis.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        labelAxis.transform.parent = this.transform;
        labelAxis.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        labelAxis.transform.localPosition = new Vector3(trackLength / 2, trackHeight - axisLabelSize * 0.002f/2, graphZ);

        UnityEngine.UI.Text axisLabelText = labelAxis.AddComponent<UnityEngine.UI.Text>();

        axisLabelText.resizeTextMinSize = 5;
        axisLabelText.resizeTextMaxSize = 100;
        axisLabelText.resizeTextForBestFit = true;

        axisLabelText.alignment = TextAnchor.MiddleLeft;

        axisLabelText.text = "100%";

        axisLabelText.rectTransform.sizeDelta = new Vector2(axisLabelSize, axisLabelSize);

        axisLabelText.rectTransform.pivot = new Vector2(0, 0.5f);

        GameObject labelGraph = new GameObject("Graph label");

        labelGraph.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace;

        labelGraph.transform.parent = this.transform;
        labelGraph.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        labelGraph.transform.localPosition = new Vector3(-trackLength / 2, trackHeight + 0.4f, 0);

        UnityEngine.UI.Text graphLabelText = labelGraph.AddComponent<UnityEngine.UI.Text>();

        graphLabelText.resizeTextMinSize = 5;
        graphLabelText.resizeTextMaxSize = 100;
        graphLabelText.resizeTextForBestFit = true;

        graphLabelText.alignment = TextAnchor.MiddleCenter;

        graphLabelText.text = "Accuracy over time";

        graphLabelText.rectTransform.sizeDelta = new Vector2(axisLabelSize*1.5f, axisLabelSize*1.5f);

        graphLabelText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }


	void Start()
    {

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

        // Only need to build mesh in editor, we only animate in the game
#if UNITY_EDITOR
        if (dirty)
        {
            dirty = false;
            BuildTracks();
        }
        if (tracks != null && tracks.Length > 1 && tracks[1].GetComponent<MeshRenderer>().sharedMaterial != GetComponent<MeshRenderer>().sharedMaterial)
            dirty = true;
#endif
    }
}
