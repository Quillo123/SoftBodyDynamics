using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RigidPolygon : MonoBehaviour
{
    //[Range(3, 100)]
    //public int numVertices = 3;

    public Vector3[] vertices { get { return polygon.vertices; } set { polygon.vertices = value; } }

    [SerializeField] Polygon polygon = new Polygon(null);

    public bool showLines = true;

    [Range(1, 10)]
    public int thickness = 1;

    public Color color = Color.white;

    public bool isPhysical = false;
    [Range(0,1)]
    public float friction = .2f;
    [Range(0,1)]
    public float normalAbsorption = 0;

    public Vector3 upperBound;
    public Vector3 lowerBound;
    public Vector3 raycastPos;

    public SoftBodyMesh SB_Mesh { get; private set; }


    //Cached Objects
    MeshFilter meshFilter;

    private void OnValidate()
    {

        UpdateVertices();
        GetBounds();

        meshFilter = GetComponent<MeshFilter>();

        SB_Mesh = new SoftBodyMesh(meshFilter);

        SB_Mesh.GenerateMeshVerticesFromShape(vertices, 2);
    }

    private void UpdateVertices()
    {
        if(vertices == null)
        {
            vertices = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                float angle = i * (360f / vertices.Length);
                vertices[i] = new Vector3(
                    transform.position.x + 5 * Mathf.Cos(Mathf.Deg2Rad * angle),
                    transform.position.y + 5 * Mathf.Sin(Mathf.Deg2Rad * angle)
                    );
            }
        }

    }

    public void OnStart()
    {
        UpdateVertices();
        GetBounds();

        meshFilter = GetComponent<MeshFilter>();

        SB_Mesh = new SoftBodyMesh(meshFilter);

        SB_Mesh.GenerateMeshVerticesFromShape(vertices, 2);
    }

    private void Update()
    {

    }

    public void PhysicsUpdate(SoftBodyObject[] softBodies)
    {

        foreach (var sb in softBodies)
        {
            foreach(var point in sb.massPoints)
            {
                var pos = sb.transform.TransformPoint(point.position);
                if (pos.x < upperBound.x && pos.x > lowerBound.x)
                {
                    if (pos.y < upperBound.y && pos.y > lowerBound.y)
                    {
                        Debug.Log("bound-collision");
                        HandleCollision(point, sb.transform);
                    }
                }
            }   
        }
    }

    private void HandleCollision(MassPoint point, Transform objectTransform)
    {
        var pos = objectTransform.TransformPoint(point.position);
        var hits = Physics2D.RaycastAll(pos, raycastPos - pos);
        IEnumerable<RaycastHit2D> hitObject = hits.Where(item => item.transform == this.transform);
        if (hitObject.Count() > 0)
        {
            if (hitObject.Count() % 2 != 0)
            {
                Debug.Log("Collision");
                Vector3 closestPoint = GetClosestPoint(pos);
                var lineHit = Physics2D.Raycast(pos, closestPoint - pos);
                point.position = objectTransform.InverseTransformPoint(closestPoint); //transform.InverseTransformPoint(closestPoint);

                if (Mathf.Abs(point.velocity.x) < Universe.precision * 100)
                    point.velocity = new Vector3(0f, point.velocity.y, point.velocity.z);
                if (Mathf.Abs(point.velocity.y) < Universe.precision * 100)
                    point.velocity = new Vector3(point.velocity.x, 0f, point.velocity.z);
                point.Reflect(lineHit.normal, normalAbsorption, friction);
            }
        }
        
    }

    private Vector3 GetClosestPoint(Vector3 position)
    {
        Vector3 point = Helpers.ClosestPointOnLine(
            transform.TransformPoint(vertices[0]),
            transform.TransformPoint(vertices[vertices.Length - 1]),
            position);
        for(int i = 0; i < vertices.Length - 1; i++)
        {
            Vector3 temp = Helpers.ClosestPointOnLine(
                transform.TransformPoint(vertices[i]),
                transform.TransformPoint(vertices[i+1]),
                position);
            if (Vector3.Distance(position, temp) < Vector3.Distance(position, point))
                point = temp;
        }
        return point;
    }

    public void OnEditorUpdate()
    {
        for(int i = 0; i < vertices.Length; i++)
        {
            SB_Mesh.GenerateMeshVerticesFromShape(vertices, 2);
        }

        GetBounds();
    }

    private void GetBounds()
    {
        upperBound = transform.TransformPoint(vertices[0]);
        lowerBound = transform.TransformPoint(vertices[0]);
        foreach(var vert in vertices)
        {
            var worldP = transform.TransformPoint(vert);
            if (worldP.x > upperBound.x) upperBound.x = worldP.x;
            else if (worldP.x < lowerBound.x) lowerBound.x = worldP.x;

            if (worldP.y > upperBound.y) upperBound.y = worldP.y;
            else if (worldP.y < lowerBound.y) lowerBound.y = worldP.y;
        }

        raycastPos = new Vector2(upperBound.x + 1, upperBound.y + 1);

    }

    
}
