using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class Spring
{
    public MassPoint A;
    public MassPoint B;

    public SpringProperties properties;

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
        float springForce = 0;
        if (properties.restLength != 0)
        {
            var dist = Vector2.Distance(A.position, B.position);
            var diff = (dist - properties.restLength) / properties.restLength;
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
        else
        {
            return 0;
        }

    }

    
}

[Serializable]
public struct SpringProperties
{
    public float stiffness;
    public float restLength;
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
