using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class DragDeformation : MonoBehaviour 
{

    Mesh mesh, colliderMesh;
    /*
    Original Vertices
    Displaced Vertices
    Original Collider Vertices
    Displaced Collider Vertices
    Distance to Center
    */
    Vector3[] ov, dv, ocv, dcv, d2c;

    void Start ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        ov = mesh.vertices;
        dv = new Vector3[ov.Length];
        d2c = new Vector3[ov.Length];

        colliderMesh = GetComponent<MeshCollider>().sharedMesh;
        ocv = colliderMesh.vertices;
        dcv = new Vector3[ocv.Length];

        for (int i = 0; i < ov.Length; i++)
        {
            dv[i] = ov[i];
            dcv[i] = ocv[i];

            d2c[i] = ov[i] - transform.InverseTransformPoint(transform.position);
        }
    }

    void Update ()
    {
        mesh.vertices = dv;
        colliderMesh.vertices = dcv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        colliderMesh.RecalculateNormals();
        colliderMesh.RecalculateBounds();
    }

    public void Deform(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < dv.Length; i++)
        {
            DisplaceVertex(i, point);
        }
    }

    private void DisplaceVertex(int i, Vector3 point)
    {
        // scale the distance moved depending on how far from contact point the vertex is
        Vector3 dist = point - dv[i];
        // the distance factor is usually very small
        float df = dist.sqrMagnitude * 1000f;
        // update vertex positions of both meshes
        dv[i] += (dist + d2c[i]) * (1f / (1f + df));
        dcv[i] = dv[i];
    }

    void OnDestroy()
    {
        mesh.vertices = ov;
        colliderMesh.vertices = ocv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        colliderMesh.RecalculateNormals();
        colliderMesh.RecalculateBounds();
    }
}
