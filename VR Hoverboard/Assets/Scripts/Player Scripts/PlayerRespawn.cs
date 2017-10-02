using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public Image theFadeObj;
    float fadeTime = 1.0f;

    Rigidbody playerRB;
    Transform respawnPoint;
    float countDownTime;
    float alpha;
    float timeIntoFade;
    float timeInPitchBlack = 0.1f;

    CameraSplashScreen countdown;
    ManagerClasses.RoundTimer roundTimer;
    float roundTimerStartTime;

    bool isRespawning = false;


    public bool IsRespawning { get { return isRespawning; } }

    private void Start()
    {
        countdown = GetComponentInChildren<CameraSplashScreen>();
        playerRB = GameManager.player.GetComponent<Rigidbody>();
        roundTimer = GameManager.instance.roundTimer;
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        //stop our respawn coroutines if we've switched scenes
        if (isRespawning)
        {
            StopAllCoroutines();

            roundTimer.PauseTimer(false);

            isRespawning = false;
            respawnPoint = null;
        }
    }

    public void RespawnPlayer(Transform rsPoint, float startTime, float countDownFrom = 3.25f)
    {
        if (!isRespawning)
        {
            roundTimer.TimeLeft = 0f;

            respawnPoint = rsPoint;
            roundTimerStartTime = startTime;
            countDownTime = countDownFrom;

            alpha = theFadeObj.color.a;           
            StartCoroutine(FadeOut());
        }     
    }

    void UpdateAlpha(bool fadingOut)
    {
        //we want to update our alpha based off of how far into the fade time we are in
        alpha = timeIntoFade / fadeTime;

        if (fadingOut == false)
            alpha = 1f - alpha;

        alpha = Mathf.Clamp01(alpha);
        theFadeObj.material.color = new Color(0f, 0f, 0f, alpha);
    }

    IEnumerator FadeOut()
    {
        timeIntoFade = 0f;
        isRespawning = true;

        //keep going until our fade time as elapsed
        while (timeIntoFade < fadeTime + timeInPitchBlack)
        {
            //only update our alpha if it isn't 0
            UpdateAlpha(true);

            timeIntoFade += Time.deltaTime;
            yield return null;
        }

        //lock gameplay movement once we fade out
        EventManager.OnSetGameplayMovementLock(true);

        //once we fade out, move the player
        if (respawnPoint != null)
        {
            playerRB.MovePosition(respawnPoint.position + respawnPoint.forward * 2f);
            playerRB.MoveRotation(Quaternion.Euler(respawnPoint.eulerAngles.x, respawnPoint.eulerAngles.y, 0f));
        }


        //then start to fade in   
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        timeIntoFade = 0f;

        //start the timer
        roundTimer.TimeLeft = roundTimerStartTime;

        //pause the timer
        roundTimer.PauseTimer(true);

        float quarterFadeTime = fadeTime * 0.25f;
        bool countdownStarted = false;

        while (timeIntoFade < fadeTime)
        {
            UpdateAlpha(false);

            if (!countdownStarted && timeIntoFade > quarterFadeTime)
            {
                countdown.StartCountdown(countDownTime);
                countdownStarted = true;
            }

            timeIntoFade += Time.deltaTime;
            yield return null;
        }

        //wait for the countdown to finish if we need to
        if (countDownTime - timeIntoFade > 0f)
            yield return new WaitForSeconds(countDownTime - timeIntoFade);

        //unlock movement
        EventManager.OnSetGameplayMovementLock(false);

        //unpause the timer
        roundTimer.PauseTimer(false);


        isRespawning = false;
        respawnPoint = null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

}

