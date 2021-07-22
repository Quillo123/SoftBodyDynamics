using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SoftBodyObject : MonoBehaviour
{
   



    public struct MassPoint
    {
        const float PRECISION = 0.001f;
        bool roundForces;

        Vector3 position;
        Vector3 force;
        Vector3 velocity;
        float mass;
        bool isStatic;

        public MassPoint(Vector3 position, float mass, bool roundForces = true, bool isStatic = false)
        {
            this.position = position;
            this.mass = mass;
            this.roundForces = roundForces;
            this.isStatic = isStatic;
            this.force = Vector3.zero;
            this.velocity = Vector3.zero;
        }

        public void AddForce(Vector3 force)
        {
            if(Mathf.Abs(force.magnitude) > PRECISION)
                this.force += force;
        }

        public void OnUpdate(float deltaTime, bool simulateForces = true)
        {
            if(simulateForces && !isStatic)
            velocity += force * deltaTime;
            if (Mathf.Abs(velocity.x) < PRECISION)
                velocity.x = 0;
            if (Mathf.Abs(velocity.y) < PRECISION)
                velocity.y = 0;
            position += velocity * deltaTime;
        }
    }
}
