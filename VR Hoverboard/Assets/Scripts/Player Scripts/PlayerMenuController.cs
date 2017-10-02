using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class PlayerMenuController : MonoBehaviour
{
    [Range(2f, 60f)] [SerializeField] float hoverForce = 15f;
    [Range(0.1f, 10.0f)] [SerializeField] float hoverHeight = 2f;

    [Header("Controller Specific Variables")]
    [Range(5f, 60f)]
    [SerializeField]
    float controllerForwardSpeed = 25f;
    [Range(5f, 60f)] [SerializeField] float controllerTurnSpeed = 12f;
    [Range(0.1f, 5f)] [SerializeField] float debugCameraSpeed = 1f;

    [Header("Gyro Specific Variables")]
    [Range(1f, 3f)]
    [SerializeField]
    float gyroForwardSpeed = 1.3f;
    [Range(0.1f, 3f)] [SerializeField] float gyroTurnSpeed = 0.3f;
    [Range(0.0f, 30.0f)] [SerializeField] float gyroPitchDeadZone = 4f;
    [Range(0.0f, 30.0f)] [SerializeField] float gyroYawDeadZone = 5f;
    float pitch;
    float yaw;

    float inverseHoverHeight;
    bool coroutinesStopped;
    bool gamepadEnabled;
    bool inAMenu;
    bool menuMovementIsLocked;

    Rigidbody playerRB;
    Transform cameraContainerTransform;
    SpatialData gyro;

    //called by our BoardManager
    public void SetupMenuControllerScript()
    {
        cameraContainerTransform = GameManager.player.GetComponentInChildren<CameraCounterRotate>().transform;
        gyro = GameManager.instance.boardScript.gyro;
        gamepadEnabled = GameManager.instance.boardScript.gamepadEnabled;

        inverseHoverHeight = hoverHeight / 1f;
        coroutinesStopped = false;
        menuMovementIsLocked = false;
    }

    public delegate void MovementLockEvent();
    public MovementLockEvent OnPlayerLock, OnPlayerUnlock;
    public void LockPlayerToPosition(Vector3 worldPosition)
    {
        ToggleMenuMovement(true);
        GameManager.player.transform.position = worldPosition;
        if (null != OnPlayerLock)
            OnPlayerLock();
    }
    public void LockPlayerToPosition(Vector3 worldPosition, Quaternion worldRotation)
    {
        ToggleMenuMovement(true);
        GameManager.player.transform.position = worldPosition;
        GameManager.player.transform.rotation = worldRotation;
        if (null != OnPlayerLock)
            OnPlayerLock();
    }
    public void UnlockPlayerPosition()
    {
        ToggleMenuMovement(false);
        if (null != OnPlayerUnlock)
            OnPlayerUnlock();
    }

    public void ToggleMenuMovement(bool locked)
    {
        menuMovementIsLocked = locked;
        playerRB.velocity = playerRB.angularVelocity = Vector3.zero;
        //print("Menu movement lock set to: " + menuMovementIsLocked);
    }

    //start our movement coroutines depending on if we are in a menu scene
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerRB == null)
            playerRB = GameManager.player.GetComponent<Rigidbody>();

        //if we're in options or hub world
        int buildIndex = scene.buildIndex;
        if (buildIndex == 0 || buildIndex == 1)
        {
            coroutinesStopped = false;
            inAMenu = true;
            playerRB.useGravity = true;

            //make sure not to have multiple coroutines going
            StopAllCoroutines();

            if (gamepadEnabled == true)
                StartCoroutine(ControllerCoroutine());
            else
                StartCoroutine(GyroCoroutine());
        }
        else if (!coroutinesStopped)
        {
            //reset our camera position to the player's rotation, if we were using debug camera rotation
            if (!VRDevice.isPresent)
                cameraContainerTransform.eulerAngles = playerRB.transform.eulerAngles;

            coroutinesStopped = true;
            inAMenu = false;
            playerRB.useGravity = false;

            StopAllCoroutines();
        }
    }

    //update our script depending on if we are using a xbox gamepad or the gyro
    //  Note: this should normally not be directly called, instead call the BoardManager's UpdateControlsType()
    public void UpdateMenuControlsType(bool gEnabled, SpatialData g)
    {
        gamepadEnabled = gEnabled;
        gyro = g;

        StopAllCoroutines();

        if (gamepadEnabled && inAMenu)
            StartCoroutine(ControllerCoroutine());
        else if (inAMenu)
            StartCoroutine(GyroCoroutine());
    }

    //make sure we don't start rotating up/down or start to roll
    void ClampRotation()
    {
        if (playerRB.rotation.eulerAngles.z != 0f || playerRB.rotation.eulerAngles.x != 0f)
            playerRB.rotation = Quaternion.Euler(new Vector3(0f, playerRB.rotation.eulerAngles.y, 0f));
    }

    //let our right thumbstick control the camera if there is no HMD present
    void DebugCameraRotation()
    {
        if (!VRDevice.isPresent)
        {
            float cameraPitch = cameraContainerTransform.eulerAngles.x + -Input.GetAxis("RVertical") * debugCameraSpeed;
            float cameraYaw = cameraContainerTransform.eulerAngles.y + Input.GetAxis("RHorizontal") * debugCameraSpeed;

            cameraContainerTransform.rotation = (Quaternion.Euler(new Vector3(cameraPitch, cameraYaw, 0f)));
        }
    }

    void ApplyHoverForce()
    {
        Ray ray = new Ray(playerRB.position, -playerRB.transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) * inverseHoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            playerRB.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }
    }

    private void ReturnToSpawnPoint()
    {
        playerRB.transform.position = GameManager.instance.levelScript.spawnPoints[1].position;
    }

    IEnumerator ControllerCoroutine()
    {
        yield return new WaitForFixedUpdate();

        ClampRotation();
        DebugCameraRotation();
        ApplyHoverForce();

        if (!menuMovementIsLocked)
        {
            if (Input.GetButtonDown("XBox Start") || Input.GetKeyDown(KeyCode.R))
                ReturnToSpawnPoint();
            playerRB.AddRelativeForce(0f, 0f, Input.GetAxis("LVertical") * controllerForwardSpeed);
            playerRB.AddRelativeTorque(0f, Input.GetAxis("LHorizontal") * controllerTurnSpeed, 0f);
        }

        StartCoroutine(ControllerCoroutine());
    }

    //helper function
    void GyroApplyDeadZone()
    {
        //leaning forward
        if (pitch > 0f)
        {
            if (pitch < gyroPitchDeadZone)
                pitch = 0f;
        }
        else
        {
            if (pitch > -gyroPitchDeadZone)
                pitch = 0f;
        }


        //leaning left
        if (yaw > 0f)
        {
            if (yaw < gyroYawDeadZone)
                yaw = 0f;
        }
        else
        {
            if (yaw > -gyroYawDeadZone)
                yaw = 0f;
        }
    }

    IEnumerator GyroCoroutine()
    {
        yield return new WaitForFixedUpdate();

        ClampRotation();
        DebugCameraRotation();
        ApplyHoverForce();

        pitch = (float)gyro.rollAngle * Mathf.Rad2Deg;
        yaw = (float)gyro.pitchAngle * Mathf.Rad2Deg * -1f;

        GyroApplyDeadZone();

        pitch *= gyroForwardSpeed;
        yaw *= gyroTurnSpeed;

        if (!menuMovementIsLocked)
        {
            playerRB.AddRelativeForce(0f, 0f, pitch);
            playerRB.AddRelativeTorque(0f, yaw, 0f);
        }

        StartCoroutine(GyroCoroutine());
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