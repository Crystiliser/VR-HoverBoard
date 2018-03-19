using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMenuController : MonoBehaviour
{
    public delegate void MovementLockEvent();
    public event MovementLockEvent OnPlayerLocking, OnPlayerLock, OnPlayerUnlock;
    [SerializeField, Range(2.0f, 60.0f)] private float hoverForce = 15.0f;
    [SerializeField, Range(0.1f, 10.0f)] private float hoverHeight = 2.0f;
    [Header("Controller Specific Variables"), SerializeField, Range(5.0f, 60.0f)] private float controllerForwardSpeed = 25.0f;
    [SerializeField, Range(5.0f, 60.0f)] private float controllerTurnSpeed = 12.0f;
    [Header("Gyro Specific Variables"), SerializeField, Range(1.0f, 3.0f)] private float gyroForwardSpeed = 1.3f;
    [SerializeField, Range(0.1f, 3.0f)] private float gyroTurnSpeed = 0.3f;
    [SerializeField, Range(0.0f, 30.0f)] private float gyroPitchDeadZone = 4.0f;
    [SerializeField, Range(0.0f, 30.0f)] private float gyroYawDeadZone = 5.0f;
    [Space, SerializeField] private float lockMotionTime = 0.75f;
    private float pitch = 0.0f, yaw = 0.0f, tVal = 0.0f;
    private bool coroutinesStopped = false, gamepadEnabled = false, inAMenu = false, menuMovementIsLocked = false, lockingMotion = false;
    private Rigidbody playerRB = null;
    private SpatialData gyro = null;
    private Vector3 startMotionPos, endMotionPos;
    private Quaternion startMotionRot, endMotionRot;
    public void SetupMenuControllerScript()
    {
        gyro = BoardManager.gyro;
        gamepadEnabled = BoardManager.gamepadEnabled;
        coroutinesStopped = false;
        menuMovementIsLocked = false;
    }
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
        OnPlayerLocking?.Invoke();
    }
    public void UnlockPlayerPosition()
    {
        lockingMotion = false;
        ToggleMenuMovement(false);
        OnPlayerUnlock?.Invoke();
    }
    public void ToggleMenuMovement(bool locked)
    {
        menuMovementIsLocked = locked;
        playerRB.velocity = playerRB.angularVelocity = Vector3.zero;
    }
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (null == playerRB)
            playerRB = GameManager.player.GetComponent<Rigidbody>();
        if (scene.buildIndex < LevelManager.LevelBuildOffset)
        {
            coroutinesStopped = false;
            inAMenu = true;
            playerRB.useGravity = true;
            StopAllCoroutines();
            if (gamepadEnabled)
                StartCoroutine(ControllerCoroutine());
            else
                StartCoroutine(GyroCoroutine());
        }
        else if (!coroutinesStopped)
        {
            coroutinesStopped = true;
            inAMenu = false;
            playerRB.useGravity = false;
            StopAllCoroutines();
        }
    }
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
    private void ClampRotation()
    {
        if (0.0f != playerRB.rotation.eulerAngles.z || 0.0f != playerRB.rotation.eulerAngles.x)
            playerRB.rotation = Quaternion.Euler(0.0f, playerRB.rotation.eulerAngles.y, 0.0f);
    }
    private void ApplyHoverForce()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(playerRB.position, -playerRB.transform.up), out hit, hoverHeight))
            playerRB.AddForce(0.0f, (hoverHeight - hit.distance) * hoverHeight * hoverForce, 0.0f, ForceMode.Acceleration);
    }
    private IEnumerator ControllerCoroutine()
    {
        yield return new WaitForFixedUpdate();
        ClampRotation();
        ApplyHoverForce();
        if (!menuMovementIsLocked)
        {
#if DEBUGGER
            if (BuildDebugger.WASD)
            {
                playerRB.AddRelativeForce(0.0f, 0.0f, Input.GetAxis("LVerticalWASD") * controllerForwardSpeed);
                playerRB.AddRelativeTorque(0.0f, Input.GetAxis("LHorizontalWASD") * controllerTurnSpeed, 0.0f);
            }
            else
#endif
            {
                playerRB.AddRelativeForce(0.0f, 0.0f, Input.GetAxis("LVertical") * controllerForwardSpeed);
                playerRB.AddRelativeTorque(0.0f, Input.GetAxis("LHorizontal") * controllerTurnSpeed, 0.0f);
            }
        }
        StartCoroutine(ControllerCoroutine());
    }
    private void GyroApplyDeadZone()
    {
        if (pitch > 0.0f)
        {
            if (pitch < gyroPitchDeadZone)
                pitch = 0.0f;
        }
        else if (pitch > -gyroPitchDeadZone)
            pitch = 0.0f;
        if (yaw > 0.0f)
        {
            if (yaw < gyroYawDeadZone)
                yaw = 0.0f;
        }
        else if (yaw > -gyroYawDeadZone)
            yaw = 0.0f;
    }
    private IEnumerator GyroCoroutine()
    {
        yield return new WaitForFixedUpdate();
        ClampRotation();
        ApplyHoverForce();
        while (null == gyro) yield return null;
        pitch = (float)gyro.rollAngle * Mathf.Rad2Deg;
        yaw = (float)gyro.pitchAngle * -Mathf.Rad2Deg;
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
    private void OnEnable() => SceneManager.sceneLoaded += OnLevelLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnLevelLoaded;
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
                OnPlayerLock?.Invoke();
            }
            else
            {
                playerRB.transform.position = Vector3.Lerp(startMotionPos, endMotionPos, tVal);
                playerRB.transform.rotation = Quaternion.Slerp(startMotionRot, endMotionRot, tVal);
            }
        }
    }
}