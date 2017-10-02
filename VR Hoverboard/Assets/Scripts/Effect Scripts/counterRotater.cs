using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class counterRotater : MonoBehaviour
{
    Quaternion rotation;

	// Use this for initialization
	void Start ()
    {
        rotation = gameObject.transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.rotation = rotation;
	}
}    