//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This script serves as bouncing movement along one or many axis.
using UnityEngine;
using System.Collections;

public class PingPongMovement : MonoBehaviour {

    //Spawn Movement; credit to 09/07/2016 "Interactive Generic Objects" lecture.
    [SerializeField]
    Vector3 directions = Vector3.zero;

    [SerializeField]
    float moveSpeed = 5.0f;

    Vector3 initPos = Vector3.zero;
    Vector3 offSet = Vector3.zero;

    float timer = 0;

    // Use this for initialization
    void Start () {

        initPos = transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        MovementUpdate();

    }

    //Enemy location spawner
    void MovementUpdate()
    {
        timer += Time.deltaTime * moveSpeed;

        if (directions.x > 0)
        {
            offSet.x = Mathf.PingPong(timer, directions.x);
        }
        if (directions.y > 0)
        {

            offSet.y = Mathf.PingPong(timer, directions.y);

        }
        if (directions.z > 0)
        {

            offSet.z = Mathf.PingPong(timer, directions.z);
        }

        transform.position = initPos + offSet;
    }
}
