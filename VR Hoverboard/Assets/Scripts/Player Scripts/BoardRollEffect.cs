using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BoardRollEffect : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRB = null;
    [SerializeField] private float rollIncreaseRate = 1.2f, rollDecreaseRate = 0.1f, maxRollDegree = 25.0f;
    [SerializeField] private float pitchIncreaseRate = 1.0f, pitchDecreaseRate = 0.5f, maxPitchDegree = 20.0f;
    private float zRotation = 0.0f, prevYRotation = 0.0f, xRotation = 0.0f, forwardSpeed = 0.0f;
    private int currScene = 1;
    private void LevelSelectionUnlocked(bool locked)
    {
        if (!locked)
        {
            StopAllCoroutines();
            zRotation = 0.0f;
            prevYRotation = transform.eulerAngles.y;
            xRotation = 0.0f;
            StartCoroutine(BoardRollCoroutine());
        }
    }
    private void RollEffect()
    {
        if (prevYRotation != transform.eulerAngles.y)
        {
            zRotation = Mathf.Clamp(zRotation + Mathf.DeltaAngle(transform.eulerAngles.y, prevYRotation) * rollIncreaseRate, -maxRollDegree, maxRollDegree);
            prevYRotation = transform.eulerAngles.y;
        }
        if (0.0f != zRotation)
        {
            zRotation = Mathf.Lerp(zRotation, 0.0f, rollDecreaseRate);
            if (-0.1f < zRotation && zRotation < 0.1f)
                zRotation = 0.0f;
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
        }
    }
    private void PitchEffect()
    {
        if (currScene < LevelManager.LevelBuildOffset)
        {
            forwardSpeed = transform.InverseTransformDirection(playerRB.velocity).z;
            if (forwardSpeed <= -0.1f || 0.1f <= forwardSpeed)
                xRotation = Mathf.Clamp(xRotation + forwardSpeed * pitchIncreaseRate, -maxPitchDegree, maxPitchDegree);
            if (0.0f != xRotation)
            {
                xRotation = Mathf.Lerp(xRotation, 0.0f, pitchDecreaseRate);
                if (-0.1f < xRotation && xRotation < 0.1f)
                    xRotation = 0.0f;
                transform.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
    }
    private IEnumerator BoardRollCoroutine()
    {
        yield return new WaitForFixedUpdate();
        RollEffect();
        PitchEffect();
        StartCoroutine(BoardRollCoroutine());
    }
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode) => currScene = SceneManager.GetActiveScene().buildIndex;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
        EventManager.OnSelectionLock += LevelSelectionUnlocked;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        EventManager.OnSelectionLock -= LevelSelectionUnlocked;
    }
}