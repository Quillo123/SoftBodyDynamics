using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBand : MonoBehaviour
{
    [SerializeField] MassPoint A;
    [SerializeField] MassPoint B;

    LineRenderer line;

    [SerializeField] float stiffness = .1f;

    [SerializeField] float restLength = 1;

    [Range(0, 0.05f)]
    [SerializeField] float dampingFactor = 0.01f;

    private void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.endWidth = 0.01f;
        line.startWidth = 0.01f;
    }

    private void FixedUpdate()
    {
        RenderSpring();

        Vector2 AtoB = (B.transform.position - A.transform.position).normalized;

        Vector2 velocityDiff = B.velocity - A.velocity;

        float dotProd = Vector2.Dot(AtoB, velocityDiff);

        float springForce = (Vector2.Distance(A.transform.position, B.transform.position) - restLength) * stiffness;
        float dampForce = dotProd * dampingFactor;
        float totalSpringForce = springForce + dampForce;

        if(totalSpringForce < 0)
        {
            totalSpringForce = 0;
        }

        A.springForce = totalSpringForce * (Vector2)(B.transform.position - A.transform.position).normalized;
        B.springForce = totalSpringForce * (Vector2)(A.transform.position - B.transform.position).normalized;


    }

    void RenderSpring()
    {
        line.SetPosition(0, A.transform.position);
        line.SetPosition(1, B.transform.position);
    }
}
