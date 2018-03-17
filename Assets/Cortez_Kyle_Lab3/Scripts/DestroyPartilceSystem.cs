//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This script is used to destroy the particle systems to save on resources.

using UnityEngine;
using System.Collections;

public class DestroyPartilceSystem : MonoBehaviour {




	// Use this for initialization
	void Start () {

        //ParticleSystem system = gameObject.GetComponent<ParticleSystem>();

        Destroy(gameObject, 1.0f);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
