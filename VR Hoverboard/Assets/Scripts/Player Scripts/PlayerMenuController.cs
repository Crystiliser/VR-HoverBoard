using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class PlayerMenuController : MonoBehaviour
{
    [SerializeField, Range(2.0f, 60.0f)] private float hoverForce = 15.0f;
    [SerializeField, Range(0.1f, 10.0f)] private float hoverHeight = 2.0f;

    [Header("Controller Specific Variables"), SerializeField, Range(5.0f, 60.0f)] private float controllerForwardSpeed = 25.0f;
    [SerializeField, Range(5.0f, 60.0f)] private float controllerTurnSpeed = 12.0f;
    [SerializeField, Range(0.1f, 5.0f)] private float debugCameraSpeed = 1.0f;

    [Header("Gyro Specific Variables"), SerializeField, Range(1.0f, 3.0f)] private float gyroForwardSpeed = 1.3f;
    [SerializeField, Range(0.1f, 3.0f)] private float gyroTurnSpeed = 0.3f;
    [SerializeField, Range(0.0f, 30.0f)] private float gyroPitchDeadZone = 4.0f;
    [SerializeField, Range(0.0f, 30.0f)] private float gyroYawDeadZone = 5.0f;
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private bool coroutinesStopped = false;
    private bool gamepadEnabled = false;
    private bool inAMenu = false;
    private bool menuMovementIsLocked = false;

    private Rigidbody playerRB = null;
    private Transform cameraContainerTransform = null;
    private SpatialData gyro = null;

    //called by our BoardManager
    public void SetupMenuControllerScript()
    {
        cameraContainerTransform = GameManager.player.GetComponentInChildren<CameraCounterRotate>().transform;
        gyro = GameManager.instance.boardScript.gyro;
        gamepadEnabled = GameManager.instance.boardScript.gamepadEnabled;

        coroutinesStopped = false;
        menuMovementIsLocked = false;
    }

    private bool lockingMotion = false;
    private float tVal = 0.0f;
    private Vector3 startMotionPos, endMotionPos;
    private Quaternion startMotionRot, endMotionRot;
    [Space, SerializeField]
    private float lockMotionTime = 0.75f;

    public delegate void MovementLockEvent();
    public MovementLockEvent OnPlayerLocking, OnPlayerLock, OnPlayerUnlock;
    public void LockPlayerToPosition(Vector3 worldPosition, Quaternion worldRotation)
    {
        lockingMotion = false;
        ToggleMenuMovement(true);
        startMotionPos = playerRB.transform.position;
        startMotionRot = playerRB.transform.rotation;
        endMotionPos = worldPosition;
        endMotionRot = worldRotation;
        tVal = 0.0f;
        lockingMotion = true;
        if (null != OnPlayerLocking)
            OnPlayerLocking();
    }
    public void UnlockPlayerPosition()
    {
        lockingMotion = false;
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
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (null == playerRB)
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

        if (inAMenu)
            if (gamepadEnabled)
                StartCoroutine(ControllerCoroutine());
            else
                StartCoroutine(GyroCoroutine());
    }

    //make sure we don't start rotating up/down or start to roll
    private void ClampRotation()
    {
        if (0.0f != playerRB.rotation.eulerAngles.z || 0.0f != playerRB.rotation.eulerAngles.x)
            playerRB.rotation = Quaternion.Euler(new Vector3(0.0f, playerRB.rotation.eulerAngles.y, 0.0f));
    }

    //let our right joystick control the camera if there is no HMD present
    private void DebugCameraRotation()
    {
        if (!VRDevice.isPresent)
        {
            float cameraPitch = cameraContainerTransform.eulerAngles.x + -Input.GetAxis("RVertical") * debugCameraSpeed;
            float cameraYaw = cameraContainerTransform.eulerAngles.y + Input.GetAxis("RHorizontal") * debugCameraSpeed;

            cameraContainerTransform.rotation = (Quaternion.Euler(new Vector3(cameraPitch, cameraYaw, 0.0f)));
        }
    }

    private void ApplyHoverForce()
    {
        Ray ray = new Ray(playerRB.position, -playerRB.transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) * hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            playerRB.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }
    }
    private bool spawnLock = false;
    private void ReturnToSpawnPoint()
    {
        lockingMotion = false;
        startMotionPos = playerRB.transform.position;
        startMotionRot = endMotionRot = playerRB.transform.rotation;
        endMotionPos = GameManager.instance.levelScript.spawnPoints[SceneManager.GetActiveScene().buildIndex].position;
        tVal = 0.0f;
        spawnLock = true;
        playerRB.GetComponent<CapsuleCollider>().enabled = false;
        lockingMotion = true;
    }

    private IEnumerator ControllerCoroutine()
    {
        yield return new WaitForFixedUpdate();

        ClampRotation();
        DebugCameraRotation();
        ApplyHoverForce();

        if (!menuMovementIsLocked)
        {
            if (Input.GetButtonDown("XBox Start") || Input.GetKeyDown(KeyCode.R))
                ReturnToSpawnPoint();
            playerRB.AddRelativeForce(0.0f, 0.0f, Input.GetAxis("LVertical") * controllerForwardSpeed);
            playerRB.AddRelativeTorque(0.0f, Input.GetAxis("LHorizontal") * controllerTurnSpeed, 0.0f);
        }

        StartCoroutine(ControllerCoroutine());
    }

    //helper function
    private void GyroApplyDeadZone()
    {
        //leaning forward
        if (pitch > 0.0f)
        {
            if (pitch < gyroPitchDeadZone)
                pitch = 0.0f;
        }
        else
        {
            if (pitch > -gyroPitchDeadZone)
                pitch = 0.0f;
        }


        //leaning left
        if (yaw > 0.0f)
        {
            if (yaw < gyroYawDeadZone)
                yaw = 0.0f;
        }
        else
        {
            if (yaw > -gyroYawDeadZone)
                yaw = 0.0f;
        }
    }

    private IEnumerator GyroCoroutine()
    {
        yield return new WaitForFixedUpdate();

        ClampRotation();
        DebugCameraRotation();
        ApplyHoverForce();

        pitch = (float)gyro.rollAngle * Mathf.Rad2Deg;
        yaw = (float)gyro.pitchAngle * Mathf.Rad2Deg * -1.0f;

        GyroApplyDeadZone();

        pitch *= gyroForwardSpeed;
        yaw *= gyroTurnSpeed;

        if (!menuMovementIsLocked)
        {
            playerRB.AddRelativeForce(0.0f, 0.0f, pitch);
            playerRB.AddRelativeTorque(0.0f, yaw, 0.0f);
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

    private void Update()
    {
        if (lockingMotion)
        {
            tVal += Time.deltaTime / lockMotionTime;
            if (tVal >= 1.0f)
            {
                playerRB.transform.position = endMotionPos;
                playerRB.transform.rotation = endMotionRot;
                lockingMotion = false;
                if (spawnLock)
                {
                    playerRB.GetComponent<CapsuleCollider>().enabled = true;
                    spawnLock = false;
                }
                else if (null != OnPlayerLock)
                    OnPlayerLock();
            }
            else
            {
                playerRB.transform.position = Vector3.Lerp(startMotionPos, endMotionPos, tVal);
                playerRB.transform.rotation = Quaternion.Slerp(startMotionRot, endMotionRot, tVal);
            }
        }
    }
}