using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingProperties : MonoBehaviour
{
    //assign through the inspector
    public bool duplicatePosition = false;
    public int positionInOrder = 0;
    public float bonusTime = 0.0f;
    public bool lastRingInScene = false;
    public int nextScene = 1;


    [SerializeField] public float timeBonusOriginal = 10.0f;
    [SerializeField] public float timeBonusMachI = 8.0f;
    [SerializeField] public float timeBonusMachII = 0.0f;
    [SerializeField] public float timeBonusMachIII = -4.0f;

    //if we have children, set their values, but only if this is marked as a duplicate
    private void Awake()
    {
        if (duplicatePosition)
        {
            RingProperties[] rps = GetComponentsInChildren<RingProperties>();

            foreach (RingProperties rp in rps)
            {
                rp.duplicatePosition = duplicatePosition;
                rp.positionInOrder = positionInOrder;
                rp.bonusTime = bonusTime;
                rp.lastRingInScene = lastRingInScene;
                rp.nextScene = nextScene;
            }
        }
    }

    private void Start()
    {
        BoardManager boardScript = GameManager.instance.boardScript;
        float max;
        float min;
        float average;
        if (boardScript.gamepadEnabled)
        {
            max = boardScript.customGamepadMovementVariables.maxSpeed;
            min = boardScript.customGamepadMovementVariables.minSpeed;
            average = (min) + ((max-min) * 0.5f);
        }
        else
        {
            max = boardScript.customGyroMovementVariables.maxSpeed;
            min = boardScript.customGyroMovementVariables.minSpeed;
            average = (min) + ((max - min) * 0.5f);
        }
        timeBonusOriginal = (average / max) * 0.6f;
        timeBonusMachI = (average / max) * 0.1f;
        timeBonusMachII = 0;
        timeBonusMachIII = -timeBonusMachI;

    }
}
