using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingScoreScript : MonoBehaviour
{
    [System.Serializable]
    public class ScoreMultipliers
    {
        public float speedMultiplier;
        public float consecutiveMultiplier;
        public float consecutiveIncreaseAmount;

        public ScoreMultipliers(float sMul, float crMul, float crInAmt) { speedMultiplier = sMul; consecutiveMultiplier = crMul; consecutiveIncreaseAmount = crInAmt; }
    }

    //static ring effect settings
    static int prevPositionInOrder;
    static int prevConsecutiveCount;
    static int consecutiveCount;
    static bool effectsStopped;

    public static ScoreMultipliers multipliers = new ScoreMultipliers(1.5f, 1f, 0.25f);
    playerCollisionSoundEffects pColSoundEffects;
    PlayerArrowHandler pArrowHandler;
    ScoreManager scoreManager;

    RingProperties rp;
    System.Type capsuleType;

    float originalCrInAmt;
    float totalMultiplier;

    bonusTimeTextUpdater bonusTimeText;

    [SerializeField] float[] boardTimeModifiers = new float[(int)BoardType.Custom + 1];

    private void Start()
    {
        scoreManager = GameManager.instance.scoreScript;
        pColSoundEffects = GameManager.player.GetComponent<playerCollisionSoundEffects>();
        pArrowHandler = GameManager.player.GetComponent<PlayerArrowHandler>();
        rp = GetComponent<RingProperties>();
        capsuleType = typeof(CapsuleCollider);
        bonusTimeText = GameManager.player.GetComponentInChildren<bonusTimeTextUpdater>();

        //set our prevRingInOrder to -1, so we don't apply a consecutive score multiplier for the very first ring we go through in a scene
        prevPositionInOrder = -1;
        consecutiveCount = 0;
        effectsStopped = true;

        originalCrInAmt = multipliers.consecutiveIncreaseAmount;
    }

    void IncreaseScore()
    {
        totalMultiplier = 1f;

        if (prevPositionInOrder + 1 == rp.positionInOrder)
        {
            //use our consecutive multiplier if this ring comes immediately after the previous one
            totalMultiplier += multipliers.consecutiveMultiplier;

            //then increase by our increase amount
            multipliers.consecutiveMultiplier += multipliers.consecutiveIncreaseAmount;

            ++consecutiveCount;
        }
        else
        {
            //otherwise, reset the consecutiveMultiplier
            multipliers.consecutiveMultiplier = originalCrInAmt;
            consecutiveCount = prevConsecutiveCount = 0;

            if (!effectsStopped)
            {
                effectsStopped = true;
                EventManagerRings.OnStopRingPulse();
            }
        }

        scoreManager.score += (int)(scoreManager.baseScorePerRing * totalMultiplier);
        //print("Score is now: " + scoreManager.score);
    }

    void UpdateRingEffects()
    {
        if (consecutiveCount > prevConsecutiveCount)
        {
            switch (consecutiveCount)
            {
                case 3:
                    if (effectsStopped)
                    {
                        effectsStopped = false;
                        EventManagerRings.OnStartRingPulse();
                    }
                    break;
                case 5:
                    break;
                case 10:
                    break;
                default:
                    break;
            }

            prevConsecutiveCount = consecutiveCount;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == capsuleType)
        {
            pArrowHandler.UpdatePlayerHUDPointer(rp);

            if (rp.positionInOrder > prevPositionInOrder)
            {
                //update our scoreManager values
                scoreManager.prevRingBonusTime = rp.bonusTime;
                scoreManager.prevRingTransform = rp.transform;
                scoreManager.roundTimer.IncreaseTimeLeft(rp.bonusTime + boardTimeModifiers[(int)GameManager.instance.boardScript.currentBoardSelection]);
                scoreManager.ringHitCount++;

                if (GameManager.instance.gameMode.currentMode == GameModes.Cursed)
                {
                    bonusTimeText.play((rp.bonusTime + boardTimeModifiers[(int)GameManager.instance.boardScript.currentBoardSelection]).ToString("n2"));
                }

                IncreaseScore();
                UpdateRingEffects();
                pColSoundEffects.PlayRingClip(gameObject);

                prevPositionInOrder = rp.positionInOrder;
            }

            if (rp.lastRingInScene)
            {
                scoreManager.levelEnd();

                //update our scoreManager values
                scoreManager.prevRingBonusTime = 0f;
                scoreManager.prevRingTransform = GameManager.instance.levelScript.spawnPoints[rp.nextScene];
                scoreManager.ringHitCount = 0;

                //TODO:: store the total time in scene someplace then reset it.....
                prevPositionInOrder = -1;

                EventManager.OnTriggerTransition(rp.nextScene);
            }
        }
    }
}
