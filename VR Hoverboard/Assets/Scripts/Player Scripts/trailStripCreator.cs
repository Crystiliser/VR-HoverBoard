using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trailStripCreator : MonoBehaviour
{
    [SerializeField] bool predictiveTrailEnabled = true;
    public bool PredictivePathEnabled { get { return predictiveTrailEnabled; } set { TogglePredictiveTrail(value); } }
    int currentSceneIndex = 0;

    Transform playerTransform;
    Rigidbody playerRigidBody;

    [SerializeField] GameObject cookiePrefab;
    MeshRenderer[] cookieRenderers;
    Transform[] cookieTransforms;
    bool cookiesDisabled = true;

    [SerializeField] float timeIncrement = 0.1f;
    [SerializeField] [Range(2, 10)] int numberOfPoints = 4;
    [SerializeField] [Range(1, 20)] int startingDistance = 3;
    [SerializeField] [Range(0f, 1f)] float rotationLerpAmount = .05f;

    Quaternion pRot = Quaternion.identity, cRot = Quaternion.identity;
    Vector3 prevRotation, currRotation, predictedPosition, predictedRotation;
    float xRotationDiff, yRotataionDiff, xRotationPrediction, yRotationPrediction;

    //since we're using a fixed time step, use the inverse of our time step in order to prevent divisions every update. 1 / 0.02 = 50
    float inverseDeltaTime = 50f;

    //use to enable or disable the trail
    public void TogglePredictiveTrail(bool enabled)
    {
        predictiveTrailEnabled = enabled;

        SetupCookies();
        UpdateCookieDisplay();
    }

    //update our scene index and trail every time a scene is loaded
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        //update our currentSceneIndex
        currentSceneIndex = scene.buildIndex;

        //make sure everything is initialized
        SetupCookies();

        //enable/disable cookies depending on scene index
        UpdateCookieDisplay();
    }

    //helper function
    void SetupCookies()
    {
        if (cookieRenderers == null)
        {
            playerRigidBody = GameManager.player.GetComponent<Rigidbody>();

            playerTransform = GameManager.player.GetComponent<Transform>();

            cookieRenderers = new MeshRenderer[numberOfPoints];
            cookieTransforms = new Transform[numberOfPoints];

            for (int i = 0; i < numberOfPoints; ++i)
            {
                GameObject go = Instantiate(cookiePrefab);
                DontDestroyOnLoad(go);
                go.transform.parent = GameManager.player.GetComponent<Transform>();

                cookieRenderers[i] = go.GetComponent<MeshRenderer>();
                cookieTransforms[i] = go.GetComponent<Transform>();

                //start the renderers as disabled, since the cookiesDisabled bool defaults to true
                cookieRenderers[i].enabled = false;
            }
        }
    }

    //helper function
    void UpdateCookieDisplay()
    {
        //set up our cookies if we are in a gameplay scene
        if (currentSceneIndex != 0 && currentSceneIndex != 1 && predictiveTrailEnabled)
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
        //otherwise, disable them
        else if (!cookiesDisabled)
        {
            foreach (MeshRenderer item in cookieRenderers)
                item.enabled = false;

            StopAllCoroutines();
            cookiesDisabled = true;
        }
    }

    IEnumerator PredictiveCoroutine()
    {
        yield return new WaitForFixedUpdate();

        cRot = Quaternion.LerpUnclamped(pRot, playerTransform.rotation, rotationLerpAmount);
        currRotation = cRot.eulerAngles;

        //currRotation = playerTransform.eulerAngles;

        //rotation difference in .02 seconds since it's fixed update
        xRotationDiff = Mathf.DeltaAngle(prevRotation.x, currRotation.x);
        yRotataionDiff = Mathf.DeltaAngle(prevRotation.y, currRotation.y);

        for (int i = 0; i < numberOfPoints; i++)
        {
            xRotationPrediction = xRotationDiff * timeIncrement * (i + startingDistance) * inverseDeltaTime;
            yRotationPrediction = yRotataionDiff * timeIncrement * (i + startingDistance) * inverseDeltaTime;

            predictedRotation = new Vector3(currRotation.x + xRotationPrediction, currRotation.y + yRotationPrediction, 0f);
            cookieTransforms[i].rotation = Quaternion.Euler(predictedRotation);

            if (i == 0)
                //set the first cookie position to the player's position
                cookieTransforms[i].position = playerTransform.position;
            else
                //set every other cookie position to the previous cookie position
                cookieTransforms[i].position = cookieTransforms[i - 1].position;

            //TODO:: couldn't we just calculate the player's speed once per update, then use that in every script that needs the velocity.magnitude...?
            cookieTransforms[i].Translate(cookieTransforms[i].forward * playerRigidBody.velocity.magnitude * timeIncrement * (i + startingDistance), Space.World);
        }

        pRot = cRot;
        prevRotation = currRotation;

        StartCoroutine(PredictiveCoroutine());
    }
    public static trailStripCreator inst;

    private void OnEnable()
    {
        inst = this;
        predictiveTrailEnabled = 0 != PlayerPrefs.GetInt("PredictiveTrail", predictiveTrailEnabled ? 1 : 0);
        PlayerPrefs.SetInt("PredictiveTrail", predictiveTrailEnabled ? 1 : 0);
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        if (this == inst)
            inst = null;
    }

}
