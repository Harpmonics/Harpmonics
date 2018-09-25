using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Array;

[RequireComponent(typeof(MeshFilter))]
public class Deformation : MonoBehaviour
{
	Mesh mesh;
	Vector3[] ogVert, disVert, vertexVelocities;
    public float springForce = 20f;
    float uniformScale = 1f;
    public float damping = 5f;

    void Start ()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		ogVert = mesh.vertices;
		disVert = new Vector3[ogVert.Length];
		for (int i = 0; i < ogVert.Length; i++)
		{
			disVert[i] = ogVert[i];
		}
        vertexVelocities = new Vector3[ogVert.Length];

        //VerticeLocations(ogVert);
    }
	
	void Update ()
	{
        uniformScale = transform.localScale.x;
        for (int i = 0; i < disVert.Length; i++)
        {
            UpdateVertex(i);
        }
        mesh.vertices = disVert;
        mesh.RecalculateNormals();
    }

	public void AddDeformForce (Vector3 point, float force)
	{
        Debug.DrawLine(Camera.main.transform.position, point);
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < disVert.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = disVert[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    void UpdateVertex (int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = disVert[i] - ogVert[i];
        displacement *= uniformScale;
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime; // remove this line for eternally oscillating strings
        vertexVelocities[i] = velocity;
        disVert[i] += velocity * (Time.deltaTime / uniformScale);
    }

    // print location of every vertice
    private void VerticeLocations (Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        { Debug.Log(vertices[i]); }
    }
}
