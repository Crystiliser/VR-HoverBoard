using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private GameObject theFadeObj = null;
    private Rigidbody playerRB = null;
    private Transform respawnPoint = null;
    private float countDownTime = 3.25f, timeIntoFade = 0.0f, timeInPitchBlack = 0.1f, roundTimerStartTime = 5.0f;
    private CameraSplashScreen countdown = null;
    private bool isRespawning = false;
    public bool IsRespawning { get { return isRespawning; } }
    private void Start()
    {
        countdown = GetComponentInChildren<CameraSplashScreen>();
        playerRB = GameManager.player.GetComponent<Rigidbody>();
    }
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isRespawning)
        {
            StopAllCoroutines();
            RoundTimer.timersPaused = false;
            isRespawning = false;
            respawnPoint = null;
        }
    }
    public void RespawnPlayer(Transform rsPoint, float startTime, float countDownFrom = 3.25f)
    {
        if (!isRespawning && GameState.GamePlay == GameManager.gameState)
        {
            RoundTimer.timeLeft = 0.0f;
            respawnPoint = rsPoint;
            roundTimerStartTime = startTime;
            countDownTime = countDownFrom;
            StartCoroutine(FadeOut());
        }
    }
    private void UpdateAlpha(bool fadingOut)
    {
        float alpha = timeIntoFade;
        if (!fadingOut)
            alpha = 1.0f - alpha;
        theFadeObj.GetComponent<Renderer>().material.SetFloat("_AlphaValue", Mathf.Clamp01(alpha - 0.01f));
    }
    private IEnumerator FadeOut()
    {
        timeIntoFade = 0.0f;
        isRespawning = true;
        while (timeIntoFade < 1.0f + timeInPitchBlack)
        {
            UpdateAlpha(true);
            timeIntoFade += Time.deltaTime;
            yield return null;
        }
        EventManager.OnSetGameplayMovementLock(true);
        if (null != respawnPoint)
        {
            playerRB.MovePosition(respawnPoint.position + respawnPoint.forward * 2.0f);
            playerRB.MoveRotation(Quaternion.Euler(respawnPoint.eulerAngles.x, respawnPoint.eulerAngles.y, 0.0f));
        }
        if (GameState.GamePlay == GameManager.gameState)
            StartCoroutine(FadeIn());
    }
    private IEnumerator FadeIn()
    {
        timeIntoFade = 0.0f;
        RoundTimer.timeLeft = roundTimerStartTime;
        RoundTimer.timersPaused = true;
        bool countdownStarted = false;
        while (timeIntoFade < 1.0f && GameState.GamePlay == GameManager.gameState)
        {
            UpdateAlpha(false);
            if (!countdownStarted && timeIntoFade > 0.25f)
            {
                countdown.StartCountdown(countDownTime);
                countdownStarted = true;
            }
            timeIntoFade += Time.deltaTime;
            yield return null;
        }
        if (GameState.GamePlay == GameManager.gameState)
        {
            if (countDownTime - timeIntoFade > 0.0f)
                yield return new WaitForSeconds(countDownTime - timeIntoFade);
            yield return null;
            if (GameState.GamePlay == GameManager.gameState)
                EventManager.OnSetGameplayMovementLock(false);
        }
        RoundTimer.timersPaused = false;
        isRespawning = false;
        respawnPoint = null;
    }
    private void OnEnable() => SceneManager.sceneLoaded += OnLevelLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnLevelLoaded;
}