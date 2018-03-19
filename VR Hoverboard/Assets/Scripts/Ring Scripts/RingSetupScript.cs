using System.Collections.Generic;
using UnityEngine;
using Xander.NullConversion;
public class RingSetupScript : MonoBehaviour
{
    [SerializeField] private GameObject lapTrigger = null;
    [SerializeField] private GameObject[] ringDifficultyParents = null;
    private Transform[] ringTransforms = null;
    private arrowPointAtUpdater arrowScript = null;
    private RingProperties[] sortedRings = null;
    private GameMode mode;
    private GameDifficulty difficulty;
    public GameObject GetRingDifficultyParent(GameDifficulty gameDifficulty) => ringDifficultyParents[(int)gameDifficulty];
    private void Start()
    {
        arrowScript = GameManager.player.GetComponentInChildren<arrowPointAtUpdater>();
        mode = GameManager.gameMode;
        difficulty = GameManager.gameDifficulty;
        foreach (GameObject item in ringDifficultyParents)
            item.ConvertNull()?.SetActive(false);
        ringDifficultyParents[(int)difficulty].SetActive(true);
        List<RingProperties> ringList = new List<RingProperties>();
        RingProperties[] rings = GetComponentsInChildren<RingProperties>();
        InsertionSort(rings);
        foreach (RingProperties rp in rings)
            ringList.Add(rp);
        setRingsMode(ringList);
        ringTransforms = new Transform[ringList.Count];
        for (int i = 0; i < ringList.Count; ++i)
            ringTransforms[i] = ringList[i].transform;
        sortedRings = rings;
        if (GameMode.Cursed == mode)
            SetupStartBonusTime();
        if (null != arrowScript)
        {
            arrowScript.thingsToLookAt = ringTransforms;
            arrowScript.sortedRings = rings;
        }
        if (null != ringTransforms)
            GetComponent<ringPathMaker>().Init(ringTransforms);
    }
    private void RemoveNEXTRing(List<RingProperties> rings)
    {
        RingProperties lastRing = rings[rings.Count - 1];
        RingProperties nextToLastRing = rings[rings.Count - 2];

        if (1 != lastRing.nextScene)
        {
            lastRing.gameObject.SetActive(false);
            rings.Remove(lastRing);
        }
        else
        {
            nextToLastRing.gameObject.SetActive(false);
            rings.Remove(nextToLastRing);
            lastRing.positionInOrder -= 1;
        }
    }
    private void setRingsMode(List<RingProperties> rings)
    {
        switch (mode)
        {
            case GameMode.Continuous:
                arrowScript.currentlyLookingAt = 1;
                break;
            case GameMode.Cursed:
                RemoveNEXTRing(rings);
                arrowScript.currentlyLookingAt = 1;
                break;
            case GameMode.Free:
                for (int i = 0; i < rings.Count - 2; ++i)
                    rings[i].gameObject.SetActive(false);
                arrowScript.currentlyLookingAt = rings.Count - 1;
                break;
            case GameMode.Race:
                RemoveNEXTRing(rings);
                arrowScript.currentlyLookingAt = 1;
                break;
        }
        if (null != lapTrigger && (GameMode.Cursed == mode || GameMode.Race == mode) && GameManager.MaxLap > 1)
            Instantiate(lapTrigger, rings[0].GetComponent<Transform>().position, rings[0].GetComponent<Transform>().rotation).GetComponent<PositionInOrderResetter>().MaxLap = GameManager.MaxLap;
    }
    private void InsertionSort(RingProperties[] rings)
    {
        int currRing = 1;
        while (currRing < rings.Length)
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
    private void SetupStartBonusTime()
    {
        PlayerMovementVariables currPMV = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables;
        RoundTimer.timeLeft = (3.0f * Vector3.Distance(GameManager.player.transform.position, sortedRings[0].transform.position) / (currPMV.minSpeed + currPMV.restingSpeed + currPMV.maxSpeed)) + 5.0f;
    }
    private void OnDisable()
    {
        if (null != arrowScript)
        {
            arrowScript.thingsToLookAt = null;
            arrowScript.currentlyLookingAt = -1;
        }
    }
}