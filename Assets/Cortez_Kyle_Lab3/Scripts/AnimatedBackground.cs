//------------------------------------------------------------------------------------------------
// Author: Kyle Cortez
// Date: 9/4/2016
//
// Credit: I used Unity script reference for many functions in this script as well as course material.
//
// Purpose: This script is used to animate the background.
using UnityEngine;
using System.Collections;

public class AnimatedBackground : MonoBehaviour {

    public Vector2 ScrollSpeed = Vector2.zero;

    private Vector2 TextureOffset = Vector2.zero;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        TextureOffset += ScrollSpeed * Time.deltaTime;

        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", TextureOffset);
	
	}
}
