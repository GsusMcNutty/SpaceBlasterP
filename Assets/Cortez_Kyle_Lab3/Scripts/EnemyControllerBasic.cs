//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
// Credit: For Mathf.Sin I used robertbu's reply on answers.unity3d to aid in figuring out how to impliment it into the movement pattern.
// Credit: Link: http://answers.unity3d.com/questions/803434/how-to-make-projectile-to-shoot-in-a-sine-wave-pat.html
// Credit: Caching start position BMayne unity3d answers
// Credit: http://answers.unity3d.com/questions/781748/using-mathfsin-to-move-an-object.html
//
// Purpose: This one is designed to control weak, basic enemies and is the base for the other enemy types.
// Purpose: Updated to include AI. Only set to steer and ram.
using UnityEngine;
using System.Collections;

public class EnemyControllerBasic : MonoBehaviour {

    [SerializeField]
    GameObject[] PickupTypes;

    private ShipPlayerController PlayerShipCtrl;

    //Enemy move speed

    [SerializeField]
    float MoveSpeed = 5.0f;

   
    //bool waveMove = false;
    
    //bool vanillaMove = true;

    ////waving function (credit below)
    //[SerializeField]
    //private float freq = 10.0f;
    //[SerializeField]
    //private float magn = 0.5f;

    //private Vector3 startPos;

    //private Vector3 axis;

    // Point Value
    [SerializeField]
    private int pointValue = 5;

    //AI work 

    [SerializeField]
    float RamSpeed = 5.0f;

    enum AIMode { Normal, Ramming, SteerTowards};

    private AIMode CurrentAIState;



    //Particles
    [SerializeField]
    GameObject ShipExplosion;

    //audio
    [SerializeField]
    AudioClip shipDestroySfx;






    // Use this for initialization
    void Start () {
        //AI Start
        CurrentAIState = AIMode.Normal;


        GameObject playerShip = GameObject.Find("PlayerShip");

        PlayerShipCtrl = playerShip.GetComponent<ShipPlayerController>();

        

        //axis = transform.right;

        
	
	}


    void StartDestroy(float timeDelay)
    {

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        



        Destroy(gameObject, timeDelay);

    }


    // Update is called once per frame
    void Update () {

        if (PlayerShipCtrl.playerAlive == true)
        {

            //DetermineAI state

            DetermineAIState();

            

            //AI Switching
            switch (CurrentAIState)
            {
                case AIMode.Normal: UpdateNormal();
                    break;

                case AIMode.Ramming: UpdateRamming();
                    break;

                case AIMode.SteerTowards: UpdateSteerTowards();
                    break;

                default:
                    Debug.Log("Unknown AI State" + CurrentAIState);
                    break;
                
              
            }

        
        }

        else if (PlayerShipCtrl.playerAlive == false)
        {
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerProjectile")
        {
            StartDestroy(shipDestroySfx.length);

            SpawnPickup();

            PlayerShipCtrl.ModScore(pointValue);

            SpawnExplosionParticles();


            GetComponent<AudioSource>().PlayOneShot(shipDestroySfx);

        }
        else if (other.tag == "PlayerShip")
        {
            StartDestroy(shipDestroySfx.length);

            SpawnExplosionParticles();

            PlayerShipCtrl.ModScore(-1);

            PlayerShipCtrl.ModShield(-1);


            GetComponent<AudioSource>().PlayOneShot(shipDestroySfx);
        }
    }

    //destroy after out of play area
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Bounds")
        {
            Destroy(gameObject);
        }
    }

    void SpawnPickup()
    {
        int randIndex = Random.Range(0, PickupTypes.Length);

        Instantiate(PickupTypes[ randIndex], transform.position, transform.rotation);
    }

    
    
    //AI Work

    void UpdateNormal()
    {
        transform.position += transform.up * Time.deltaTime * MoveSpeed;

    }

    void UpdateRamming()
    {
        transform.position += transform.up * Time.deltaTime * RamSpeed;

    }

    void UpdateSteerTowards()
    {
        Vector3 directionToPlayer = PlayerShipCtrl.transform.position - transform.position;

        transform.up = Vector3.RotateTowards(transform.up, directionToPlayer, Time.deltaTime * MoveSpeed, 0.0f);
    }







    //AI
    bool CanSeeTarget(string _targetTag)
    {
        RaycastHit hitInfo;

        bool hitAny = Physics.Raycast(transform.position, transform.up, out hitInfo);

        if (hitAny)
        {
            if (hitInfo.collider.gameObject.tag == _targetTag)
            {
                return true;

            }
               

        }

        return false;

    }


    




    // AI State
    void DetermineAIState()
    {

        //For Steering toward player
        Vector3 directionToPlayer = PlayerShipCtrl.transform.position - transform.position;

        Vector3 DirToPlayerNorm = directionToPlayer.normalized;

        float product = Vector3.Dot(transform.up, DirToPlayerNorm);

        float angle = Mathf.Acos(product);

        angle = angle * Mathf.Rad2Deg;



        bool canSee = CanSeeTarget("PlayerShip");

        

        // Ram only
        if (canSee)
        {
            CurrentAIState = AIMode.Ramming;

            GetComponent<Renderer>().material.color = Color.red;
            
        }

        else if (product > 0 && angle < 90)
        {
            CurrentAIState = AIMode.SteerTowards;

            GetComponent<Renderer>().material.color = Color.green;
        }

        else
        {
            CurrentAIState = AIMode.Normal;

            GetComponent<Renderer>().material.color = Color.white;
        }


    }


    //Particles

    void SpawnExplosionParticles()
    {
        Instantiate(ShipExplosion, transform.position, transform.rotation);
    }


}
