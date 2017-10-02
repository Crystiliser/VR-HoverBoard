using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutatorAnchor : MonoBehaviour
{
	void Start ()
    {
        if (GetComponent<MeshRenderer>().enabled == true)
            GetComponent<MeshRenderer>().enabled = false;
    }

}
