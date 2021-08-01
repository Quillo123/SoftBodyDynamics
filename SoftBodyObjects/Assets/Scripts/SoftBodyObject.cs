using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SoftBodyObject : MonoBehaviour
{
    #region Properties
    //SoftBody Properties
    public Polygon polygon = new Polygon(null);
    public Vector3[] vertices { get { return polygon.vertices; } set { polygon.vertices = vertices; } }

    public MassPoint[] massPoints;

    

    public Spring[] springs;



    public float mass = 10;
    [Range(3, 25)]
    public int fillFactor = 3;

    public bool isStatic = false;

    public bool roundForces = true;

    //MassPoint Properties
    public MassPointProperties massPointProperties = new MassPointProperties(10, 2, true, false);

    //Spring Properties
    public SpringProperties springProperties = new SpringProperties(1000, 1, 10, 20, true);

    //Arrays


    public SoftBodyMesh SB_Mesh { get; private set; }

    public Color color = Color.white;

    //Cached Objects
    MeshFilter meshFilter;

    #endregion

    #region UnityMessages
    private void OnValidate()
    {
        meshFilter = GetComponent<MeshFilter>();

        SB_Mesh = new SoftBodyMesh(meshFilter);

        SB_Mesh.GenerateMeshVerticesFromShape(vertices, fillFactor);
    }

    public void OnStart()
    {
        SB_Mesh.GenerateMeshVerticesFromShape(vertices, fillFactor);
        PopulateMassPoints();
        PopulateSprings();

    }

    
    #endregion

    public void PhysicsUpdate()
    {
        if (!isStatic)
        {
            foreach (Spring spring in springs)
            {
                var force = spring.SpringForceA();
                spring.A.AddForce(force);
                force = spring.SpringForceB();
                spring.B.AddForce(force);
            }

            Vector3[] newMeshVertices = new Vector3[massPoints.Length];

            for (int i = 0; i < massPoints.Length; i++)
            {
                massPoints[i].AddForce(Universe.gForce * massPoints[i].mass);
                massPoints[i].OnUpdate(Time.fixedDeltaTime, !isStatic);
                newMeshVertices[i] = massPoints[i].position;
            }

            SB_Mesh.vertices = newMeshVertices;
        }
}    


    #region Methods
    public void OnEditorUpdate()
    {
    }

    public void GenerateMesh()
    {
        SB_Mesh.GenerateMeshVerticesFromShape(vertices, fillFactor);
    }

    private void PopulateMassPoints()
    {
        if (SB_Mesh.initialized)
        {
            massPoints = new MassPoint[SB_Mesh.vertices.Length];

            
            for (int i = 0; i < massPoints.Length; i++)
            {
                massPoints[i] = new MassPoint(
                    SB_Mesh.vertices[i],
                    mass / massPoints.Length,
                    .1f,
                    massPointProperties,
                    massPoints);
            }
        }
        
    }

    private void PopulateSprings()
    {
        if (SB_Mesh.initialized)
        {
            List<Spring> newSprings = new List<Spring>();
            for (int x = 0; x < fillFactor - 1; x++)
            {
                for (int y = 0; y < fillFactor - 1; y++)
                {
                    int one = x + (y * fillFactor);
                    int two = x + (y * fillFactor) + 1;
                    int three = x + ((y + 1) * fillFactor);
                    int four = x + ((y + 1) * fillFactor) + 1;

                    SpringProperties properties = new SpringProperties(
                        springProperties.stiffness,
                        Vector3.Distance(massPoints[one].position, massPoints[two].position),
                        springProperties.dampingFactor,
                        springProperties.maxStrength,
                        springProperties.squareForce
                        );
                    if(y == 0)
                        newSprings.Add(new Spring(massPoints[one], massPoints[two], properties));

                    properties.restLength = Vector3.Distance(massPoints[two].position, massPoints[four].position);
                    newSprings.Add(new Spring(massPoints[two], massPoints[four], properties));

                    properties.restLength = Vector3.Distance(massPoints[four].position, massPoints[three].position);
                    newSprings.Add(new Spring(massPoints[four], massPoints[three], properties));

                    if(x == 0)
                    {
                        properties.restLength = Vector3.Distance(massPoints[three].position, massPoints[one].position);
                        newSprings.Add(new Spring(massPoints[three], massPoints[one], properties));
                    }
                    

                    properties.restLength = Vector3.Distance(massPoints[one].position, massPoints[four].position);
                    newSprings.Add(new Spring(massPoints[one], massPoints[four], properties));

                    properties.restLength = Vector3.Distance(massPoints[three].position, massPoints[two].position);
                    newSprings.Add(new Spring(massPoints[three], massPoints[two], properties));

                    for(int i = 1; i <= 6; i++)
                    {
                        SetMassPointRadius(newSprings[newSprings.Count - i]);
                    }
                }
            }
            springs = newSprings.ToArray();

            /**
            for (int i = 0; i < SB_Mesh.triangles.Length; i += 3)
            {
                SpringProperties properties = new SpringProperties(
                    springProperties.stiffness,
                    Vector3.Distance(massPoints[SB_Mesh.triangles[i]].position, massPoints[SB_Mesh.triangles[i + 1]].position),
                    springProperties.dampingFactor,
                    springProperties.maxStrength,
                    springProperties.squareForce
                    );
                newSprings.Add( new Spring(massPoints[SB_Mesh.triangles[i]], massPoints[SB_Mesh.triangles[i + 1]], properties));

                properties.restLength = Vector3.Distance(massPoints[SB_Mesh.triangles[i + 1]].position, massPoints[SB_Mesh.triangles[i + 2]].position);
                newSprings.Add( new Spring(massPoints[SB_Mesh.triangles[i + 1]], massPoints[SB_Mesh.triangles[i + 2]], springProperties));


                if (i % 2 == 0)
                {
                    properties.restLength = Vector3.Distance(massPoints[SB_Mesh.triangles[i + 2]].position, massPoints[SB_Mesh.triangles[i]].position);
                    newSprings.Add(new Spring(massPoints[SB_Mesh.triangles[i + 2]], massPoints[SB_Mesh.triangles[i]], springProperties));
                }
            }
            **/
        }
    }

    private void SetMassPointRadius(Spring spring)
    {
        float dis = spring.properties.restLength / 2;
        if (dis < spring.A.radius)
            spring.A.radius = dis;
        if (dis < spring.B.radius)
            spring.B.radius = dis;
    }

    public void AddVertex(int index, Vector3 newPos)
    {
        Vector3[] temp = new Vector3[vertices.Length + 1];
        for(int i = 0; i < temp.Length; i++)
        {
            if(index == i)
            {
                temp[i] = newPos;
            }
            if(index < i)
            {
                temp[i] = vertices[i - 1];
            }
            else
            {
                temp[i] = vertices[i];
            }
        }
    }

    public Vector3[] GetMeshVertices()
    {
        if (!SB_Mesh.initialized)
            GenerateMesh();
        return SB_Mesh.vertices; 
    }

    public int[] GetMeshTriangles()
    {
        if (!SB_Mesh.initialized)
            GenerateMesh();
        return SB_Mesh.triangles;
    }

    public void AddForce(Vector3 force)
    {
        Vector3 forceToAdd = force / massPoints.Length;
        foreach(MassPoint m in massPoints)
        {
            m.AddForce(forceToAdd);
        }
    }

    #endregion
}
