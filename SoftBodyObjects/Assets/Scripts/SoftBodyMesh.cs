using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoftBodyMesh
{
    Mesh mesh;
    MeshFilter meshFilter;

    public Vector3[] vertices;
    public Vector2[] UV;
    public int[] triangles;

    public SoftBodyMesh(MeshFilter meshFilter, Mesh mesh = null)
    {
        if (mesh)
        {
            this.mesh = mesh;
        }
        else
        {
            mesh = new Mesh();
        }
        this.meshFilter = meshFilter;
        meshFilter.mesh = mesh;

    }

    public Vector3[] GenerateMeshVerticesFromShape(Vector3[] pVertices, int fillFactor)
    {
        

        return null;
    }
}
