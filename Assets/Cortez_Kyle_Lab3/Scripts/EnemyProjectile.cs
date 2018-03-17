//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: Much like the player's projectile script, this serves to control the behaviours of all enemy projectiles.
using UnityEngine;
using System.Collections;

public class EnemyProjectile : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    
    private ShipPlayerController ParentShip;

  
    private HUD PlayerScore;


    //Particles
    [SerializeField]
    GameObject HitParticles;

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

    //So audio does not stop
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

        

    }

    //Destroys projectile after 1 second when boundary is hit.
    void OnTriggerExit(Collider entity)
    {
        if (entity.tag == "Bounds")
        {
            Destroy(gameObject, 1.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerShip")
        {
            Debug.Log("Hit a Player!");

            Destroy(gameObject,2.0f);
            
            //audio
            StartDestroy(shotSfxClip.length);

            ParentShip.ModScore(-1);

            ParentShip.ModShield(-1);

            SpawnHitParticles();


        }
    }

    void SpawnHitParticles()
    {
        Instantiate(HitParticles, transform.position, transform.rotation);
    }

}

