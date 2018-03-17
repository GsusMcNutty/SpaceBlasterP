//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This script is designed as the player's projectile and its functions.
using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    public GameObject Boundary;

    private ShipPlayerController ParentShip;

    private HUD PlayerScore;

    //Particles
    [SerializeField]
    GameObject HitParticles;

    [SerializeField]
    GameObject TrailParticles;


    //Audio

    [SerializeField]
    AudioClip shotSfxClip;

    [SerializeField]
    Renderer ShotParticleRenderer;



    // Use this for initialization
    void Start()
    {
        GameObject playerShip = GameObject.Find("PlayerShip");

        ParentShip = playerShip.GetComponent<ShipPlayerController>();

        GetComponent<AudioSource>().PlayOneShot(shotSfxClip);

        ShotParticleRenderer.enabled = true;

    }

    void StartDestroy(float timeDelay)
    {

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        ShotParticleRenderer.enabled = false;
        


        Destroy(gameObject, timeDelay);

    }


    // Update is called once per frame
    void Update()
    {

        transform.position += transform.up * Time.deltaTime * MoveSpeed;

        UpdateTrailParticles();
    }


    void UpdateTrailParticles()
    {
        SpawnTrailParticles();
        
    }

    //Destroys projectile after 1 second when boundary is hit.
    void OnTriggerExit (Collider entity)
    {
        if (entity.tag == "Bounds")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if(other.tag == "EnemyShip")
        {
            Debug.Log("Hit an Enemy!");
            Destroy(gameObject, 2.0f);

           

            ParentShip.ModScore(1);

            ParentShip.ModEnemyHits(1);

            //particle
            SpawnHitParticles();


            //audio
            StartDestroy(shotSfxClip.length);     

            
        }
    }

    void SpawnHitParticles()
    {
        Instantiate(HitParticles, transform.position, transform.rotation);
    }


    void SpawnTrailParticles()
    {
        Instantiate(TrailParticles, transform.position, transform.rotation);

        
    }
}
