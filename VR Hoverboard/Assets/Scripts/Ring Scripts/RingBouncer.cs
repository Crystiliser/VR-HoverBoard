using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBouncer : MonoBehaviour
{
    enum StartDirection { Positive, Negative };

    Transform anchor;
    float direction = 1f;
    float step;

    Vector3 leftPos, rightPos, topPos, bottomPos, currPos;

    [SerializeField] StartDirection startDirection = StartDirection.Positive;
    [SerializeField] bool bounceVertically = true;
    [SerializeField] float bounceDistance = 1f;
    [SerializeField] float bounceRate = 5f;

	void Start ()
    {
        if (startDirection == StartDirection.Negative)
            direction = -1f;

        anchor = GetComponent<Transform>();

        if (bounceVertically)
        {
            bottomPos = anchor.position + anchor.TransformDirection(Vector3.up * -bounceDistance);
            topPos = anchor.position + anchor.TransformDirection(Vector3.up * bounceDistance);
        }
        else
        {
            leftPos = anchor.position + anchor.TransformDirection(Vector3.left * bounceDistance);
            rightPos = anchor.position + anchor.TransformDirection(Vector3.left * -bounceDistance);
        }

        currPos = anchor.position;
    }

    void BounceVertically()
    {
        step = Time.deltaTime * bounceRate;
        if (direction > 0f)
        {
            if (anchor.position == topPos)
                direction *= -1f;
          
            currPos = Vector3.MoveTowards(currPos, topPos, step);
        }
        else
        {
            if (anchor.position == bottomPos)
                direction *= -1f;

            currPos = Vector3.MoveTowards(currPos, bottomPos, step);
        }

        anchor.position = currPos;
    }

    void BounceHorizontally()
    {
        step = Time.deltaTime * bounceRate;
        if (direction > 0f)
        {
            if (anchor.position == leftPos)
                direction *= -1f;

            currPos = Vector3.MoveTowards(currPos, leftPos, step);
        }
        else
        {
            if (anchor.position == rightPos)
                direction *= -1f;

            currPos = Vector3.MoveTowards(currPos, rightPos, step);
        }

        anchor.position = currPos;
    }

    private void FixedUpdate()
    {
        if (bounceVertically)
            BounceVertically();
        else
            BounceHorizontally();
    }
}
