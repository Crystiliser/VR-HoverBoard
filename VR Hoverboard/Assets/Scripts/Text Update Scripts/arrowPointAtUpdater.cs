﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class arrowPointAtUpdater : MonoBehaviour
{
    public Transform[] thingsToLookAt;
    public RingProperties[] sortedRings;
    //[SerializeField] float angle;
    //GameObject player;
    public Camera the_camera;

    [SerializeField] float angle;

    [SerializeField] float pointPosition = 0.5f;
    Slider myself;

    [HideInInspector] public int currentlyLookingAt = -1;


    private void Start()
    {
        //player = GameManager.player;
        myself = GetComponent<Slider>();
    }
    private void Update()
    {
        if (currentlyLookingAt > 0 && currentlyLookingAt < thingsToLookAt.Length)
        {
            Vector3 direction = thingsToLookAt[currentlyLookingAt].position - the_camera.transform.position;


            angle = Vector3.SignedAngle(the_camera.transform.forward, direction, Vector3.up);

            if (angle < 90 && angle > 0)
            {
                pointPosition = angle / 180;
                pointPosition += 0.5f;
            }
            else if (180 > angle && angle >= 90)
            {
                pointPosition = 1;
            }
            else if (-180 <= angle && angle < -90)
            {
                pointPosition = 0;
            }
            else if (angle > -90 && angle < 0)
            {
                pointPosition = angle / -180;
                pointPosition = 0.5f - pointPosition;
                if (pointPosition < 0)
                {
                    pointPosition = 0;
                }
            }

            myself.value = pointPosition;

            //Debug.DrawLine(referenceObject.transform.position, thingsToLookAt[currentlyLookingAt].position);

            Debug.DrawRay(the_camera.transform.position, the_camera.transform.forward * 100, Color.blue);
        }
    }
}
