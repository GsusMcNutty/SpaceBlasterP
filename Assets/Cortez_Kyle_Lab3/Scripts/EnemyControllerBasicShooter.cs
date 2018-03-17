//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/18/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
// Credit: For Mathf.Sin I used robertbu's reply on answers.unity3d to aid in figuring out how to impliment it into the movement pattern.
// Credit: Link: http://answers.unity3d.com/questions/803434/how-to-make-projectile-to-shoot-in-a-sine-wave-pat.html
// Credit: Caching start position BMayne unity3d answers
// Credit: http://answers.unity3d.com/questions/781748/using-mathfsin-to-move-an-object.html
//
// Purpose: This one is designed to control weak, basic enemies and is the base for the other enemy types.
// Purpose: Updated to include AI. Only set to shoot once, then steer and ram. This enemy can dodge.

using UnityEngine;
using System.Collections;

public class EnemyControllerBasicShooter : MonoBehaviour {

    [SerializeField]
    GameObject[] PickupTypes;

    private ShipPlayerController PlayerShipCtrl;

    //Enemy move speed

    [SerializeField]
    float MoveSpeed = 5.0f;




    

    // Point Value
    [SerializeField]
    private int pointValue = 5;

    //AI work 

    [SerializeField]
    float RamSpeed = 5.0f;

    enum AIMode { Normal, Ramming, SteerTowards, Dodge};

    private AIMode CurrentAIState;





    //firing


    [SerializeField]
    public EnemyProjectile Bullet;

    [SerializeField]
    private float FireRate = 0.4f;


    
    //Particles
    [SerializeField]
    GameObject ShipExplosion;


    //audio
    [SerializeField]
    AudioClip shipDestroySfx;





    // Use this for initialization
    void Start()
    {
        //AI Start
        CurrentAIState = AIMode.Normal;



        

        GameObject playerShip = GameObject.Find("PlayerShip");

        PlayerShipCtrl = playerShip.GetComponent<ShipPlayerController>();

        //ChooseMovement();

        

        //startPos = transform.position;

        StartCoroutine(Firing());

    }


    void StartDestroy(float timeDelay)
    {

        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;




        Destroy(gameObject, timeDelay);

    }


    // Update is called once per frame
    void Update()
    {

        if (PlayerShipCtrl.playerAlive == true)
        {

            //DetermineAI state

            DetermineAIState();



            //Determine MovePattern
            //if (waveMove == false && vanillaMove == true)
            //{

            //}
            //else if (waveMove == true && vanillaMove == false)
            //{

            //    startPos += transform.up * Time.deltaTime * MoveSpeed;
            //    transform.position = startPos + axis * Mathf.Sin(Time.time * freq) * magn;
            //}

            //AI Switching
            switch (CurrentAIState)
            {
                case AIMode.Normal:
                    UpdateNormal();
                    break;
                    
                case AIMode.Ramming:
                    UpdateRamming();
                    break;

                case AIMode.SteerTowards:
                    UpdateSteerTowards();
                    break;

                case AIMode.Dodge:
                    UpdateDodge();
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


    //firing

    IEnumerator Firing()
    {


        yield return new WaitForSeconds(FireRate);

        EnemyFire();
        

        yield return new WaitForSeconds(FireRate);
       

    }


    void EnemyFire()
    {

        Instantiate(Bullet, transform.position, transform.rotation);


    }






    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerProjectile")
        {
            

            SpawnExplosionParticles();

            SpawnPickup();

            PlayerShipCtrl.ModScore(pointValue);

            StartDestroy(shipDestroySfx.length);

            GetComponent<AudioSource>().PlayOneShot(shipDestroySfx);

        }
        else if (other.tag == "PlayerShip")
        {
            

            SpawnExplosionParticles();

            PlayerShipCtrl.ModScore(-1);

            PlayerShipCtrl.ModShield(-1);

            StartDestroy(shipDestroySfx.length);

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

        Instantiate(PickupTypes[randIndex], transform.position, transform.rotation);
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

    // for dodging
    void UpdateDodge()
    {
        Vector3 directionToPlayer = -PlayerShipCtrl.transform.position - transform.position;

        transform.up = Vector3.RotateTowards(-transform.up, -directionToPlayer, Time.deltaTime * MoveSpeed, 0.0f);

        transform.position += transform.up * Time.deltaTime * RamSpeed;

        StartCoroutine(AboutFace());
    }



    //turns the enemy back around after dodging
    IEnumerator AboutFace()
    {

        Vector3 directionToPlayer = -PlayerShipCtrl.transform.position - transform.position;

        yield return new WaitForSeconds(0.5f);

        transform.up = Vector3.RotateTowards(-transform.up, directionToPlayer, Time.deltaTime * MoveSpeed, 0.0f);

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

        bool canSeeBullet = CanSeeTarget("PlayerProjectile");


        if (canSee)
        {
            if (canSeeBullet == true)
            {
                CurrentAIState = AIMode.Dodge;

                GetComponent<Renderer>().material.color = Color.blue;

            }

            else if (product > 0 && angle < 90)
            {

                CurrentAIState = AIMode.Ramming;

                GetComponent<Renderer>().material.color = Color.red;
            }

        }



        else if (product > 0 && angle < 90)
        {
            if (canSeeBullet == true)
            {
                CurrentAIState = AIMode.Dodge;

                GetComponent<Renderer>().material.color = Color.blue;

            }

            else if (product > 0 && angle < 90)
            {
                CurrentAIState = AIMode.SteerTowards;

                GetComponent<Renderer>().material.color = Color.green;
            }
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
