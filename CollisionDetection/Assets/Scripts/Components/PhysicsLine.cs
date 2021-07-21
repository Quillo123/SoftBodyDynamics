using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineComponent), typeof(EdgeCollider2D))]
public class PhysicsLine : MonoBehaviour
{

    LineComponent line;
    EdgeCollider2D edgeCollider;

    private void OnValidate()
    {
        line = GetComponent<LineComponent>();
        edgeCollider = GetComponent<EdgeCollider2D>();


        UpdateColliderPoints();
    }

    private void Update()
    {
        OnUpdate();
    }

    protected void OnUpdate()
    {
        UpdateColliderPoints();
    }

    public void UpdateColliderPoints()
    {
        Vector2[] points = { line.Start(), line.End() };
        edgeCollider.points = points;
    }

    
}
