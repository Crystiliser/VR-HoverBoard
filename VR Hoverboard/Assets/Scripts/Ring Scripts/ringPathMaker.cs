using System.Collections.Generic;
using UnityEngine;
public class ringPathMaker : MonoBehaviour
{
    [SerializeField] private Stack<Vector3> controlPointsStack = new Stack<Vector3>();
    private bool drawLine = true;
    private AI_Race_Mode_Script Race_AI;
    private void TogglePath(bool isOn) => drawLine = isOn;
    private void OnEnable()
    {
        EventManager.OnSetRingPath += TogglePath;
        Race_AI = FindObjectOfType<AI_Race_Mode_Script>();
    }
    private void OnDisable()
    {
        EventManager.OnSetRingPath -= TogglePath;
    }
    public void Init(Transform[] array)
    {
        LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
        if (drawLine)
        {
            controlPointsStack.Push(array[0].position);
            controlPointsStack.Push(array[0].position);
            int firstDuplicate = 0, duplicateNum = 0, lastRing = 0;
            bool foundDuplicate = false;
            RingProperties theRing;
            for (int i = 1; i < array.Length; ++i)
            {
                theRing = array[i].GetComponent<RingProperties>();
                if (theRing.DuplicatePosition)
                {
                    if (!foundDuplicate)
                    {
                        firstDuplicate = i;
                        duplicateNum = theRing.positionInOrder;
                    }
                    else if (duplicateNum != theRing.positionInOrder)
                    {
                        controlPointsStack.Push(array[firstDuplicate].position);
                        duplicateNum = theRing.positionInOrder;
                        firstDuplicate = i;
                        lastRing = i;
                    }
                    foundDuplicate = true;
                }
                else
                {
                    if (foundDuplicate)
                    {
                        controlPointsStack.Push(array[firstDuplicate].position);
                        foundDuplicate = false;
                    }
                    controlPointsStack.Push(array[i].position);
                    lastRing = i;
                }
            }
            controlPointsStack.Push(array[lastRing].position);
            Vector3[] finalPoints = CatmullRomSplineDrawn.MakePath(controlPointsStack.ToArray());
            if (null != Race_AI)
                Race_AI.Ring_path = finalPoints;
            lineRenderer.positionCount = finalPoints.Length;
            lineRenderer.SetPositions(finalPoints);
        }
    }
}
public static class CatmullRomSplineDrawn
{
    public static Vector3[] MakePath(Vector3[] points)
    {
        Stack<Vector3> finalPoints = new Stack<Vector3>();
        Vector3 a, b, c;
        for (int i = 1; i < points.Length - 2; ++i)
        {
            a = (points[i] - points[i + 1]) * 3.0f - points[i - 1] + points[i + 2];
            b = points[i - 1] + points[i - 1] - points[i] * 5.0f + points[i + 1] * 4.0f - points[i + 2];
            c = points[i + 1] - points[i - 1];
            finalPoints.Push(a * 0.004f + b * 0.02f + c * 0.1f + points[i]);
            finalPoints.Push(a * 0.032f + b * 0.08f + c * 0.2f + points[i]);
            finalPoints.Push(a * 0.108f + b * 0.18f + c * 0.3f + points[i]);
            finalPoints.Push(a * 0.256f + b * 0.32f + c * 0.4f + points[i]);
            finalPoints.Push((a + b + c) * 0.5f + points[i]);
        }
        return finalPoints.ToArray();
    }
}