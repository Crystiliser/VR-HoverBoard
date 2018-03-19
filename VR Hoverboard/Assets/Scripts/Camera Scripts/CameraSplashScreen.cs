using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class CameraSplashScreen : MonoBehaviour
{
    [SerializeField] private GameObject countdownTextElement = null;
    private TextMeshPro tmp = null;
    private effectController dustEffectController = null;
    private float maxSeconds = 0.0f;
    private void Start()
    {
        tmp = countdownTextElement.GetComponent<TextMeshPro>();
        maxSeconds = 0.0f;
        dustEffectController = GetComponentInParent<PlayerGameplayController>().GetComponentInChildren<effectController>();
    }
    public void StartCountdown(float countdownSeconds)
    {
        countdownTextElement.SetActive(true);
        maxSeconds = countdownSeconds;
        dustEffectController.DustField.Pause();
        StopAllCoroutines();
        StartCoroutine(CountdownCoroutine());
    }
    private IEnumerator CountdownCoroutine()
    {
        float currTime = 0.0f;
        while (currTime < maxSeconds)
        {
            tmp.SetText((maxSeconds - currTime).ToString("##"));
            currTime += Time.deltaTime;
            yield return null;
        }
        maxSeconds = 0.0f;
        dustEffectController.DustField.Play();
        countdownTextElement.SetActive(false);
    }
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        maxSeconds = 0.0f;
        countdownTextElement.SetActive(false);
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