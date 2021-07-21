using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineComponent : MonoBehaviour
{
    public Line line = new Line();

    public bool showLine = true;

    [Range(1, 10)]
    public int thickness = 1;

    public Color color = Color.white;

    LineRenderer lineRenderer;
    internal bool isPoly = false;
    const float tFactor = 25f;

    private void OnValidate()
    {
        if (line == null)
        {
            line = new Line();
        }   

        Material material = new Material(Shader.Find("Unlit/Color"));

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.sharedMaterial = material;
        lineRenderer.startWidth = thickness / 10f;
        lineRenderer.endWidth = thickness / 10f;
        lineRenderer.numCapVertices = 2;
        lineRenderer.numCornerVertices = 2;
    }

    private void Update()
    {
        OnUpdate();

        //bool found;
        //Line.GetIntersection(this.line, line2.line, out found);
        //if (found)
        //    Debug.Log("intersected");
    }

    public LineComponent(Vector2 _start, Vector2 _end)
    {
        line.start = _start;
        line.end = _end;

        lineRenderer.SetPosition(0, line.start);
        lineRenderer.SetPosition(1, line.end);
    }

    protected virtual void OnUpdate()
    {
        if (showLine)
        {
            DrawLine();
        }
        else
        {
            HideLine();
        }
    }

    public virtual void OnEditorUpdate()
    {
        DrawLineInEditor();
        if (GetComponent<PhysicsLine>())
        {
            GetComponent<PhysicsLine>().UpdateColliderPoints();
        }
        
    }

    public void DrawLine()
    {
        if (lineRenderer.enabled == false)
        {
            lineRenderer.enabled = true;
        }

        lineRenderer.startWidth = thickness / tFactor;
        lineRenderer.endWidth = thickness / tFactor;

        lineRenderer.sharedMaterial.color = color;

        if (lineRenderer.GetPosition(0) != (Vector3)transform.TransformPoint(line.start))
        {
            lineRenderer.SetPosition(0, transform.TransformPoint(line.start));
        }
        if (lineRenderer.GetPosition(1) != (Vector3)transform.TransformPoint(line.end))
        {
            lineRenderer.SetPosition(1, transform.TransformPoint(line.end));
        }
    }

    internal Vector3 ClosestPoint(Vector3 position)
    {            
        var point = line.ClosestPoint(transform.InverseTransformPoint(position));
        return transform.TransformPoint(point);
    }

    public void HideLine()
    {
        lineRenderer.enabled = false;
    }

    public void DrawLineInEditor()
    {
        if (showLine)
        {
            DrawLine();
        }
        else
        {
            HideLine();
        }
    }

    public void SetStart(Vector2 start) 
    { 
        line.start = start;
    }
    public void SetEnd(Vector2 end) 
    { 
        line.end = end;
    }

    public Vector2 Start()
    {
        return line.start;
    }

    public Vector2 End()
    {
        return line.end;
    }

    public void CenterPositionOnLine()
    {
        Vector2 start, end;
        start = transform.TransformPoint(line.start);
        end = transform.TransformPoint(line.end);

        transform.position = transform.TransformPoint(line.Center());
        line.start = transform.InverseTransformPoint(start);
        line.end = transform.InverseTransformPoint(end);
    }

    public void CenterLineOnPosition(Vector2 lastPos)
    {
        Vector2 moveAmount =  (Vector2)transform.position - lastPos;

        line.start += moveAmount;
        line.end += moveAmount;
    }

}
