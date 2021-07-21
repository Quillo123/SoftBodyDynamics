using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Spring : MonoBehaviour
{

    public MassPoint A;
    public MassPoint B;

    LineRenderer line;

    public float stiffness = .1f;

    public float restLength = 1;

    //[Range(0.5f, 10f)]
    public float dampingFactor = 0.5f;

    public float maxStrength = 20;

    public bool useSquareForce = true;

    public float precision = 0.01f;

    private void OnValidate()
    {
        line = GetComponent<LineRenderer>();
        line.endWidth = 0.01f;
        line.startWidth = 0.01f;
        RenderSpring();
    }

    public void OnEditorUpdate()
    {
        RenderSpring();
    }

    private void FixedUpdate()
    {
        if (A && B)
        {
            RenderSpring();
            transform.position = A.transform.position;

            Vector2 AtoB = (B.transform.position - A.transform.position).normalized;

            Vector2 velocityDiff = B.velocity - A.velocity;

            float dotProd = Vector2.Dot(AtoB, velocityDiff);
            float springForce;

            var diff = (Vector2.Distance(A.transform.position, B.transform.position) - restLength) / restLength;
            if (useSquareForce)
            {
                if (diff < 0) 
                    springForce = -Mathf.Pow(diff, 2) * stiffness;// * (A.mass);
                else
                    springForce = Mathf.Pow(diff, 2) * stiffness;
            }
            else
            {
                springForce = diff * stiffness;// * (A.mass);
            }

            float dampForce = dotProd * dampingFactor;
            float totalSpringForce = Mathf.Clamp(springForce + dampForce, -maxStrength, maxStrength);

            if(Mathf.Abs(totalSpringForce) > precision)
            {
                Vector2 springForceA = totalSpringForce * (Vector2)(B.transform.position - A.transform.position).normalized;// * .7f;
                Vector2 springForceB = totalSpringForce * (Vector2)(A.transform.position - B.transform.position).normalized;// * .7f;

                A.AddSpringForce(springForceA);
                B.AddSpringForce(springForceB);
            }  
        }
    }

    void RenderSpring()

    {
        if (!line)
        {
            line = GetComponent<LineRenderer>();
        }
        if(A && B)
        {
            line.SetPosition(0, A.transform.position);
            line.SetPosition(1, B.transform.position);
        }
    }

    public void PrepSpring(float stiff, float rLength, float dFactor)
    {
        stiffness = stiff;
        restLength = rLength;
        dampingFactor = dFactor;
    }

    public void SetMassPoints(MassPoint a, MassPoint b)
    {
        A = a;
        B = b;
    }
}
