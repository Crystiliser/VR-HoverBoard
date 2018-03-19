using UnityEngine;
using Xander.NullConversion;
public class RingScoreScript : MonoBehaviour
{
    private float consecutiveMultiplier = 0.25f;
    private Position_Text_script Player_position = null;
    private const float consecutiveIncreaseAmount = 0.25f;
    private static readonly System.Type capsuleType = typeof(CapsuleCollider);
    private static int prevPositionInOrder = -1, prevConsecutiveCount = 0, consecutiveCount = 0;
    private static bool effectsStopped = true;
    private playerCollisionSoundEffects pColSoundEffects = null;
    private PlayerArrowHandler pArrowHandler = null;
    private RingProperties rp = null;
    private float originalCrInAmt = 0.25f;
    private bonusTimeTextUpdater bonusTimeText = null;
    private ParticleSystem hitEffect = null;
    private PlayerRespawn respawnScript = null;
    [SerializeField] private GameObject portaleffect = null;
    public static void ResetPrevPositionInOrder() => prevPositionInOrder = 0;
    private void Start()
    {
        pColSoundEffects = GameManager.player.GetComponent<playerCollisionSoundEffects>();
        pArrowHandler = GameManager.player.GetComponent<PlayerArrowHandler>();
        rp = GetComponent<RingProperties>();
        bonusTimeText = GameManager.player.GetComponentInChildren<bonusTimeTextUpdater>(true);
        hitEffect = GetComponentInChildren<ParticleSystem>();
        respawnScript = GameManager.player.GetComponent<PlayerRespawn>();
        prevPositionInOrder = -1;
        consecutiveCount = 0;
        Player_position = FindObjectOfType<Position_Text_script>();
        effectsStopped = true;
        originalCrInAmt = consecutiveIncreaseAmount;
        if (rp.LastRingInScene && (GameMode.Race == GameManager.gameMode || GameMode.Cursed == GameManager.gameMode) && GameManager.MaxLap > 1)
            portaleffect.SetActive(false);
    }
    private void IncreaseScore()
    {
        float totalMultiplier = ScoreManager.score_multiplier;
        if (prevPositionInOrder + 1 == rp.positionInOrder)
        {
            totalMultiplier += consecutiveMultiplier;
            consecutiveMultiplier += consecutiveIncreaseAmount;
            ++consecutiveCount;
        }
        else
        {
            totalMultiplier = 1;
            consecutiveMultiplier = originalCrInAmt;
            consecutiveCount = prevConsecutiveCount = 0;
            if (!effectsStopped)
            {
                effectsStopped = true;
                EventManager.StopRingPulse();
            }
        }
        ScoreManager.score += (int)(ScoreManager.baseScorePerRing * totalMultiplier);
        ScoreManager.score_multiplier = totalMultiplier;
    }
    private void UpdateRingEffects()
    {
        if (consecutiveCount > prevConsecutiveCount)
        {
            if (effectsStopped && 3 == consecutiveCount)
            {
                effectsStopped = false;
                EventManager.StartRingPulse();
            }
            prevConsecutiveCount = consecutiveCount;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (null != respawnScript && !respawnScript.IsRespawning && capsuleType == other.GetType() && "Player" == other.tag)
        {
            pArrowHandler.UpdatePlayerHUDPointer(rp);

            if (rp.positionInOrder > prevPositionInOrder)
            {
                ScoreManager.prevRingBonusTime = rp.bonusTime;
                ScoreManager.prevRingTransform = rp.transform;
                ++ScoreManager.ringHitCount;
                if (GameMode.Cursed == GameManager.gameMode)
                {
                    RoundTimer.IncreaseTimeLeft(rp.bonusTime);
                    bonusTimeText.play((rp.bonusTime).ToString("n2"));
                }
                IncreaseScore();
                UpdateRingEffects();
                pColSoundEffects.PlayRingClip(gameObject);
                hitEffect.ConvertNull()?.Play();
                prevPositionInOrder = rp.positionInOrder;
            }
            if (rp.LastRingInScene)
            {
                prevPositionInOrder = -1;
                if (GameMode.Race != GameManager.gameMode && GameMode.Cursed != GameManager.gameMode)
                {
                    ScoreManager.LevelEnd();
                    ScoreManager.prevRingBonusTime = 0.0f;
                    ScoreManager.prevRingTransform = LevelManager.SpawnPoints[rp.nextScene];
                    ScoreManager.ringHitCount = 0;
                    prevPositionInOrder = -1;
                    EventManager.OnTriggerTransition(rp.nextScene);
                }
                else if (RingProperties.laptext.CurrLap == GameManager.MaxLap)
                {
                    ScoreManager.LevelEnd();
                    ScoreManager.prevRingBonusTime = 0.0f;
                    ScoreManager.prevRingTransform = LevelManager.SpawnPoints[rp.nextScene];
                    ScoreManager.ringHitCount = 0;
                    prevPositionInOrder = -1;
                    RingProperties.laptext.CurrLap = 1;
                    EventManager.OnTriggerTransition(rp.nextScene);
                }
                else if ((++RingProperties.laptext.CurrLap) == GameManager.MaxLap)
                    portaleffect.SetActive(true);
            }
        }
    }
}