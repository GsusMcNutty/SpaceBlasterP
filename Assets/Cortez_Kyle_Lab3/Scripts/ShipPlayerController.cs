//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
// Credit: For storing the starting rotation of the player, aldonaletto's reply on answers.unity3d helped me understand how
// Credit: I was going about doing it wrong (not using Quaternion for the startRot).
// Credit: http://answers.unity3d.com/questions/332001/how-to-reset-a-gamev-object-to-its-original-rotati.html
//
//Purpose: This is designed to be the player controller and central controller for most of the scripts.
//This script tends to be found referenced in some way in most of the other scritps.


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipPlayerController : MonoBehaviour {

    //Lab work 1
    public Camera CameraObject;

    private float FireRate = 0.4f;

    private float FireTimer = 0.0f;

    private float MoveSpeed = 10.0f;

    

    private Vector3 movement = Vector3.zero;


    [SerializeField]
    float rotSpeed = 1.0f;

    //start pos, rot, && renderer
    private Vector3 startPos;

    private Quaternion startRot;

    [SerializeField]
    private Renderer render;

    //Boost
    private bool boostReady = true;

    [SerializeField]
    public bool boostCooled = true;

    private float BoostCoolDown = 3.0f;

    //Normal Shot
    public Projectile Bullet;

    //Multishot
    bool canUseMulti = false;

    //Burst
    bool canUseBurst = false;

    private List <Projectile> projectileInstances;

    //Ammo
    public TextMesh ammoTextMesh;
      
    private int ammoCount;

    [SerializeField]
    private int ammoMax = 60;

    private string ammoText;



    //Lives
    [SerializeField]
    int MaxLives = 3;

    private int CurLives = 3;

    [SerializeField]
    public bool playerAlive = true;
   

    //Shields
    [SerializeField]
    int MaxShield = 3;

    private int CurShield = 3;

    //Health
    [SerializeField]
    int MaxHealth = 3;

    private int CurHealth = 3;


    //Scoring
    //private int numShotsFired = 0;

    private int numEnemyHits = 0;

    public int Score;

    //Hud
    [SerializeField]
    HUD PlayerHUD;

    //Particles
    [SerializeField]
    GameObject PupParticles;

    [SerializeField]
    GameObject TrailParticles;

    private bool SpawnTrail = false;

    [SerializeField]
    GameObject CrashParticles;

    [SerializeField]
    GameObject PlayerHitParticles;

    [SerializeField]
    GameObject PLifeLost;

    [SerializeField]
    GameObject PLowLife;

    bool lowLives = false;


    //Audio

    [SerializeField]
    AudioClip pickupSfxClip;

    [SerializeField]
    AudioClip shieldHitSfx;

    [SerializeField]
    AudioClip projHitSfx;

    [SerializeField]
    AudioClip shipCrashSfx;

    [SerializeField]
    AudioClip lostLifeSfx;


    [SerializeField]
    AudioClip shipDestroyedSfx;

    [SerializeField]
    AudioClip resetSfx;






    // Use this for initialization
    void Start () {

        ResetStats();

        startPos = transform.position;
        startRot = transform.rotation;

        




    }
	
	// Update is called once per frame
	void Update () {

        if (playerAlive == true)
        {
            UpdatePosition();

            UpdateFiring();

            UpdateAmmo(ammoCount);

            UpdateBoost();

            if(SpawnTrail == true)
            {
                UpdateTrailParticles();
            }

            if (lowLives == true)
            {
                UpdateLowLifeParticles();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetStats();

                


                //audio

                GetComponent<AudioSource>().PlayOneShot(resetSfx);
            }
        }

        //when lives are exhausted, player can no longer move and must restart.
        else if (playerAlive == false)
        {
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetStats();

                

                //audio

                GetComponent<AudioSource>().PlayOneShot(resetSfx);

            }
        }

    

    }

    //Resets
    void ResetStats()
    {
        Debug.Log("Press R to restart");

        render.enabled = true;

        transform.position = startPos;

        transform.rotation = startRot;
        

        playerAlive = true;

        //Ammo reset
        ammoCount = ammoMax;

        //firing bools
        canUseMulti = true;
        canUseBurst = true;

        //numShotsFired = 0;
        numEnemyHits = 0;
        Score = 0;

        PlayerHUD.updateScore(0);

        //Player Lives
        CurLives = MaxLives;

        PlayerHUD.CreateLifeIcons(MaxLives);

        PlayerHUD.updateLives(CurLives);

        lowLives = false;

        //Player Shields
        CurShield = MaxShield;

        PlayerHUD.CreateShieldIcons(MaxShield);

        PlayerHUD.updateShields(CurShield);

        //Player Health
        CurHealth = MaxHealth;

        PlayerHUD.CreateHealthIcons(MaxHealth);

        PlayerHUD.updateHealth(CurHealth);

        
    }

    

    //Player Stats
    public void ModEnemyHits (int _hits)
    {
        numEnemyHits += _hits;
    }

    public void ModScore (int _value)
    {
        Score += _value;

        PlayerHUD.updateScore(Score);
        
    }

    //Mod Lives
    public void ModLives (int value)
    {

        if (CurLives >= 2)
        {
            CurLives += value;

            PlayerHUD.updateLives(CurLives);

            if (CurLives == 1)
            {
                lowLives = true;
            }
        }
        else if (CurLives == 1)
        {
            CurLives -= 1;

            print("Playerdead");

            PlayerHUD.updateLives(CurLives);

            playerAlive = false;

            render.enabled = false;

            GetComponent<AudioSource>().PlayOneShot(shipDestroyedSfx);

        }
  

    }
    
    //ModShields
    public void ModShield(int value)
    {
        if (CurShield >= 2)
        {
            CurShield += value;
            PlayerHUD.updateShields(CurShield);

            GetComponent<AudioSource>().PlayOneShot(shieldHitSfx);

            if (CurShield == MaxShield)
            {
                CurShield = MaxShield -1;
                PlayerHUD.updateShields(CurShield);

                GetComponent<AudioSource>().PlayOneShot(shieldHitSfx);
            }
            
            
        }
        else if (CurShield == 1)
        {
            CurShield -= 1;

            ModHealth(-1);

            print("Player shields down!");

            PlayerHUD.updateShields(CurShield);

            GetComponent<AudioSource>().PlayOneShot(shieldHitSfx);

            if (playerAlive == true)
            {
                CurShield = MaxShield;
                PlayerHUD.updateShields(CurShield);
            }
       

        }

    }

    //Mod Health
    public void ModHealth(int value)
    {
        if (CurHealth >= 2)
        {
            CurHealth += value;

            PlayerHUD.updateHealth(CurHealth);


        }
        else if (CurHealth == 1)
        {
            CurHealth -= 1;

            ModLives(-1);

            SpawnLifeLostParticles();

            GetComponent<AudioSource>().PlayOneShot(lostLifeSfx);

            print("Player Health down!");

            PlayerHUD.updateHealth(CurHealth);

            PlayerHUD.updateLives(CurLives);

            if (playerAlive == true)
            {
                CurHealth = MaxHealth;

                PlayerHUD.updateHealth(CurHealth);
            }


        }

    }



    //Movement
    void UpdatePosition()
    {

        float RotAngle = rotSpeed * Time.deltaTime;
        // x movement
        movement.x = Input.GetAxis("Horizontal") * Time.deltaTime * MoveSpeed;
        gameObject.transform.Translate(movement);

        Vector3 screenPosition = CameraObject.WorldToScreenPoint(gameObject.transform.position);

        if (screenPosition.x > Screen.width)
        {
            screenPosition.x -= Screen.width;

            gameObject.transform.position = CameraObject.ScreenToWorldPoint(screenPosition);
        }

        else if (screenPosition.x < 0)
        {
            screenPosition.x += Screen.width;
            gameObject.transform.position = CameraObject.ScreenToWorldPoint(screenPosition);
        }
        //y movement
        movement.y = Input.GetAxis("Vertical") * Time.deltaTime * MoveSpeed;
        gameObject.transform.Translate(movement);

        if (screenPosition.y > Screen.height)
        {
            screenPosition.y -= Screen.height;

            gameObject.transform.position = CameraObject.ScreenToWorldPoint(screenPosition);
        }

        else if (screenPosition.y < 0)
        {
            screenPosition.y += Screen.height;
            gameObject.transform.position = CameraObject.ScreenToWorldPoint(screenPosition);
        }

        //rotation
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(transform.forward, RotAngle * rotSpeed * 0.2f);

        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(transform.forward, -RotAngle * rotSpeed * 0.2f );

        }

    }


    //Boost
    //Boost is active while held down and cools down when button is released.
    //Code is set to recieve the addition of the boost bar when I can get it working properly.
    void UpdateBoost()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (boostReady == true && boostCooled == true)
            {
                
                MoveSpeed = 20.0f;
                print("Boost has Activated");
                boostCooled = false;

                UpdateTrailParticles();

                SpawnTrail = true;
            }
            else
            {
                print("Boost Cooling Down");
            }
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            
            if (boostReady == true && boostCooled == false)
            {
                SpawnTrail = false;

                MoveSpeed = 10.0f;
                print("Three Seconds til Boost");
                StartCoroutine(Cooling());
            }
        }

    }
    /// <summary>
    /// Boost Cool Down Coroutine
    /// Coroutine credit to unity script reference for WaitForSeconds
    /// 
    /// Note that "prints" are only there to give console feedback. 
    /// You can // Way One and delete the //'s for Way two to enable/disable console feedback.
    /// </summary>
    /// <returns></returns>
    IEnumerator Cooling()
    {
        boostReady = false;
        //Way One-----------------------------------------------
        yield return new WaitForSeconds(BoostCoolDown);

        //Way Two-----------------------------------------------
        //yield return new WaitForSeconds(1.0f);
        //print("Two");
        //yield return new WaitForSeconds(1.0f);
        //print("One");
        //yield return new WaitForSeconds(1.0f);
         

        print("Boost is Ready!");
        boostReady = true;
        boostCooled = true;
        
    }



    //UpdateAmmo
    public void UpdateAmmo(int value)
    {

        ammoTextMesh.text = "Ammo:" + ammoCount + "/" + ammoMax;
    }

    //For testing ammo percent 1x/1x and both values can be changed in the inspector.
    //"Ammo Box" is available as the static icon in the map space.
    void OnTriggerEnter(Collider other)
        
    {

        //particle generators && audio Sfx

        if(other.tag == "EnemyProjectile")
        {
            SpawnPlayerHitParticles();

            GetComponent<AudioSource>().PlayOneShot(projHitSfx);
        }

        if(other.tag == "EnemyShip")
        {
            SpawnCrashParticles();

            GetComponent<AudioSource>().PlayOneShot(shipCrashSfx);
        }


        //pickup hits
        if (other.tag == "Pickup")
        {
            Debug.Log("Pickup hit by player!");

            if(other.name == "Score")
            {
                ModScore(5);

                Destroy(other.gameObject);
            }

            //Life pickup
            if (other.name == "Life")
            {
                ModLives(1);

                Destroy(other.gameObject);
            }

            //HP pickup
            if (other.name == "HP")
            {
                ModHealth(1);

                Destroy(other.gameObject);
            }

            //Shield pickup
            if (other.name == "Shield")
            {
                ModShield(1);

                Destroy(other.gameObject);
            }

            //Ammo pickup
            if (other.name == "Ammo")
            {
                if (ammoCount == ammoMax)
                {
                    print("AmmoFull");
                }
                else if (ammoCount <= ammoMax)
                {
                    ammoCount = ammoMax;

                    Destroy(other.gameObject);
                }
            }
            if (other.name == "Negative")
            {
                Destroy(other.gameObject);

                int randIndex = Random.Range(0, 2);

                if(randIndex == 0)
                {
                    ModHealth(-1);
                }
                if(randIndex == 1)
                {
                    ModShield(-1);
                }
                if(randIndex == 2)
                {
                    boostCooled = false;
                }
               
            }
            //MultiShot pickup
            if (other.name == "Multi")
            {
                Destroy(other.gameObject);

                canUseMulti = true;
            }

            //Burst pickup
            if (other.name == "Burst")
            {
                Destroy(other.gameObject);

                canUseBurst = true;
            }

            SpawnPupParticles();

            GetComponent<AudioSource>().PlayOneShot(pickupSfxClip);

        }

    }

    //player firing
    void UpdateFiring()
    {

        FireTimer += Time.deltaTime;

        //Normal Fire check
        if (Input.GetButton("Fire1"))
        {

            if (FireTimer > FireRate)
            {
                FireTimer = 0;
                print("The fire button has been pressed!");

                DoWeaponFire();
            }

        }
        //Multi-Shot Check

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (canUseMulti == true)
            {
                if (FireTimer > FireRate)
                {
                    FireTimer = 0;
                    print("The MultiFire button has been pressed!");

                    DoMultiShot(4);

                    canUseMulti = false;
                }
            }
            else if (canUseMulti == false)
            {
                print("Multishot Powerup Required!");
            }
        }

        //Burst Fire Check
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (canUseBurst == true)
            {
                if (FireTimer > FireRate)
                {
                    FireTimer = 0;
                    print("The BurstFire button has been pressed!");

                    DoBurstFire();

                    canUseBurst = false;

                }
            }
            else if (canUseBurst == false)
            {
                print("BurstFire Powerup Required!");
            }
        }

    }

    void DoWeaponFire()
    {
        if (ammoCount > 0)
        {

            print("The \"DoWeaponFire\" function has been called!");
            Instantiate(Bullet, transform.position, transform.rotation);
            ammoCount--;
        }
        //If ammo = 0, weapon cannot fire.
        else
        {
            print("NoAmmo");
        }
    }

    //Multi-shot has similar coding to that of instancing shields, hp, and lives in the HUD
    void DoMultiShot(int numShots)
    {
        if (ammoCount >= 3)
        {
            projectileInstances = new List<Projectile>();

            projectileInstances.Add(Bullet);

            Vector3 positionOffset = new Vector3(-0.4f, 0, 0);

            for (int i = 1; i < numShots; ++i)
            {
                Projectile newBullets;
                newBullets = Instantiate(Bullet,
                                         transform.position,
                                         transform.rotation) as Projectile;

                Vector3 newPosition = newBullets.transform.position;
                
                
                newPosition.x += (positionOffset.x * i + 0.8f);
                newBullets.transform.position = newPosition;

                newBullets.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                projectileInstances.Add(newBullets);


            }

            ammoCount--;
            ammoCount--;
            ammoCount--;



        }
        else
        {
            // if 3 rounds of ammo is unavailable, ability will not fire.
            print("Not enough Ammo");
        }
    }

    //BurstFire; Credit to unity3d API script reference "WaitForSeconds"
    void DoBurstFire()
    {
       
            if (ammoCount >= 3)
            {
                StartCoroutine(Fire());
            }

            else
            //much like multishot, ability will not fire if there is not enough ammo for the burst.
            {
                print("Not enough Ammo!");
            }

           
    }

    IEnumerator Fire()
    {
        DoWeaponFire();
        yield return new WaitForSeconds(0.1f);
        DoWeaponFire();
        yield return new WaitForSeconds(0.1f);
        DoWeaponFire();
    }




    //Particles
    void SpawnPupParticles()
    {
        Instantiate(PupParticles, transform.position, transform.rotation);
    }


    
    void SpawnTrailParticles()
    {
        Instantiate(TrailParticles, transform.position, transform.rotation);
    }


    void SpawnPlayerHitParticles()
    {
        Instantiate(PlayerHitParticles, transform.position, transform.rotation);
    }

    void SpawnCrashParticles()
    {
        Instantiate(CrashParticles, transform.position, transform.rotation);
    }

    void SpawnLifeLostParticles()
    {
        Instantiate(PLifeLost, transform.position, transform.rotation);
    }

    void SpawnLifeLowParticles()
    {
        Instantiate(PLowLife, transform.position, transform.rotation);
    }



    void UpdateLowLifeParticles()
    {
        SpawnLifeLowParticles();
    }



    void UpdateTrailParticles()
    {


        SpawnTrailParticles();
    }

}
