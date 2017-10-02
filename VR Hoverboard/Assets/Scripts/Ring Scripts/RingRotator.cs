using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//parent this gameobject to any rings that you want to rotate
//don't forget to set the individual ring properties and positions
public class RingRotator : MonoBehaviour
{
    Transform anchor;
    [SerializeField] float rotateRate = 5f;

    void Start()
    {
        anchor = GetComponent<Transform>();
    }

    void Update()
    {
        if (rotateRate != 0f)
            anchor.Rotate(Vector3.forward, Time.deltaTime * rotateRate);
    }
}
