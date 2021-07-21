using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    [Range(3, 100)]
    public int numVertices = 3;

    public List<Vector2> vertices = new List<Vector2>();
    public List<LineComponent> edges = new List<LineComponent>();

    public bool showLines = true;

    [Range(1, 10)]
    public int thickness = 1;

    public Color color = Color.white;

    public bool isPhysical = false;

    public Vector2 upperBound;
    public Vector2 lowerBound;
    public Vector2 raycastPos;

    private void OnValidate()
    {
        if(vertices == null)
        {
            vertices = new List<Vector2>(numVertices);

            for(int i = 0; i < numVertices; i++)
            {
                float angle = i * (360f / numVertices);
                vertices[i] = new Vector2(
                    transform.position.x + 5 * Mathf.Cos(Mathf.Deg2Rad * angle), 
                    transform.position.y + 5 * Mathf.Sin(Mathf.Deg2Rad * angle)
                    );
            }

        }
        GetBounds();
    }

    private void Start()
    {
        transform.tag = "Polygon";

    }

    private void Update()
    {
        OnCollisionUpdate();
    }

    public void OnCollisionUpdate()
    {
        var mPoints = FindObjectsOfType<MassPoint>();
        foreach (var point in mPoints)
        {
            var pos = point.transform.position;
            if (pos.x < upperBound.x && pos.x > lowerBound.x)
            {
                if (pos.y < upperBound.y && pos.y > lowerBound.x)
                {
                    HandleCollision(point);
                }
            }
        }
    }

    private void HandleCollision(MassPoint point)
    {
        var hits = Physics2D.RaycastAll(point.transform.position, raycastPos - (Vector2)point.transform.position);
        if (hits.Length % 2 != 0)
        {
            Vector2 closestPoint = GetClosestPoint(point.transform.position);
            var lineHit = Physics2D.Raycast(point.transform.position, closestPoint - (Vector2)point.transform.position);
            point.transform.position = closestPoint;



            Vector2 newVelocity = Vector2.Reflect(point.GetVelocity(), lineHit.normal) * .8f;

            point.SetVelocity(newVelocity);
        }
    }

    private Vector2 GetClosestPoint(Vector3 position)
    {
        Vector2 point = edges[0].ClosestPoint(position);
        foreach (var edge in edges)
        {
            Vector2 temp = edge.ClosestPoint(position);
            if (Vector2.Distance(position, temp) < Vector2.Distance(position, point))
                point = temp;
        }
        return point;
    }

    public void OnEditorUpdate()
    {
        foreach(var edge in edges)
        {
            edge.color = color;
            edge.thickness = thickness;
            edge.showLine = showLines;
            edge.OnEditorUpdate();
        }

        GetBounds();
    }

    private void GetBounds()
    {
        upperBound = Vector2.zero;
        lowerBound = Vector2.zero;
        foreach(var vert in vertices)
        {
            if (vert.x > upperBound.x) upperBound.x = vert.x;
            else if (vert.x < lowerBound.x) lowerBound.x = vert.x;

            if (vert.y > upperBound.y) upperBound.y = vert.y;
            else if (vert.y < lowerBound.y) lowerBound.y = vert.y;
        }

        upperBound = transform.TransformPoint(upperBound);
        lowerBound = transform.TransformPoint(lowerBound);
        raycastPos = new Vector2(upperBound.x + 1, upperBound.y + 1);

    }

    public void GenerateEdges()
    {
        for(int i = transform.childCount; i > 0; i--)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        edges.Clear();

        if (isPhysical)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var line = new GameObject("edge" + i);
                line.transform.SetParent(transform);
                line.AddComponent<PhysicsLine>();
                var lineComp = line.GetComponent<LineComponent>();
                lineComp.isPoly = true;
                if (i == vertices.Count - 1)
                {
                    lineComp.SetStart(transform.TransformPoint(vertices[vertices.Count - 1]));
                    lineComp.SetEnd(transform.TransformPoint(vertices[0]));
                }
                else
                {
                    lineComp.SetStart(transform.TransformPoint(vertices[i]));
                    lineComp.SetEnd(transform.TransformPoint(vertices[i + 1]));
                }
                edges.Add(line.GetComponent<LineComponent>());
            }
        }
        else
        {
            for(int i = 0; i < vertices.Count; i++)
            {
                var line = new GameObject("edge" + i);
                line.transform.SetParent(transform);
                var lineComp = line.AddComponent<LineComponent>();
                lineComp.isPoly = true;
                if(i == vertices.Count - 1)
                {
                    lineComp.SetStart(transform.TransformPoint(vertices[vertices.Count - 1]));
                    lineComp.SetEnd(transform.TransformPoint(vertices[0]));
                }
                else
                {
                    lineComp.SetStart(transform.TransformPoint(vertices[i]));
                    lineComp.SetEnd(transform.TransformPoint(vertices[i + 1]));
                }
                edges.Add(line.GetComponent<LineComponent>());
            }
        }
    }
}
