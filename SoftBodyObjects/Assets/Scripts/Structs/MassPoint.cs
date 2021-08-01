using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class MassPoint
{

    public MassPointProperties properties;

    public Vector3 position;
    public float mass;
    public float radius;
    public Vector3 force;
    public Vector3 velocity;

    Queue<Vector3> forces = new Queue<Vector3>();

    private MassPoint[] massPoints;


    public MassPoint(Vector3 position, float mass, float radius, float repulsionStrength, int repulsionCurveFactor, bool roundForces = true, bool isStatic = false, MassPoint[] massPoints = null)
    {
        this.properties = new MassPointProperties(
            repulsionStrength,
            repulsionCurveFactor,
            roundForces,
            isStatic
            );

        this.position = position;
        this.mass = mass;
        this.radius = radius;
        this.force = Vector3.zero;
        this.velocity = Vector3.zero;
        this.massPoints = massPoints;
    }

    public MassPoint(Vector3 position, float mass, float radius, MassPointProperties properties, MassPoint[] massPoints = null)
    {
        this.position = position;
        this.mass = mass;
        this.radius = radius;
        this.properties = new MassPointProperties(properties);
        this.force = Vector3.zero;
        this.velocity = Vector3.zero;
        this.massPoints = massPoints;
    }

    public void AddForce(Vector3 force)
    {
        var newForce = new Vector3(
            Mathf.Round(force.x * (1 / Universe.precision)) / (1 / Universe.precision),
            Mathf.Round(force.y * (1 / Universe.precision)) / (1 / Universe.precision));

        this.force += newForce;



        //if (Mathf.Abs(this.force.x) < Universe.precision)
        //    this.force = new Vector3(0f, this.force.y, this.force.z);
        //if (Mathf.Abs(this.force.y) < Universe.precision)
        //    this.force = new Vector3(this.force.x, 0f, this.force.z);
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

            CollisionDetect(deltaTime);

            position += velocity * deltaTime;
        }
        else
        {
            forces.Clear();
        }
        force = Vector3.zero;
    }

    internal void Reflect(Vector3 normal)
    {
        velocity = Vector3.Reflect(velocity, normal);
    }

    private void CollisionDetect(float deltaTime)
    {
        var nextPos = position + velocity * deltaTime;
        foreach (var point in massPoints)
        {
            float dist = Vector3.Distance(nextPos, point.position);
            if (dist < radius * 2 && dist != 0)
            {
                Vector3 dir = (point.position - position).normalized;
                if (dist < radius)
                {
                    velocity = Vector3.Reflect(velocity, dir);
                }
                else
                {
                   /* 
                    var forceToAdd = dir * (Mathf.Pow(dist * mass, properties.repulsionCurveFactor));//  * (velocity.magnitude)) ;// - 1);
                    forceToAdd = new Vector3(Mathf.Clamp(forceToAdd.x, 0, properties.repulsionStrength), Mathf.Clamp(forceToAdd.y, 0, properties.repulsionStrength));
                    if (forceToAdd.magnitude > Universe.precision)
                    {
                        velocity += forceToAdd * deltaTime;
                    }
                   */
                }
            }
        }
    }
}

[Serializable]
public struct MassPointProperties
{

    public float repulsionStrength;
    public int repulsionCurveFactor;

    public bool roundForces { get; private set; }
    public bool isStatic;

    public MassPointProperties(float repulsionStrength, int repulsionCurveFactor, bool roundForces, bool isStatic = false)
    {
        this.repulsionStrength = repulsionStrength;
        this.repulsionCurveFactor = repulsionCurveFactor;
        this.roundForces = roundForces;
        this.isStatic = isStatic;
    }

    public MassPointProperties(MassPointProperties properties)
    {
        this.repulsionStrength = properties.repulsionStrength;
        this.repulsionCurveFactor = properties.repulsionCurveFactor;
        this.roundForces = properties.roundForces;
        this.isStatic = properties.isStatic;
    }
}
