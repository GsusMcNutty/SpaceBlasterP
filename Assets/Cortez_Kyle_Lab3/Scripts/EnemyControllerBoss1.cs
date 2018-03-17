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
// Purpose: This is a boss that the enemy can fight once the number of spawns have been reached.
// Purpose: This enemy type was unfinished but the initial start has been accomplished.
// Purpose: It does look cool coming in though.

using UnityEngine;
using System.Collections;

public class EnemyControllerBoss1 : MonoBehaviour
{

    [SerializeField]
    GameObject[] PickupTypes;

    private ShipPlayerController PlayerShipCtrl;

    //Enemy move speed

    [SerializeField]
    float MoveSpeed = 2.0f;


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
    [SerializeField]
    float coastTimer = 2.0f;

    bool startMovement = true;

    //float ChargeSpeed = 7.0f;

    enum AIMode { StartMovement, AimTowards, FiringSingle, FiringMulti, Resting };

    private AIMode CurrentAIState;

    //bool isStrafing = false;

    //bool canStrafe = false;









    // Use this for initialization
    void Start()
    {

        //AI Start
        CurrentAIState = AIMode.StartMovement;

        canFire = true;


        CurEShield = MaxEShield;


        GameObject playerShip = GameObject.Find("PlayerShip");

        PlayerShipCtrl = playerShip.GetComponent<ShipPlayerController>();

        

        //ChooseMovement();



        //axis = transform.right;

        //startPos = transform.position;


        StartCoroutine(Firing());


        StartCoroutine(CoastForward());



    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerShipCtrl.playerAlive == true)
        {

            

            DetermineAIState();


            switch (CurrentAIState)
            {

                case AIMode.StartMovement:
                    UpdateStartMovement();
                    break;

                case AIMode.AimTowards:
                    UpdateAimTowards();
                    break;

                case AIMode.FiringSingle:
                    UpdateFiringSingle();
                    break;

                case AIMode.FiringMulti:
                    UpdateFiringMulti();
                    break;



                case AIMode.Resting:
                    UpdateResting();
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

    }

    //firing
    IEnumerator Firing()
    {

        yield return new WaitForSeconds(FireRate);

        ChooseFire();
        if (canFire == true)
        {
            EnemyFire();

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
        if (other.tag == "Bounds")
        {
            Destroy(gameObject);
        }
    }




    public void ModShield(int value)
    {
        if (CurEShield >= 1)
        {
            CurEShield += value;

        }

        if (CurEShield == 0)
        {

            Destroy(gameObject);
            SpawnPickup();

            PlayerShipCtrl.ModScore(pointValue);

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
            Destroy(gameObject);

            PlayerShipCtrl.ModScore(-1);

            PlayerShipCtrl.ModShield(-1);
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
    //        isStrafing = false;

    //    }
    //    if (randMoveIndex == 1)
    //    {

    //        isStrafing = true;
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

    void UpdateResting()
    {
        StopAllCoroutines();

    }


    //To get to starting Pos

    void UpdateStartMovement()
    {
        if(startMovement == true)
        {
            transform.position += transform.up * Time.deltaTime * CoastSpeed;
            
        }

    }

    IEnumerator CoastForward()
    {

        

        yield return new WaitForSeconds(coastTimer);

        //canStrafe = true;

        startMovement = false;

        


    }


    void UpdateAimTowards()
    {
        Vector3 directionToPlayer = PlayerShipCtrl.transform.position - transform.position;

        transform.up = Vector3.RotateTowards(transform.up, directionToPlayer, Time.deltaTime * MoveSpeed, 0.0f);


    }

    
    void UpdateFiringSingle()
    {

    }


    void UpdateFiringMulti()
    {

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


        if (startMovement == true)
        {
            UpdateStartMovement();


        }
        else if (canSee && product > 0 && angle < 90)
        {

            CurrentAIState = AIMode.AimTowards;

            GetComponent<Renderer>().material.color = Color.green;

        }

        if (canSee)
        {


            GetComponent<Renderer>().material.color = Color.red;
        }

    }
}

