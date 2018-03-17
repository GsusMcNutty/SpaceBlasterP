//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This script is to be used for instantiating most of the HUD components as well as be used to 
// refresh the HUD.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour {

    

    //Scoretext
    public TextMesh scoreTextMesh;




    //LifeIcons

    public LifeIcon lifeIconObj;

    private List<LifeIcon> lifeIconInstances;

    //ShieldIcons

    public ShieldIcon shieldIconObj;

    private List<ShieldIcon> shieldIconInstances;

    //Health Icons

    public HealthIcon healthIconObj;

    private List<HealthIcon> healthIconInstances;






    // Use this for initialization
    void Start () {

     

    }

    //score updater
    public void updateScore(int value)
    {
        scoreTextMesh.text = "Score:" + value;

        
    }

    //LifeIcon Instancing

    public void CreateLifeIcons(int numLives)
    {
        if (lifeIconInstances == null)
        {
            lifeIconInstances = new List<LifeIcon>();

            lifeIconInstances.Add(lifeIconObj);

            Vector3 positionOffset = new Vector3(1.5f, 0, 0);

            for (int i = 1; i < numLives; ++i)
            {
                LifeIcon newlifeIcon;
                newlifeIcon = Instantiate(lifeIconObj,
                                           lifeIconObj.transform.position,
                                           lifeIconObj.transform.rotation) as LifeIcon;

                Vector3 newPosition = newlifeIcon.transform.position;
                newPosition.x += (positionOffset.x * i * 0.6f);
                newlifeIcon.transform.position = newPosition;

                newlifeIcon.transform.localScale = new Vector3(1 , 1 , 1);

                lifeIconInstances.Add(newlifeIcon);
            }
        }
    }

    public void updateLives(int numLivesRemaining)
    {
        for (int i = 0; i < lifeIconInstances.Count; ++i)
        {
            bool bActive = i < numLivesRemaining;
            lifeIconInstances[i].gameObject.SetActive(bActive);
        }
    }



    //Shield instances
    //For both Shields and Health, the code is essentially the Experiment One's code for instancing Lives

    public void CreateShieldIcons(int numShields)
    {
        if (shieldIconInstances == null)
        {
            shieldIconInstances = new List<ShieldIcon>();

            shieldIconInstances.Add(shieldIconObj);

            Vector3 positionOffset = new Vector3(0.1f, 0, 0);

            for (int i = 1; i < numShields; ++i)
            {
                ShieldIcon newShieldIcon;
                newShieldIcon = Instantiate(shieldIconObj,
                                           shieldIconObj.transform.position,
                                           shieldIconObj.transform.rotation) as ShieldIcon;

                Vector3 newPosition = newShieldIcon.transform.position;
                newPosition.x -= (positionOffset.x * i);
                newShieldIcon.transform.position = newPosition;

                newShieldIcon.transform.localScale = new Vector3(1, 1, 1);

                shieldIconInstances.Add(newShieldIcon);
            }
        }
    }

    public void updateShields(int numShieldsRemaining)
    {
        for (int i = 0; i < shieldIconInstances.Count; ++i)
        {
            bool bActive = i < numShieldsRemaining;
            shieldIconInstances[i].gameObject.SetActive(bActive);
        }
    }

    //Health instances
    public void CreateHealthIcons(int numHealth)
    {
        if (healthIconInstances == null)
        {
            healthIconInstances = new List<HealthIcon>();

            healthIconInstances.Add(healthIconObj);

            Vector3 positionOffset = new Vector3(0.4f, 0, 0);

            for (int i = 1; i < numHealth; ++i)
            {
                HealthIcon newHealthIcon;
                newHealthIcon = Instantiate(healthIconObj,
                                           healthIconObj.transform.position,
                                           healthIconObj.transform.rotation) as HealthIcon;

                Vector3 newPosition = newHealthIcon.transform.position;
                newPosition.x += (positionOffset.x * i);
                newHealthIcon.transform.position = newPosition;

                newHealthIcon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                healthIconInstances.Add(newHealthIcon);
            }
        }
    }

    public void updateHealth(int numHealthRemaining)
    {
        for (int i = 0; i < healthIconInstances.Count; ++i)
        {
            bool bActive = i < numHealthRemaining;
            healthIconInstances[i].gameObject.SetActive(bActive);
        }
    }


    // Update is called once per frame
    void Update () {
	
	}
}
