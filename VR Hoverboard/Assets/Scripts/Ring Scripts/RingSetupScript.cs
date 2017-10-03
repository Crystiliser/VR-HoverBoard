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

            //use a list to remove rings we won't need depending on game mode
            List<RingProperties> ringList = new List<RingProperties>();

            rings = GetComponentsInChildren<RingProperties>();

            //insertion sort the rings according to their position in order
            InsertionSort(rings, rings.GetLength(0));

            //set our sorted rings to our ring list
            foreach (RingProperties rp in rings)
            {
                ringList.Add(rp);
            }

            //update our ring list depending on game mode
            setRingsMode(mode, ringList);

            //assign the transforms from the sorted list
            ringTransforms = new Transform[ringList.Count];

            for (int i = 0; i < ringList.Count; i++)
                ringTransforms[i] = ringList[i].transform;
            

            arrowScript.thingsToLookAt = ringTransforms;
            arrowScript.sortedRings = rings;
        }
        if (ringTransforms != null)
        {
            gameObject.GetComponent<ringPathMaker>().init(ringTransforms);
        }
    }

    void setRingsMode(GameModes theMode, List<RingProperties> rings)
    {
        switch (theMode)
        {
            case GameModes.Continuous:
                arrowScript.currentlyLookingAt = 1;
                break;
            case GameModes.Cursed:
                RingProperties lastRing = rings[rings.Count - 1];
                RingProperties nextToLastRing = rings[rings.Count - 2];

                if (lastRing.nextScene != 1)
                {
                    lastRing.gameObject.SetActive(false);
                    rings.Remove(lastRing);
                }
                else
                {
                    nextToLastRing.gameObject.SetActive(false);
                    rings.Remove(nextToLastRing);
                }
                arrowScript.currentlyLookingAt = 1;
                break;
            case GameModes.Free:
                for (int i = 0; i < rings.Count - 2; i++)
                {
                    rings[i].gameObject.SetActive(false);
                }

                arrowScript.currentlyLookingAt = rings.Count - 1;
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
