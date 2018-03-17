//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/18/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This script is set to count the amount of spawns from the normal generators to make a boss spawn.


using UnityEngine;
using System.Collections;

public class BossGenerator : MonoBehaviour
{

    //
    [SerializeField]
    GameObject EnemyType;

    [SerializeField]
    ShipPlayerController PlayerShip;








    //Boss stuff


    [SerializeField]
    int maxSpawns = 30;

    [SerializeField]
    int curSpawns = 0;

    [SerializeField]
    EnemyGenerator Generator;

    [SerializeField]
    EnemyGenerator GeneratorLeft;

    [SerializeField]
    EnemyGenerator GeneratorRight;




    // Use this for initialization
    void Start()


    {

       

    }

    // Update is called once per frame
    void Update()
    {


        if(Input.GetKeyDown(KeyCode.R))
        {
            curSpawns = 0;

            Generator.canSpawn = true;
            GeneratorLeft.canSpawn = true;
            GeneratorRight.canSpawn = true;

            
            GeneratorLeft.canSpawnBurst = true;
            GeneratorRight.canSpawnBurst = true;
        }








    }

    //Boss Stuff
    public void UpdateSpawn(int value)
    {
        if (curSpawns < maxSpawns)
        {
            curSpawns += value;
        }

        if (curSpawns >= maxSpawns)
        {
            doSpawn();
            Generator.canSpawn = false;
            GeneratorLeft.canSpawn = false;
            GeneratorRight.canSpawn = false;



            Debug.Log("Spawned Max amount of enemies, Boss Spawns!");
        }
    }






    



    void doSpawn()
    {
        

        Instantiate(EnemyType,
                     transform.position, transform.rotation);

        

    }



}
