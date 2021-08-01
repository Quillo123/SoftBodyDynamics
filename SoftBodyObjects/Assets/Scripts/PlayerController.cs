using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    SoftBodyObject playerSoftBody;

    public float jumpHeight = 10;

    void Start()
    {
        playerSoftBody = GameObject.FindGameObjectWithTag("Player").GetComponent<SoftBodyObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //playerSoftBody.AddForce(Vector3.up * jumpHeight);
        }
    }
}
