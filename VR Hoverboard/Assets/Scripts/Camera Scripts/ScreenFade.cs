using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScreenFade : MonoBehaviour
{
    [SerializeField] private GameObject theFadeObj = null;
    private const float fadeTime = 0.5f, quaterFadeTime = fadeTime * 0.25f, countDownTime = 3.25f;
    private float timeIntoFade = 0.0f;
    private int lastBuildIndex = -1;
    private AsyncOperation asyncOp = null;
    private CameraSplashScreen countdown = null;
    private void Awake()
    {
        countdown = GetComponentInChildren<CameraSplashScreen>();
    }
    private IEnumerator FadeIn()
    {
        GameManager.player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        RoundTimer.timersPaused = true;
        while (null != asyncOp && !asyncOp.isDone)
            yield return null;
        bool countdownStarted = false, usingCountdown;
        timeIntoFade = 0.0f;
        usingCountdown = ((SceneManager.GetActiveScene().buildIndex >= LevelManager.LevelBuildOffset) ? (LevelManager.HubWorldBuildIndex == lastBuildIndex) : false);
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
        if (usingCountdown && countDownTime - timeIntoFade > 0.0f)
            yield return new WaitForSeconds(countDownTime - timeIntoFade);
        if (SceneManager.GetActiveScene().buildIndex >= LevelManager.LevelBuildOffset)
            EventManager.OnSetGameplayMovementLock(false);
        RoundTimer.timersPaused = false;
    }
    private IEnumerator FadeOut()
    {
        timeIntoFade = 0.0f;
        EventManager.OnSetGameplayMovementLock(true);
        GameManager.gameState = GameState.SceneTransition;
        while (timeIntoFade < fadeTime)
        {
            timeIntoFade += Time.deltaTime;
            UpdateAlpha(true);
            yield return null;
        }
        lastBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = LevelManager.nextScene;
        if (nextScene >= LevelManager.LevelBuildOffset)
            nextScene += LevelManager.GetLevelOffset;
        asyncOp = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;
    }
    private void UpdateAlpha(bool fadingOut)
    {
        float alpha = timeIntoFade / fadeTime;
        if (!fadingOut)
            alpha = 1.0f - alpha;
        theFadeObj.GetComponent<Renderer>().material.SetFloat("_AlphaValue", Mathf.Clamp01(alpha));
    }
    private void OnLevelFinished(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }
    public void StartTransitionFade()
    {
        if (GameState.SceneTransition != GameManager.gameState)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }
    private void OnEnable() => SceneManager.sceneLoaded += OnLevelFinished;
    private void OnDisable() => SceneManager.sceneLoaded -= OnLevelFinished;
}