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
// Purpose: This builds upon the basic enemy script and adds more functions like shields and firing projectiles at the player to
// Purpose: create a tougher enemy.
// Purpose: Updated with AI. This enemy coasts forward while firing and avoids shots when possible.
// Purpose: Once ammo is expended, it will charge up a boost forward in the direction it is facing.

using UnityEngine;
using System.Collections;

public class EnemyControllerFighter : MonoBehaviour
{
    [SerializeField]
    GameObject[] PickupTypes;

    private ShipPlayerController PlayerShipCtrl;

    //Enemy move speed

    [SerializeField]
    float MoveSpeed = 5.0f;


    //bool waveMove = false;

    //bool vanillaMove = true;

    //waving function (credit below)
    //[SerializeField]
    //private float freq = 10.0f;
    //[SerializeField]
    //private float magn = 0.5f;

    //private Vector3 startPos;

    //private Vector3 axis;

    //firing
    bool canFire = false;

    [SerializeField]
    public EnemyProjectile Bullet;

    [SerializeField]
    private float FireRate = 1.0f;

    [SerializeField]
    private int ammoCount = 3;

    //FighterShield
    [SerializeField]
    private int MaxEShield = 2;
    [SerializeField]
    private int CurEShield = 2;

    // Point Value
    [SerializeField]
    private int pointValue = 10;



    //AI work 

    [SerializeField]
    float CoastSpeed = 2.0f;

    float ChargeSpeed = 7.0f;

    enum AIMode { Normal, Coasting, SteerTowards, Dodge, Charge };

    private AIMode CurrentAIState;


    //Particles
    [SerializeField]
    GameObject ShipExplosion;


    //audio
    [SerializeField]
    AudioClip shipDestroySfx;

    [SerializeField]
    AudioClip shieldHitSfx;






    // Use this for initialization
    void Start()
        {

        //AI Start
        CurrentAIState = AIMode.Normal;

        canFire = true;


        CurEShield = MaxEShield;


        GameObject playerShip = GameObject.Find("PlayerShip");

        PlayerShipCtrl = playerShip.GetComponent<ShipPlayerController>();



        //ChooseMovement();



        //axis = transform.right;

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

            DetermineAIState();


            switch (CurrentAIState)
            {
                case AIMode.Normal:
                    UpdateNormal();
                    break;

                case AIMode.Coasting:
                    UpdateCoasting();
                    break;

                case AIMode.SteerTowards:
                    UpdateSteerTowards();
                    break;

                case AIMode.Dodge:
                    UpdateDodge();
                    break;

                case AIMode.Charge:
                    UpdateCharge();
                    break;

                default:
                    Debug.Log("Unknown AI State" + CurrentAIState);
                    break;


            }

            //if (waveMove == false && vanillaMove == true)
            //{
            //    transform.position += transform.up * Time.deltaTime * MoveSpeed;
            //}
            //else if (waveMove == true && vanillaMove == false)
            //{

            //    startPos += transform.up * Time.deltaTime * MoveSpeed;
            //    transform.position = startPos + axis * Mathf.Sin(Time.time * freq) * magn;
            //}
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

        ChooseFire();
        if (canFire == true)
        {
            EnemyFire();
            ammoCount--;
        }

        if (ammoCount >= 1)
        {
            StartCoroutine(Firing());
        }

        else
        {
            
            Debug.Log("NPC out of AMMO!");
        }

        



    }


    void EnemyFire()
    {
        Instantiate(Bullet, transform.position, transform.rotation);
    }


    //destroy on exit of boundary
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Bounds")
        {
            Destroy(gameObject);
        }
    }




    public void ModShield(int value)
    {
        if (CurEShield >= 1)
        {
            CurEShield += value;

            GetComponent<AudioSource>().PlayOneShot(shieldHitSfx);

        }

        if (CurEShield == 0)
        {

            

            SpawnExplosionParticles();

            SpawnPickup();

            PlayerShipCtrl.ModScore(pointValue);

            StartDestroy(shipDestroySfx.length);

            GetComponent<AudioSource>().PlayOneShot(shipDestroySfx);

        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerProjectile")
        {

            ModShield(-1);




        }
        else if (other.tag == "PlayerShip")
        {
            

            PlayerShipCtrl.ModScore(-1);

            PlayerShipCtrl.ModShield(-1);

            SpawnExplosionParticles();

            StartDestroy(shipDestroySfx.length);

            GetComponent<AudioSource>().PlayOneShot(shipDestroySfx);


        }
    }

    void SpawnPickup()
    {
        int randIndex = Random.Range(0, PickupTypes.Length);

        Instantiate(PickupTypes[randIndex], transform.position, transform.rotation);
    }

    //void ChooseMovement()
    //{
    //    int randMoveIndex = Random.Range(0, 2);

    //    if (randMoveIndex == 0)
    //    {
    //        waveMove = true;
    //        vanillaMove = false;

    //    }
    //    if (randMoveIndex == 1)
    //    {
    //        waveMove = false;
    //        vanillaMove = true;

    //    }
    //}




    void ChooseFire()
    {
        int randFireIndex = Random.Range(0, 2);

        if (randFireIndex == 0)
        {

            canFire = true;

        }
        if (randFireIndex == 1)
        {
        
            
            canFire = false;

        }
    }

    //AI Work

    void UpdateNormal()
    {
        transform.position += transform.up * Time.deltaTime * MoveSpeed;

    }


    void UpdateCoasting()
    {
        transform.position += transform.up * Time.deltaTime * CoastSpeed;
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

        transform.position += transform.up * Time.deltaTime * MoveSpeed;

        StartCoroutine(AboutFace());
    }


    //turns the enemy back around after dodging
    IEnumerator AboutFace()
    {

        Vector3 directionToPlayer = -PlayerShipCtrl.transform.position - transform.position;

        yield return new WaitForSeconds(0.5f);

        transform.up = Vector3.RotateTowards(-transform.up, directionToPlayer, Time.deltaTime * ChargeSpeed, 0.0f);

    }


    //charging past the player
    void UpdateCharge()
    {
        StartCoroutine(ChargeWait());
        
    }


    IEnumerator ChargeWait()
    {
        yield return new WaitForSeconds(0.5f);

        transform.position += transform.up * Time.deltaTime * ChargeSpeed;
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


        if (canSee && ammoCount >= 1)
        {
            if (canSeeBullet == true)
            {
                CurrentAIState = AIMode.Dodge;

                GetComponent<Renderer>().material.color = Color.blue;

            }

            else if (product > 0 && angle < 90)
            {

                CurrentAIState = AIMode.Coasting;

                GetComponent<Renderer>().material.color = Color.red;
            }

        }



        else if (product > 0 && angle < 90 && ammoCount >= 1)
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


        else if (ammoCount == 0)
        {
            CurrentAIState = AIMode.Charge;
            GetComponent<Renderer>().material.color = Color.yellow;
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

