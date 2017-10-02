using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAimAt : MonoBehaviour
{
    public Transform[] thingsToLookAt;
    public RingProperties[] sortedRings;

    [HideInInspector] public int currentlyLookingAt = -1;
    void Update()
    {
        if (currentlyLookingAt >= 0 && currentlyLookingAt < thingsToLookAt.Length)
        {
            gameObject.transform.LookAt(thingsToLookAt[currentlyLookingAt].transform);
        }
    }

}
