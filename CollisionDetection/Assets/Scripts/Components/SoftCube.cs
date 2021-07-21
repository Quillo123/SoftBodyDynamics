using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftCube : MonoBehaviour
{

    public List<List<MassPoint>> mPoints;
    public List<Spring> springs;

    public int size = 2;
    public float stiffness = 50;
    public float restLength = 0.5f;
    //[Range(0.5f, 10f)]
    public float dampingFactor = 1;
    public float massPointRadius = 0.2f;
    public float mass = 1;

    public bool isFixed = false;

    public Sprite massPointSprite;

    private void OnValidate()
    {
        mPoints = new List<List<MassPoint>>(size);
        springs = new List<Spring>();
        for(int i = 0; i < size; i++)
        {
            mPoints.Insert(i, new List<MassPoint>(size));
        }
    }


    void Update()
    {
        OnUpdate();
    }

    public void OnUpdate() {
        
    }

    public void OnEditorUpdate()
    {
        if(mPoints[0].Count != size)
        {
            GenerateCube(DestroyImmediate, true);
        }
        foreach(Spring spring in springs)
        {
            spring.OnEditorUpdate();
        }
        CheckChanges();
    }

    public void CheckChanges()
    {
        if(GetComponentInChildren<MassPoint>())
        {
            if (mPoints[0].Count != 0)
            {
                
                if (
                    mPoints[0][0].isFixed != isFixed ||
                    mPoints[0][0].mass != mass ||
                    mPoints[0][0].radius != massPointRadius
                )
                {
                    Debug.Log("Changing MassPoint Properties");
                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            mPoints[x][y].isFixed = isFixed;
                            mPoints[x][y].mass = mass;
                            mPoints[x][y].radius = massPointRadius;
                        }
                    }
                }
            }
        }
        if (GetComponentInChildren<Spring>())
        {
            if (springs.Count != 0)
            {
                if (
                    springs[0].stiffness != stiffness || 
                    springs[0].restLength != restLength || 
                    springs[0].dampingFactor != dampingFactor
                )
                {
                    Debug.Log("Changing Spring Properties");
                    foreach (Spring spring in springs)
                    {
                        spring.stiffness = stiffness;
                        //spring.restLength = restLength;
                        spring.dampingFactor = dampingFactor;
                    }
                }
            }
        }
        
        
    }

    public void GenerateCube(System.Action<Object> DestroyObject, bool regenerate = false)
    {
        if(regenerate || transform.GetComponentsInChildren<MassPoint>().Length != size * size)
        {
            List<MassPoint> massPoints = new List<MassPoint>();
            for (int i = transform.childCount; i > 0; i--)
            {
                GameObject child = transform.GetChild(0).gameObject;
                if (child.GetComponent<MassPoint>() || child.GetComponent<Spring>())
                {
                    DestroyObject(child);
                }
                for(int j = 0; j < size; j++)
                {
                    if (mPoints[j] == null)
                    {
                        mPoints[j] = new List<MassPoint>();
                    }
                    else
                    {
                        mPoints[j].Clear();
                    }
                }
            }
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    System.Type[] t = { typeof(MassPoint), typeof(SpriteRenderer) };
                    //GameObject m = new GameObject("Point[" + x + ',' + y + "] ", t);
                    GameObject m = Resources.Load<GameObject>("Prefabs/MassPoint");
                    m.GetComponent<SpriteRenderer>().sprite = massPointSprite;
                    MassPoint massPoint = Instantiate(m, transform).GetComponent<MassPoint>();
                    massPoint.transform.position = new Vector3(
                        transform.position.x + restLength * x,
                        transform.position.y + restLength * y,
                        transform.position.z);
                    massPoint.radius = massPointRadius;
                    massPoint.mass = mass;

                    //GameObject spring = new GameObject("Spring[" + x + "," + y + "]", typeof(Spring));
                    GameObject spring = Resources.Load<GameObject>("Prefabs/Spring");

                    if (x == 0)
                    {
                        if (y != 0)
                        {
                            Spring s = Instantiate(spring, transform).GetComponent<Spring>();
                            s.PrepSpring(stiffness, restLength, dampingFactor);
                            s.SetMassPoints(massPoint, mPoints[x][y - 1]);
                        }
                    }
                    else
                    {
                        Spring[] s = new Spring[4];
                        s[0] = Instantiate(spring, transform).GetComponent<Spring>();
                        s[1] = Instantiate(spring, transform).GetComponent<Spring>();

                        


                        springs.Add(s[0]);
                        springs.Add(s[1]);

                        if (y == 0)
                        {
                            s[0].SetMassPoints(massPoint, mPoints[x - 1][y]);
                            s[0].restLength = restLength;

                            s[1].SetMassPoints(massPoint, mPoints[x - 1][y + 1]);
                            s[1].restLength = Mathf.Sqrt(2 * Mathf.Pow(restLength, 2));

                            springs.Remove(s[2]);
                            springs.Remove(s[3]);
                        }
                        else
                        {
                            s[2] = Instantiate(spring, transform).GetComponent<Spring>();
                            springs.Add(s[2]);


                            s[0].SetMassPoints(massPoint, mPoints[x - 1][y]);
                            s[0].restLength = restLength;

                            s[1].SetMassPoints(massPoint, mPoints[x][y - 1]);
                            s[1].restLength = restLength;

                            s[2].SetMassPoints(massPoint, mPoints[x - 1][y - 1]);
                            s[2].restLength = Mathf.Sqrt(2 * Mathf.Pow(restLength, 2));
                            if(y != size - 1)
                            {
                                s[3] = Instantiate(spring, transform).GetComponent<Spring>();
                                springs.Add(s[3]);

                                s[3].SetMassPoints(massPoint, mPoints[x - 1][y + 1]);
                                s[3].restLength = Mathf.Sqrt(2 * Mathf.Pow(restLength, 2));
                            }
                        }
                    }
                    massPoints.Add(massPoint);
                    massPoint.SetMassPointCollisionList(massPoints);
                    mPoints[x].Insert(y, massPoint);
                }
            }
        }
    }
    
    private void HandleInternalCollisions()
    {

    }
}
