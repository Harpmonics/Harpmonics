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

    /// <summary>
    /// Should the graph meshes be rebuilt?
    /// </summary>
    private bool dirty = false;
    
    private void BuildMesh(GameObject track)
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


        // Intermediary accuracies

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

    bool calledOnce = false;

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

        tracks = new GameObject[noteTracks];

        for(int i = 0; i < tracks.Length; i++)
        {
            tracks[i] = new GameObject("Track " + i);
        }

        float zPos = -((trackWidth + trackSpacing) * tracks.Length) / 2 + trackSpacing/2;

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

            BuildMesh(track);
        }
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
        if (tracks != null && tracks.Length > 0 && tracks[0].GetComponent<MeshRenderer>().sharedMaterial != GetComponent<MeshRenderer>().sharedMaterial)
            dirty = true;
#endif
    }
}
