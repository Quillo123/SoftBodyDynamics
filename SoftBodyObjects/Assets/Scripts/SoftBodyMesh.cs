using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoftBodyMesh
{
    Mesh mesh;
    MeshFilter meshFilter;

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

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

    public Vector3[] GenerateMeshVerticesFromShape(Vector3[] pVertices)
    {
        

        return null;
    }
}
