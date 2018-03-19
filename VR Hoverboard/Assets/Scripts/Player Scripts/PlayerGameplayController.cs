using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerGameplayController : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1.0f)] private float gryoPitchInterpolation = 0.075f;
    private bool gamepadEnabled = false, playerMovementLocked = true;
    private SpatialData gyro = null;
    public Rigidbody playerRigidbody = null;

    private float pitch = 0.0f, yaw = 0.0f, gyroPrevPitch = 0.0f, newAcceleration = 0.0f;
    public float currAcceleration = 0.0f;
    public PlayerMovementVariables movementVariables = null;
#if DEBUGGER
    private float debugSpeedIncrease = 0.0f;
    public float DebugSpeedIncrease { get { return debugSpeedIncrease; } }
    public void StartDebugSpeedControls() => debugSpeedIncrease = 0.0f;
#else
    private const float debugSpeedIncrease = 0.0f;
#endif
    public void SetupGameplayControllerScript()
    {
        gamepadEnabled = BoardManager.gamepadEnabled;
        playerRigidbody = GetComponent<Rigidbody>();
        gyro = BoardManager.gyro;
        gyroPrevPitch = 0.0f;
        newAcceleration = currAcceleration = 0.0f;
    }
    private void SetPlayerMovementLock(bool locked)
    {
        if (locked != playerMovementLocked)
        {
            StopAllCoroutines();
            if (locked)
            {
                playerRigidbody.velocity = Vector3.zero;
#if DEBUGGER
                debugSpeedIncrease = 0.0f;
#endif
            }
            else if (gamepadEnabled)
                StartCoroutine(GamepadMovementCoroutine());
            else
                StartCoroutine(GyroMovementCoroutine());
            playerMovementLocked = locked;
        }
    }
    public void UpdateGameplayControlsType(bool gEnabled, SpatialData g)
    {
        gamepadEnabled = gEnabled;
        gyro = g;
        BoardManager.BoardSelect(BoardManager.currentBoardSelection);
        if (!playerMovementLocked)
        {
            StopAllCoroutines();
            if (gamepadEnabled)
                StartCoroutine(GamepadMovementCoroutine());
            else
                StartCoroutine(GyroMovementCoroutine());
        }
    }

    public void UpdatePlayerBoard(PlayerMovementVariables newVariables)
    {
        movementVariables = newVariables;
        playerRigidbody.mass = movementVariables.mass;
        playerRigidbody.drag = movementVariables.drag;
        playerRigidbody.angularDrag = movementVariables.angularDrag;
        movementVariables.maxAscendAngle = 360.0f - movementVariables.maxAscendAngle;
        movementVariables.restingThreshold *= 0.5f;
        if (!gamepadEnabled)
            movementVariables.yawSensitivity *= Mathf.Rad2Deg * -0.05f;
    }
    public void ApplyForce()
    {
#if DEBUGGER
        float debugChangeAmt = Input.GetAxis("DebugAccelerateDecelerate");
        if (0.0f != debugChangeAmt)
            if (debugSpeedIncrease + debugChangeAmt > -50.0f && debugSpeedIncrease + debugChangeAmt < 101.0f)
                debugSpeedIncrease += debugChangeAmt;
#endif
        if (RoundTimer.timeInLevel < 1.0f)
            currAcceleration = movementVariables.downwardAcceleration;
        else
            currAcceleration = Mathf.Lerp(currAcceleration, newAcceleration, movementVariables.momentum);
        if (!playerMovementLocked)
        {
            float playerSpeed = playerRigidbody.velocity.magnitude;
            if (pitch > 360.0f - movementVariables.restingThreshold || pitch < movementVariables.restingThreshold)
            {
                newAcceleration = movementVariables.restingAcceleration + debugSpeedIncrease;
                if (playerSpeed < movementVariables.restingSpeed + debugSpeedIncrease)
                    playerRigidbody.AddRelativeForce(0.0f, 0.0f, currAcceleration + debugSpeedIncrease, ForceMode.Acceleration);
                else
                    playerRigidbody.AddRelativeForce(0.0f, 0.0f, playerSpeed, ForceMode.Acceleration);
            }
            else if (pitch < 180.0f)
            {
                newAcceleration = movementVariables.downwardAcceleration;
                if (playerSpeed < movementVariables.maxSpeed + debugSpeedIncrease)
                    playerRigidbody.AddRelativeForce(0.0f, 0.0f, currAcceleration + debugSpeedIncrease, ForceMode.Acceleration);
                else
                    playerRigidbody.AddRelativeForce(0.0f, 0.0f, playerSpeed, ForceMode.Acceleration);
            }
            else
            {
                newAcceleration = movementVariables.upwardAcceleration;
                if (playerSpeed < movementVariables.minSpeed + debugSpeedIncrease)
                    playerRigidbody.AddRelativeForce(0.0f, 0.0f, currAcceleration + debugSpeedIncrease, ForceMode.Acceleration);
                else
                    playerRigidbody.AddRelativeForce(0.0f, 0.0f, playerSpeed, ForceMode.Acceleration);
            }
        }
    }
    private IEnumerator GamepadMovementCoroutine()
    {
        yield return new WaitForFixedUpdate();

#if DEBUGGER
        if (BuildDebugger.WASD)
        {
            pitch = playerRigidbody.rotation.eulerAngles.x + Input.GetAxis("LVerticalWASD") * movementVariables.pitchSensitivity;
            yaw = playerRigidbody.rotation.eulerAngles.y + Input.GetAxis("LHorizontalWASD") * movementVariables.yawSensitivity;
        }
        else
#endif
        {
            pitch = playerRigidbody.rotation.eulerAngles.x + Input.GetAxis("LVertical") * movementVariables.pitchSensitivity;
            yaw = playerRigidbody.rotation.eulerAngles.y + Input.GetAxis("LHorizontal") * movementVariables.yawSensitivity;
        }
        if (pitch < 180.0f)
        {
            if (pitch > movementVariables.maxDescendAngle)
                pitch = movementVariables.maxDescendAngle;
        }
        else if (pitch < movementVariables.maxAscendAngle)
            pitch = movementVariables.maxAscendAngle;
        ApplyForce();
        playerRigidbody.MoveRotation(Quaternion.Euler(pitch, yaw, 0.0f));
        StartCoroutine(GamepadMovementCoroutine());
    }
    private IEnumerator GyroMovementCoroutine()
    {
        yield return new WaitForFixedUpdate();
        gyroPrevPitch = pitch = Mathf.Lerp(gyroPrevPitch, (float)gyro.rollAngle * Mathf.Rad2Deg * movementVariables.pitchSensitivity, gryoPitchInterpolation);
        yaw = playerRigidbody.rotation.eulerAngles.y + (float)gyro.pitchAngle * movementVariables.yawSensitivity;
        ApplyForce();
        playerRigidbody.MoveRotation(Quaternion.Euler(pitch, yaw, 0.0f));
        StartCoroutine(GyroMovementCoroutine());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (SceneManager.GetActiveScene().buildIndex >= LevelManager.LevelBuildOffset)
            playerRigidbody.AddForce(collision.impulse * movementVariables.bounceModifier, ForceMode.Impulse);
    }
    void OnEnable() { EventManager.OnToggleMovement += SetPlayerMovementLock; }
    void OnDisable() { EventManager.OnToggleMovement -= SetPlayerMovementLock; }
}