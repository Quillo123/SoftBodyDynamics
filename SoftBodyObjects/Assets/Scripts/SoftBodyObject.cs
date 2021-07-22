using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SoftBodyObject : MonoBehaviour
{
    #region Properties
    //SoftBody Properties
    public Vector3[] vertices;
    public float mass = 10;
    public int fillFactor = 1;
    public bool isStatic = false;
    public bool roundForces = true;

    //MassPoint Properties
    public MassPoint.MassPointProperties massPointProperties = new MassPoint.MassPointProperties(1, 10, 2, true, false);

    //Spring Properties
    public Spring.SpringProperties springProperties = new Spring.SpringProperties(1000, 1, 10, 20, true);

    //Arrays
    MassPoint[] massPoints;
    Spring[] springs;

    SoftBodyMesh SB_Mesh;

    //Cached Objects
    MeshFilter meshFilter;

    #endregion

    #region UnityMessages
    private void OnValidate()
    {
        meshFilter = GetComponent<MeshFilter>();

        SB_Mesh = new SoftBodyMesh(meshFilter);
        if (vertices == null)
        {
            vertices = new Vector3[3];
        }
        
    }

    private void Start()
    {
        SB_Mesh.GenerateMeshVerticesFromShape(vertices, fillFactor);
        PopulateMassPoints();
        PopulateSprings();
    }

    
    #endregion



    #region Methods
    public void OnEditorUpdate()
    {

    }

    private void PopulateMassPoints()
    {
        massPoints = new MassPoint[SB_Mesh.vertices.Length];
        for(int i = 0; i < massPoints.Length; i++)
        {
            massPoints[i] = new MassPoint(
                SB_Mesh.vertices[i], 
                mass / massPoints.Length, 
                massPointProperties.radius, 
                massPointProperties.repulsionStrength, 
                massPointProperties.repulsionCurveFactor, 
                roundForces, 
                isStatic);
        }
    }

    private void PopulateSprings()
    {
        springs = new Spring[SB_Mesh.triangles.Length];
        for(int i = 0; i < SB_Mesh.triangles.Length; i+=3)
        {
            springs[i] = new Spring(massPoints[SB_Mesh.triangles[i]], massPoints[SB_Mesh.triangles[i + 1]], springProperties.stiffness, springProperties.restLength, springProperties.dampingFactor, springProperties.maxStrength, springProperties.squareForce);
            springs[i] = new Spring(massPoints[SB_Mesh.triangles[i + 1]], massPoints[SB_Mesh.triangles[i + 2]], springProperties.stiffness, springProperties.restLength, springProperties.dampingFactor, springProperties.maxStrength, springProperties.squareForce);
            springs[i] = new Spring(massPoints[SB_Mesh.triangles[i + 2]], massPoints[SB_Mesh.triangles[i]], springProperties.stiffness, springProperties.restLength, springProperties.dampingFactor, springProperties.maxStrength, springProperties.squareForce);
        }
    }

    #endregion


    #region Structs
    public struct MassPoint
    {
        
        public MassPointProperties properties;

        public Vector3 position;
        public float mass;
        public Vector3 force { get; private set; }
        public Vector3 velocity { get; private set; }


        public MassPoint(Vector3 position, float mass, float radius, float repulsionStrength, int repulsionCurveFactor, bool roundForces = true, bool isStatic = false)
        {
            this.properties = new MassPointProperties(
                radius,
                repulsionStrength,
                repulsionCurveFactor,
                roundForces,
                isStatic
                );

            this.position = position;
            this.mass = mass;
            this.force = Vector3.zero;
            this.velocity = Vector3.zero;
        }

        public MassPoint(Vector3 position, float mass, MassPointProperties properties)
        {
            this.position = position;
            this.mass = mass;
            this.properties = properties;
            this.force = Vector3.zero;
            this.velocity = Vector3.zero;
        }

        public void AddForce(Vector3 force)
        {
            if (Mathf.Abs(force.magnitude) > Universe.precision)
                this.force += force;
        }

        public void OnUpdate(float deltaTime, bool simulateForces = true)
        {
            if (simulateForces && !properties.isStatic)
            {
                velocity += force * deltaTime;
                if (Mathf.Abs(velocity.x) < Universe.precision)
                    velocity = new Vector3(0f, velocity.y, velocity.z);
                if (Mathf.Abs(velocity.y) < Universe.precision)
                    velocity = new Vector3(velocity.x, 0f, velocity.z);

                position += velocity * deltaTime;
            }
            force = Vector3.zero;
        }

        [Serializable]
        public struct MassPointProperties {
            
            public float radius;
            public float repulsionStrength;
            public int repulsionCurveFactor;

            public bool roundForces { get; private set; }
            public bool isStatic { get; private set; }


            public MassPointProperties(float radius, float repulsionStrength, int repulsionCurveFactor, bool roundForces, bool isStatic)
            {
                this.radius = radius;
                this.repulsionStrength = repulsionStrength;
                this.repulsionCurveFactor = repulsionCurveFactor;
                this.roundForces = roundForces;
                this.isStatic = isStatic;
            }
        }
    }

    public struct Spring
    {
        public MassPoint A;
        public MassPoint B;

        SpringProperties properties;

        public Spring(MassPoint A, MassPoint B, float stiffness, float restLength, float dampingFactor, float maxStrength, bool squareForce = true)
        {
            this.A = A;
            this.B = B;
            this.properties = new SpringProperties(
                stiffness,
                restLength,
                dampingFactor,
                maxStrength,
                squareForce
                );
        }

        public Spring(MassPoint A, MassPoint B, SpringProperties properties)
        {
            this.A = A;
            this.B = B;
            this.properties = properties;
        }

        public Vector3 SpringForceA()
        {
            float totalSpringForce = TotalSpringForce();
            return totalSpringForce * (Vector2)(B.position - A.position).normalized;
        }

        public Vector3 SpringForceB()
        {
            float totalSpringForce = TotalSpringForce();
            return totalSpringForce * (Vector2)(A.position - B.position).normalized;
        }

        float TotalSpringForce()
        {
            Vector2 AtoB = (B.position - A.position).normalized;

            Vector2 velocityDiff = B.velocity - A.velocity;

            float dotProd = Vector2.Dot(AtoB, velocityDiff);
            float springForce;

            var diff = (Vector2.Distance(A.position, B.position) - properties.restLength) / properties.restLength;
            if (properties.squareForce)
            {
                if (diff < 0)
                    springForce = -Mathf.Pow(diff, 2) * properties.stiffness;
                else
                    springForce = Mathf.Pow(diff, 2) * properties.stiffness;
            }
            else
            {
                springForce = diff * properties.stiffness;
            }

            float dampForce = dotProd * properties.dampingFactor;
            return Mathf.Clamp(springForce + dampForce, -properties.maxStrength, properties.maxStrength);
        }

        [Serializable]
        public struct SpringProperties
        {
            public float stiffness;
            public float restLength { get; private set; }
            public float dampingFactor;
            public float maxStrength;
            public bool squareForce;

            public SpringProperties(float stiffness, float restLength, float dampingFactor, float maxStrength, bool squareForce = true)
            {
                this.stiffness = stiffness;
                this.restLength = restLength;
                this.dampingFactor = dampingFactor;
                this.maxStrength = maxStrength;
                this.squareForce = squareForce;
            }
        }
    }
    #endregion
}
