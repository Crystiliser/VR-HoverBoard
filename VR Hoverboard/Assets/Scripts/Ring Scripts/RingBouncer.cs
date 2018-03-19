using UnityEngine;
public class RingBouncer : MonoBehaviour
{
    private enum StartDirection { Positive, Negative };
    private Transform anchor = null;
    private bool negativeDirection = false;
    private Vector3 maxPos, minPos, currPos;
    [SerializeField] private StartDirection startDirection = StartDirection.Positive;
    [SerializeField] private bool bounceVertically = true;
    [SerializeField] private float bounceDistance = 1.0f;
    [SerializeField] private float bounceRate = 5.0f;
    private void Start()
    {
        if (StartDirection.Negative == startDirection)
            negativeDirection = true;
        anchor = transform;
        if (bounceVertically)
        {
            maxPos = anchor.position + anchor.TransformDirection(0.0f, bounceDistance, 0.0f);
            minPos = anchor.position - anchor.TransformDirection(0.0f, bounceDistance, 0.0f);
        }
        else
        {
            maxPos = anchor.position + anchor.TransformDirection(-bounceDistance, 0.0f, 0.0f);
            minPos = anchor.position - anchor.TransformDirection(-bounceDistance, 0.0f, 0.0f);
        }
        currPos = anchor.position;
    }
    private void FixedUpdate()
    {
        if (negativeDirection)
        {
            negativeDirection = currPos != minPos;
            anchor.position = currPos = Vector3.MoveTowards(currPos, minPos, Time.fixedDeltaTime * bounceRate);
        }
        else
        {
            negativeDirection = currPos == maxPos;
            anchor.position = currPos = Vector3.MoveTowards(currPos, maxPos, Time.fixedDeltaTime * bounceRate);
        }
    }
}