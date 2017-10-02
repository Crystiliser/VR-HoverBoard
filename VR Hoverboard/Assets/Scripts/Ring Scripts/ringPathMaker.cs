using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ringPathMaker : MonoBehaviour
{
    bool drawLine = true;
    //needs to be at least 4
    [SerializeField]
    Stack<Vector3> controlPointsStack = new Stack<Vector3>();

    CatmullRomSplineDrawn pathDrawer = new CatmullRomSplineDrawn();

    LineRenderer myself;
    

    //line or loop
    public bool isLooping = false;



    private void Start()
    {
        myself = GetComponent<LineRenderer>();
    }
    

    #region EventStuff
    void togglePath(bool isOn)
    {
        drawLine = isOn;
    }
    private void Awake()
    {
        EventManager.OnSetRingPath += togglePath;
    }
    private void OnDisable()
    {
        EventManager.OnSetRingPath -= togglePath;
    }

    #endregion


    //sets up array before we even do the catmull stuff, then does the actual drawing once at the very end
    public void init(Transform[] array)
    {
        if (drawLine)
        {
            //push back the first ring in the array 
            //twice because we dont want to do looping
            controlPointsStack.Push(array[0].position);
            controlPointsStack.Push(array[0].position);
            int firstDuplicate = 0;
            bool foundDuplicate = false;
            int duplicateNum = 0;
            int lastRing = 0;

            RingProperties theRing = array[0].GetComponent<RingProperties>();
            //loop through the arary pushing back every ring that isnt tagged dupicate
            //if it is a dulicate find one of the rings and only push back that one
            for (int i = 1; i < array.Length; i++)
            {
                theRing = array[i].gameObject.GetComponent<RingProperties>();
                if (theRing.duplicatePosition)
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

            //actual drawing stuff
            Vector3[] finalPoints = pathDrawer.makePath(controlPointsStack.ToArray());

            myself.positionCount = finalPoints.Length;
            myself.SetPositions(finalPoints);
        }
    }

}
