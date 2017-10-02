using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectScript : MonoBehaviour {
   public Color color;
    public Color invis;
    public GameObject obj;
    Renderer objectRenderer;
    public float howFarToFade;
	// Use this for initialization

        //I do not claim to know what I am doing, but thinking about it 
        //this may slow down the game if it is running this script on every single ring
        //attach these to all the prefabs in the scene and test it out tomorrow or tonight when I get home
	void Start () {
        howFarToFade = 15.0f;
        obj = this.gameObject;
        objectRenderer = obj.GetComponent<Renderer>();
        
        color = objectRenderer.material.color;
        invis = color;
        invis.a = 0;

        //this will currently work if I set the obj to the pTorus in the editor
        //also if I set the ring rendering mode to transparent
        //run it by the team, if they want me to find a way to optimize it I will have to do more research
	}
	
	// Update is called once per frame
	void Update () {
        float distance = (Camera.main.transform.position - obj.transform.position).magnitude;
        if(distance < howFarToFade)
        {
            objectRenderer.material.color = Color.Lerp(color, invis, (howFarToFade / 2) / distance);
        }
	}
}
