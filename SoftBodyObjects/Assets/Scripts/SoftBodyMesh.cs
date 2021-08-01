using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoftBodyMesh
{
    Mesh mesh;
    MeshFilter meshFilter;

    public Vector3[] vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            if (value.Length == _vertices.Length)
            {
                _vertices = value;
                mesh.vertices = _vertices;
            }
            else
            {
                Debug.LogError("Can't change vertices length!!");
            }
        }
    }

    private Vector3[] _vertices;
    public Vector2[] UV { get; private set; }
    public int[] triangles { get; private set; }

    public bool initialized { get; private set; } = false; 

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

    public void GenerateMeshVerticesFromShape(Vector3[] pVertices, int fillFactor)
    {
        List<Vector3> newVertices = new List<Vector3>();
        Vector3[] bounds = GetBounds(pVertices);
        float xStep = Mathf.Abs(bounds[0].x - bounds[1].x) / (fillFactor - 1);
        float yStep = Mathf.Abs(bounds[0].y - bounds[1].y) / (fillFactor - 1);

        for (int x = 0; x < fillFactor; x++)
        {
            for(int y = 0; y < fillFactor; y++)
            {
                newVertices.Add(bounds[0] - new Vector3(xStep * x, yStep * y, 0));
            }
        }

        List<int> newTriangles = new List<int>();

        for(int x = 0; x < fillFactor - 1; x++)
        {
            for(int y = 0; y < fillFactor - 1; y++)
            {
                int one = x + (y * fillFactor);
                int two = x + (y * fillFactor) + 1;
                int three = x + ((y + 1) * fillFactor);
                int four = x + ((y + 1) * fillFactor) + 1;


                
                if (x % 2 == y % 2)
                {
                    newTriangles.Add(one);
                    newTriangles.Add(two);
                    newTriangles.Add(four);
                    newTriangles.Add(four);
                    newTriangles.Add(three);
                    newTriangles.Add(one);
                }
                else
                {
                    newTriangles.Add(three);
                    newTriangles.Add(one);
                    newTriangles.Add(two);
                    newTriangles.Add(two);
                    newTriangles.Add(four);
                    newTriangles.Add(three);
                }


            }
        }

        _vertices = newVertices.ToArray();
        triangles = newTriangles.ToArray();

        if (!mesh)
            mesh = new Mesh();

        mesh.vertices = _vertices;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;

        initialized = true;
    }

    private Vector3[] GetBounds(Vector3[] vertices)
    {
        
        Vector3 upperBound = Vector2.zero;
        Vector3 lowerBound = Vector2.zero;
        foreach (var vert in vertices)
        {
            if (vert.x > upperBound.x) upperBound.x = vert.x;
            else if (vert.x < lowerBound.x) lowerBound.x = vert.x;

            if (vert.y > upperBound.y) upperBound.y = vert.y;
            else if (vert.y < lowerBound.y) lowerBound.y = vert.y;
        }

        Vector3[] retVal = { upperBound, lowerBound };
        return retVal;
    }
}
