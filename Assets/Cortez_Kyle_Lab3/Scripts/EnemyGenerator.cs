//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This is to generate enemies at random from a table. It also has functionality to spawn enemies in bursts
// if the option is checked in the inspector.

using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
    //
    [SerializeField]
    GameObject[] EnemyTypes;

    [SerializeField]
    ShipPlayerController PlayerShip;

   

    //MinMax spawn timer
    [SerializeField]
    float TimeMin = 1.0f;

    [SerializeField]
    float TimeMax = 4.0f;

    private float TimeLimit = 0;

    private float Timer = 0;

    //Spawning burst of enemies
    
    private bool spawnBurst = false;

    [SerializeField]
    public bool canSpawnBurst = false;

    [SerializeField]
    private int numEnemies = 3;

    [SerializeField]
    private float spawnBurstTimer = 10.0f;


    //Boss stuff
    [SerializeField]
    public bool canSpawn = true;

    [SerializeField]
    BossGenerator BossGenerator;

  






    // Use this for initialization
    void Start () {

        spawnBurst = false;

        

        if (PlayerShip.playerAlive == true)
        {
            if (canSpawn == true)
            {

                if (canSpawnBurst == true)
                {
                    StartCoroutine(burstSpawn());
                }
            }

        }





    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerShip.playerAlive == true)
        {
            if (canSpawn == true)
            {
                updateTimer();
            }

            else if (canSpawn == false)
            {
                StopAllCoroutines();
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Start();
        }
    }



    void UpdateSpawner()
    {
                if (PlayerShip.playerAlive == true)
        {
            if (canSpawn == true)
            {

                if (canSpawnBurst == true)
                {
                    StartCoroutine(burstSpawn());
                }
            }

        }
    }


    //Spawning Burst of Enemies
    IEnumerator burstSpawn()
    {
        do
        {
            yield return new WaitForSeconds(spawnBurstTimer);

            Debug.Log("Spawned Burst");
            for (int i = 0; i < numEnemies; i++)
            {
                yield return new WaitForSeconds(0.5f);
                doSpawn();


            }
        } while (canSpawnBurst == true);

    }



        //Random enemy generation

    void updateTimer()
    {
        Timer += Time.deltaTime;

        if (Timer >= TimeLimit)
        {
            TimeLimit = Random.Range(TimeMin, TimeMax);
            Timer = 0;

            if (canSpawnBurst == false && spawnBurst == false)
            {
                doSpawn();
            }
        }
    }

    void doSpawn()
    {
        
            int randIndex = Random.Range(0, EnemyTypes.Length);

            Instantiate(EnemyTypes[randIndex],
                         transform.position, transform.rotation);

            BossGenerator.UpdateSpawn(1);
        

    }


  
}
