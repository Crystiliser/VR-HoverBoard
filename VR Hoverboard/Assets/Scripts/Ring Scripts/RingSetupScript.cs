using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSetupScript : MonoBehaviour
{
    Transform[] ringTransforms;
    arrowPointAtUpdater arrowScript;

    //state of what the rings should be setup as determined by gamemode
    public GameModes mode;


    void Start()
    {
        arrowScript = GameManager.player.GetComponentInChildren<arrowPointAtUpdater>();
        mode = GameManager.instance.gameMode.currentMode;

        if (arrowScript != null)
        {
            RingProperties[] rings;
            rings = GetComponentsInChildren<RingProperties>();

            //insertion sort the rings according to their position in order
            int arrayLength = rings.GetLength(0);
            InsertionSort(rings, arrayLength);
            
            setRingsMode(mode, rings);

            //assign the transforms from the sorted array
            ringTransforms = new Transform[rings.GetLength(0)];

            for (int i = 0; i < arrayLength; i++)
                ringTransforms[i] = rings[i].transform;
            

            arrowScript.thingsToLookAt = ringTransforms;
            arrowScript.sortedRings = rings;
        }
        if (ringTransforms != null)
        {
            gameObject.GetComponent<ringPathMaker>().init(ringTransforms);
        }
    }

    void setRingsMode(GameModes theMode, RingProperties[] rings)
    {
        switch (theMode)
        {
            case GameModes.Continuous:
                arrowScript.currentlyLookingAt = 1;
                break;
            case GameModes.Cursed:
                RingProperties lastRing = rings[rings.Length - 1];
                RingProperties nextToLastRing = rings[rings.Length - 2];

                if (lastRing.nextScene != 1)
                {
                    lastRing.gameObject.SetActive(false);
                }
                else
                {
                    nextToLastRing.gameObject.SetActive(false);
                }
                arrowScript.currentlyLookingAt = 1;
                break;
            case GameModes.Free:
                for (int i = 0; i < rings.Length - 2; i++)
                {
                    rings[i].gameObject.SetActive(false);
                }

                arrowScript.currentlyLookingAt = rings.Length - 1;
                break;
            case GameModes.GameModesSize:
                break;
            default:
                break;
        }
    }

    void InsertionSort(RingProperties[] rings, int arrayLength)
    {
        int currRing = 1;
        while (currRing < arrayLength)
        {
            RingProperties storedRing = rings[currRing];

            int compareRing = currRing - 1;
            while (compareRing >= 0 && rings[compareRing].positionInOrder > storedRing.positionInOrder)
            {
                rings[compareRing + 1] = rings[compareRing];
                --compareRing;
            }

            rings[compareRing + 1] = storedRing;
            ++currRing;
        }
    }

    private void OnDisable()
    {
        if (arrowScript != null)
        {
            arrowScript.thingsToLookAt = null;
            arrowScript.currentlyLookingAt = -1;
        }
    }
}
