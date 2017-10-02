using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubPortalRotator : MonoBehaviour
{
    [SerializeField] float rotateRate = 30f;
    [SerializeField] float distanceFromCenter = 28f;
    Transform t;

    float rotateAmount;
	
	void Awake ()
    {
        t = GetComponent<Transform>();

        rotateAmount = 360f / t.childCount;

        for (int i = 0; i < t.childCount; i++)
        {
            t.GetChild(i).Rotate(Vector3.up * i * rotateAmount);
            t.GetChild(i).Translate(Vector3.back * distanceFromCenter, Space.Self);
            t.GetChild(i).LookAt(t);
        }
	}
	
	void Update ()
    {
        t.Rotate(Vector3.up, rotateRate * Time.deltaTime);
	}
}
