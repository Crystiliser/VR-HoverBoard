using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class trailStripCreator : MonoBehaviour
{
    [SerializeField] private bool predictiveTrailEnabled = true;
    private int currentSceneIndex = 0;
    private Rigidbody playerRigidBody = null;
    [SerializeField] private GameObject cookiePrefab = null;
    private MeshRenderer[] cookieRenderers = null;
    private Transform[] cookieTransforms = null;
    private bool cookiesDisabled = true;
    [SerializeField] float timeIncrement = 0.1f;
    [SerializeField] [Range(2, 10)] private int numberOfPoints = 4;
    [SerializeField] [Range(1, 20)] private int startingDistance = 3;
    [SerializeField] [Range(0.0f, 1.0f)] private float rotationLerpAmount = 0.05f;
    private Quaternion pRot = Quaternion.identity, cRot = Quaternion.identity;
    private Vector3 prevRotation, currRotation, predictedPosition, predictedRotation;
    private float xRotationDiff, yRotataionDiff, xRotationPrediction, yRotationPrediction;
    private const float inverseDeltaTime = 50.0f;
    public static trailStripCreator inst = null;
    public bool PredictivePathEnabled { get { return predictiveTrailEnabled; } set { TogglePredictiveTrail(value); } }
    private void OnEnable()
    {
        inst = this;
        GameSettings.GetBool("PredictiveTrail", ref predictiveTrailEnabled);
        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        if (this == inst)
            inst = null;
    }
    public void TogglePredictiveTrail(bool enabled)
    {
        predictiveTrailEnabled = enabled;
        SetupCookies();
        UpdateCookieDisplay();
    }
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        SetupCookies();
        UpdateCookieDisplay();
    }
    private void SetupCookies()
    {
        if (null == cookieRenderers)
        {
            playerRigidBody = GameManager.player.GetComponent<Rigidbody>();
            cookieRenderers = new MeshRenderer[numberOfPoints];
            cookieTransforms = new Transform[numberOfPoints];
            for (int i = 0; i < numberOfPoints; ++i)
            {
                GameObject go = Instantiate(cookiePrefab, GameManager.player.transform);
                cookieRenderers[i] = go.GetComponent<MeshRenderer>();
                cookieTransforms[i] = go.transform;
                cookieRenderers[i].enabled = false;
            }
        }
    }
    private void UpdateCookieDisplay()
    {
        if (predictiveTrailEnabled && currentSceneIndex >= LevelManager.LevelBuildOffset)
        {
            if (cookiesDisabled)
            {
                foreach (MeshRenderer item in cookieRenderers)
                    item.enabled = true;
                prevRotation = currRotation = Vector3.zero;
                StartCoroutine(PredictiveCoroutine());
                cookiesDisabled = false;
            }
        }
        else if (!cookiesDisabled)
        {
            foreach (MeshRenderer item in cookieRenderers)
                item.enabled = false;
            StopAllCoroutines();
            cookiesDisabled = true;
        }
    }
    private IEnumerator PredictiveCoroutine()
    {
        yield return new WaitForFixedUpdate();
        cRot = Quaternion.LerpUnclamped(pRot, GameManager.player.transform.rotation, rotationLerpAmount);
        currRotation = cRot.eulerAngles;
        xRotationDiff = Mathf.DeltaAngle(prevRotation.x, currRotation.x);
        yRotataionDiff = Mathf.DeltaAngle(prevRotation.y, currRotation.y);
        for (int i = 0; i < numberOfPoints; ++i)
        {
            xRotationPrediction = xRotationDiff * timeIncrement * (i + startingDistance) * inverseDeltaTime;
            yRotationPrediction = yRotataionDiff * timeIncrement * (i + startingDistance) * inverseDeltaTime;
            predictedRotation = new Vector3(currRotation.x + xRotationPrediction, currRotation.y + yRotationPrediction, 0f);
            cookieTransforms[i].rotation = Quaternion.Euler(predictedRotation);
            if (0 == i)
                cookieTransforms[i].position = GameManager.player.transform.position;
            else
                cookieTransforms[i].position = cookieTransforms[i - 1].position;
            cookieTransforms[i].Translate(cookieTransforms[i].forward * playerRigidBody.velocity.magnitude * timeIncrement * (i + startingDistance), Space.World);
        }
        pRot = cRot;
        prevRotation = currRotation;
        StartCoroutine(PredictiveCoroutine());
    }
}