using UnityEngine;
public class PlayerArrowHandler : MonoBehaviour
{
    private arrowPointAtUpdater arrowScript = null;
    private RingProperties theRing = null;
    private int prevPositionInOrder = 0;
    private void Start()
    {
        prevPositionInOrder = 0;
    }
    private void HandleDuplicateRing(int ringArrLength)
    {
        int originalPosition = theRing.positionInOrder, originalLookingAt = arrowScript.currentlyLookingAt, comparePosition = 0;
        arrowScript.currentlyLookingAt = -1;
        for (int offset = 1; arrowScript.currentlyLookingAt + offset < ringArrLength; ++offset)
        {
            comparePosition = arrowScript.sortedRings[originalLookingAt + offset].positionInOrder;
            if (originalPosition < comparePosition)
            {
                arrowScript.currentlyLookingAt = originalLookingAt + offset;
                break;
            }
        }
    }
    private void HandleRegularRing(int ringArrLength)
    {
        RingProperties rp;
        for (int i = 0; i < ringArrLength; ++i)
        {
            if (-1 != arrowScript.currentlyLookingAt)
            {
                rp = arrowScript.sortedRings[arrowScript.currentlyLookingAt];
                if (!rp.LastRingInScene && rp.positionInOrder <= theRing.positionInOrder)
                {
                    ++arrowScript.currentlyLookingAt;
                    if (arrowScript.currentlyLookingAt >= arrowScript.sortedRings.Length)
                    {
                        arrowScript.currentlyLookingAt = 0;
                    }
                }
                else
                    break;
            }
        }
    }
    public void UpdatePlayerHUDPointer(RingProperties rp)
    {
        theRing = rp;
        if (null != arrowScript && prevPositionInOrder < theRing.positionInOrder)
        {
            int ringArrLength = arrowScript.sortedRings.GetLength(0);
            if (theRing.DuplicatePosition)
                HandleDuplicateRing(ringArrLength);
            else
                HandleRegularRing(ringArrLength);
            prevPositionInOrder = theRing.positionInOrder;
        }
        if (theRing.LastRingInScene)
        {
            if (null != arrowScript)
                arrowScript.currentlyLookingAt = -1;
            prevPositionInOrder = 0;
        }
    }
    private void OnEnable()
    {
        arrowScript = GetComponentInChildren<arrowPointAtUpdater>();
    }
}