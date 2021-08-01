using System.Collections;
using System;
using UnityEngine;

[Serializable]
public struct Polygon
{
    public Vector3[] vertices;

    public Polygon(Vector3[] vertices)
    {
        if(vertices == null)
        {
            this.vertices = new Vector3[3];
            this.vertices[0] = new Vector3(0, 0, 0);
            this.vertices[1] = new Vector3(1, 0, 0);
            this.vertices[2] = new Vector3(0, 1, 0);
        }
        else
        {
            this.vertices = vertices;  
        }
    }

    public Vector3 FindClosestPoint(Vector3 point)
    {
        Vector3 closestPoint = vertices[0];
        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 tempPoint;
            if(i != vertices.Length - 1)
            {
                tempPoint = Helpers.ClosestPointOnLine(vertices[i], vertices[i + 1], point);
            }
            else
            {
                tempPoint = Helpers.ClosestPointOnLine(vertices[i], vertices[0], point);
            }

            if (Vector3.Distance(point, tempPoint) < Vector3.Distance(closestPoint, point))
            {
                closestPoint = tempPoint;
            }
        }
        return closestPoint;
    }
}
