using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] Image theFadeObj;

    float alpha = 1.0f;
    float fadeTime = 0.5f;
    float timeIntoFade = 0f;
    float quaterFadeTime;
    float countDownTime = 3.25f;

    AsyncOperation asyncOp;
    CameraSplashScreen countdown;
    ManagerClasses.RoundTimer roundTimer;

    //used for scene transitions
    ManagerClasses.GameState gameState;

    private void Awake()
    {
        gameState = GameManager.instance.gameState;
        roundTimer = GameManager.instance.roundTimer;
        countdown = GetComponentInChildren<CameraSplashScreen>();
        quaterFadeTime = fadeTime * 0.25f;
    }

    // Fades alpha from 1.0 to 0.0, use at beginning of scene
    IEnumerator FadeIn()
    {
        roundTimer.PauseTimer(true);

        //don't start the fade in until loading is done
        while (asyncOp != null && !asyncOp.isDone)
            yield return null;

        bool usingCountdown, countdownStarted = false;
        timeIntoFade = 0f;

        //only use our countdown if we are going to a gameplay scene
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
        {
            //if we are transitioning from one gameplay level to another, just start player movement without a countdown
            if (GameManager.instance.lastBuildIndex != 1)
                usingCountdown = false;
            else
                usingCountdown = true;
        }
        else
            usingCountdown = false;       

        while (timeIntoFade < fadeTime)
        {
            timeIntoFade += Time.deltaTime;
            UpdateAlpha(false);

            if (usingCountdown && !countdownStarted && timeIntoFade > quaterFadeTime)
            {
                countdown.StartCountdown(countDownTime);
                countdownStarted = true;
            }

            yield return null;
        }

        //wait for the countdown to finish if we need to
        if (usingCountdown && countDownTime - timeIntoFade > 0f)
            yield return new WaitForSeconds(countDownTime - timeIntoFade);

        //only unlock player movement when we are in a gameplay scene
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
            EventManager.OnSetGameplayMovementLock(false);

        roundTimer.PauseTimer(false);
    }

    // Fades from 0.0 to 1.0, use at end of scene
    IEnumerator FadeOut()
    {
        timeIntoFade = 0f;

        //lock player gameplay movement
        EventManager.OnSetGameplayMovementLock(true);

        //make sure our state updates
        gameState.currentState = GameStates.SceneTransition;

        while (timeIntoFade < fadeTime)
        {
            timeIntoFade += Time.deltaTime;
            UpdateAlpha(true);

            yield return null;
        }

        //update our last level before changing scenes
        GameManager.instance.lastBuildIndex = SceneManager.GetActiveScene().buildIndex;

        asyncOp = SceneManager.LoadSceneAsync(GameManager.instance.levelScript.nextScene, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;
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

    // Starts a fade in when a new level is loaded
    void OnLevelFinished(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void StartTransitionFade()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinished;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinished;
    }
}
