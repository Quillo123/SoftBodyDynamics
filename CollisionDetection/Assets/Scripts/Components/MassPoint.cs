using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MassPoint : MonoBehaviour
{
    public static Universe universe;

    public Vector2 velocity = new Vector2();
    public Vector2 force = new Vector2();
    public Vector2 springForce = new Vector2();
    public float mass = 1f;
    public float radius = 0.1f;

    public bool isFixed = false;
    public bool isReflective = false;
    public float maxRepulsiveforce = 20;

    [SerializeField] LayerMask collisionMask;

    public Vector2 upperBound;
    public Vector2 lowerBound;

    private List<MassPoint> massPoints;

    private void OnValidate()
    {
        universe = FindObjectOfType<Universe>();
    }

    private void Start()
    {
        transform.tag = "MassPoint";
    }

    private void FixedUpdate()
    {
        if (mass != 0 && isFixed == false)
        {
            Vector2 gForce = universe.gConst * mass;

            force += gForce;

            force += springForce;// / mass;

            CollisionDetect();

            //if (force.magnitude < 0.001)
            //    force = Vector2.zero;

            velocity += (force * Time.deltaTime);

            //Vector3 moveAmount = new Vector3(velocity.x, velocity.y) * Time.deltaTime;

            transform.position += new Vector3(velocity.x, velocity.y) * Time.deltaTime;
        }
        force = Vector2.zero;
        springForce = Vector2.zero;
    }

    private void CollisionDetect()
    {
        //var massPoints = FindObjectsOfType<MassPoint>();
        foreach (var point in massPoints)
        {
            float dist = Vector2.Distance(transform.position, point.transform.position);
            if(dist < radius * 2 && dist != 0)
            {
                Vector3 dir = (point.transform.position - transform.position).normalized;
                if (dist < radius)
                {
                    velocity = Vector2.Reflect(velocity, dir);
                }
                else
                {
                    var forceToAdd = (Vector2)dir * (1 / Mathf.Pow(dist/radius, 2) /* velocity.magnitude*/ - 1);
                    forceToAdd = new Vector2(Mathf.Clamp(forceToAdd.x, -maxRepulsiveforce, maxRepulsiveforce), Mathf.Clamp(forceToAdd.y, -maxRepulsiveforce, maxRepulsiveforce));
                    if(forceToAdd.magnitude > 0.01)
                    {
                        force += forceToAdd;
                    }
                }  
            }
        }
    }

    internal void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }

    internal Vector2 GetVelocity()
    {
        return velocity;
    }



    public void AddSpringForce(Vector2 sForce)
    {
        springForce += sForce;
    }

    public void SetMassPointCollisionList(List<MassPoint> list)
    {
        massPoints = list;
    }
}


