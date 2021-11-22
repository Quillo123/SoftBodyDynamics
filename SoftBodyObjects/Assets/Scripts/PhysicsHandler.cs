using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PhysicsHandler : MonoBehaviour
{

    SoftBodyObject[] softBodies;
    RigidPolygon[] rigidBodies;

    [Range(1, 10)]
    public float gameSpeed = 1;

    void Start()
    {
        softBodies = FindObjectsOfType<SoftBodyObject>();
        rigidBodies = FindObjectsOfType<RigidPolygon>();
        foreach (RigidPolygon rb in rigidBodies)
        {
            rb.OnStart();
        }

        foreach (SoftBodyObject sb in softBodies)
        {
            sb.OnStart();
        }

        StartCoroutine(StartPhysicsSimulation());
    }

    IEnumerator StartPhysicsSimulation()
    { 
        while (true)
        {
            foreach (RigidPolygon rb in rigidBodies)
            {
                rb.PhysicsUpdate(softBodies);
            }

            

            foreach (SoftBodyObject sb in softBodies)
            {
                sb.PhysicsUpdate();
            }

            //Debug.Log("Physics Update Completed");
            yield return new WaitForSeconds((1 / gameSpeed) * Time.fixedDeltaTime);
        }
    }
}
