//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This is a very modular script that is the base for all pickups in the game.
using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    
    float MoveSpeed = 2.0f;

    [SerializeField]
    string subTypeName = "Unknown Pickup";


    enum AIMode { Normal,  MoveTowards };

    private AIMode CurrentMoveState;

    private ShipPlayerController PlayerShipCtrl;





    // Use this for initialization
    void Start () {

        //movestate
        CurrentMoveState = AIMode.Normal;

        GetComponent<Collider>().name = subTypeName;

        //playership component
        GameObject playerShip = GameObject.Find("PlayerShip");

        PlayerShipCtrl = playerShip.GetComponent<ShipPlayerController>();

        
        
	
	}

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Bounds")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        transform.position += transform.up * Time.deltaTime * MoveSpeed;

        DetermineMoveState();


        switch (CurrentMoveState)
        {
            case AIMode.Normal:
                UpdateNormal();
                break;

            case AIMode.MoveTowards:
                UpdateMoveTowards();
                break;


        }

    }


    void UpdateNormal()
    {
        transform.position += transform.up * Time.deltaTime * MoveSpeed;

    }

    void UpdateMoveTowards()
    {
        Vector3 directionToPlayer = PlayerShipCtrl.transform.position - transform.position;

        transform.up = Vector3.MoveTowards(transform.up, directionToPlayer, MoveSpeed * 2);

        transform.position += transform.up * Time.deltaTime * MoveSpeed * 2;
    }





    void DetermineMoveState()
    {
        //for moving towards player
        Vector3 directionToPlayer = PlayerShipCtrl.transform.position - transform.position;

        Vector3 DirToPlayerNorm = directionToPlayer.normalized;

        float product = Vector3.Dot(transform.up, DirToPlayerNorm);

        float angle = Mathf.Acos(product);

        angle = angle * Mathf.Rad2Deg;



        if (Input.GetKey(KeyCode.C))
        {

            CurrentMoveState = AIMode.MoveTowards;
            GetComponent<Renderer>().material.color = Color.green;

        }

        else
        {
            CurrentMoveState = AIMode.Normal;
            GetComponent<Renderer>().material.color = Color.red;
        }

    }
}
