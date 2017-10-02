using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerGameplayController : MonoBehaviour
{
    bool debugSpeedEnabled;
    bool gamepadEnabled = false;
    bool playerMovementLocked = true;

    SpatialData gyro;
    Rigidbody playerRigidbody;
    ManagerClasses.RoundTimer roundTimer;

    float pitch, yaw;
    float gyroPrevPitch;
    float newAcceleration, currAcceleration;
    [SerializeField] [Range(0.01f, 1.0f)] float gryoPitchInterpolation = 0.1f;
    [SerializeField] [Range(1f, 3f)] float gyroPitchPercentIncrease = 1f;
    float debugSpeedIncrease;

    BoardManager boardManager;
    public ManagerClasses.PlayerMovementVariables movementVariables;

    public float DebugSpeedIncrease { get { return debugSpeedIncrease; } }

    public void StartDebugSpeedControls()
    {
        debugSpeedEnabled = true;
        debugSpeedIncrease = 0f;
    }

    public void StopDebugSpeedControls()
    {
        debugSpeedEnabled = false;
        debugSpeedIncrease = 0f;
    }

    //called by our BoardManager
    public void SetupGameplayControllerScript()
    {
        boardManager = GameManager.instance.boardScript;
        roundTimer = GameManager.instance.roundTimer;

        gamepadEnabled = boardManager.gamepadEnabled;
        playerRigidbody = GetComponent<Rigidbody>();
        gyro = boardManager.gyro;
        gyroPrevPitch = 0f;
        newAcceleration = currAcceleration = 0f;
    }

    //function to subscribe to the OnToggleMovement event
    void SetPlayerMovementLock(bool locked)
    {
        if (locked != playerMovementLocked)
        {
            //if we aren't locked
            if (!locked)
            {
                //print("Player Gameplay Controller UNLOCKED!");

                //be sure to not have multiple instances of our coroutines going
                StopAllCoroutines();

                if (gamepadEnabled)
                    StartCoroutine(GamepadMovementCoroutine());
                else
                    StartCoroutine(GyroMovementCoroutine());
            }
            else
            {
                //print("Player Gameplay Controller LOCKED!");
                StopAllCoroutines();

                //if we're locking movement, then set the velocity to zero
                playerRigidbody.velocity = Vector3.zero;

                if (debugSpeedEnabled)
                    debugSpeedIncrease = 0f;
            }

            playerMovementLocked = locked;
        }
    }

    //update our script depending on if we are using a xbox gamepad or the gyro
    //  Note: this should normally not be directly called, instead call the BoardManager's UpdateControlsType()
    public void UpdateGameplayControlsType(bool gEnabled, SpatialData g)
    {
        gamepadEnabled = gEnabled;
        gyro = g;

        boardManager.BoardSelect(boardManager.currentBoardSelection);

        //if our movement isn't locked, update what coroutine we are using
        if (!playerMovementLocked)
        {
            StopAllCoroutines();

            if (gamepadEnabled)
                StartCoroutine(GamepadMovementCoroutine());
            else
                StartCoroutine(GyroMovementCoroutine());
        }
    }

    //updates our movement variabels based on a BoardType
    //  Note: this should normally not be directly called, instead call the BoardManager's BoardSelect()
    public void UpdatePlayerBoard(ManagerClasses.PlayerMovementVariables newVariables)
    {
        movementVariables = newVariables;

        playerRigidbody.mass = movementVariables.mass;
        playerRigidbody.drag = movementVariables.drag;
        playerRigidbody.angularDrag = movementVariables.angularDrag;

        //adjust our max ascend value for easier use when clamping the pitch for gamepad controls
        movementVariables.maxAscendAngle = 360 - movementVariables.maxAscendAngle;

        //make sure only half of the resting threshold is being checked for the upper and lower angles
        movementVariables.restingThreshold *= 0.5f;

        if (!gamepadEnabled)
        {
            //since the information we are getting from the gyro is in radians, include Mathf.Rad2Deg in our sensitivities
            movementVariables.pitchSensitivity = movementVariables.pitchSensitivity * Mathf.Rad2Deg * 0.05f;
            movementVariables.yawSensitivity = movementVariables.yawSensitivity * Mathf.Rad2Deg * 0.05f * -1f;
        }
    }

    //helper function
    void ApplyForce()
    {
        //update our debugSpeedIncease, if it is enabled
        if (debugSpeedEnabled)
        {
            float changeAmt = Input.GetAxis("DebugAccelerateDecelerate");
            if (changeAmt != 0f)
            {
                if (debugSpeedIncrease + changeAmt > -50f && debugSpeedIncrease + changeAmt < 101f)
                    debugSpeedIncrease += changeAmt;
            }
        }

        //update our currAcceleration based off of our inverted momentum variable
        //give the player a boost for the first couple of seconds in a level
        if (roundTimer.TimeInLevel < 1f)
            currAcceleration = movementVariables.restingAcceleration;
        else
            currAcceleration = Mathf.Lerp(currAcceleration, newAcceleration, movementVariables.momentum);

        float playerSpeed = playerRigidbody.velocity.magnitude;

        //update our newAcceleration based off of what the player's pitch is
        if (!playerMovementLocked)
        {
            //if restingThreshold were set to 10
            //         pitch > 355 or pitch < 5
            if (pitch > 360f - movementVariables.restingThreshold || pitch < movementVariables.restingThreshold)
            {
                newAcceleration = movementVariables.restingAcceleration + debugSpeedIncrease;
                //print("In resting threshold! " + movementVariables.restingAcceleration);

                if (playerSpeed < movementVariables.restingSpeed + debugSpeedIncrease)
                    playerRigidbody.AddRelativeForce(Vector3.forward * (currAcceleration + debugSpeedIncrease), ForceMode.Acceleration);
                else
                    playerRigidbody.AddRelativeForce(Vector3.forward * (playerSpeed), ForceMode.Acceleration);

            }
            else if (pitch < 180f)
            {
                newAcceleration = movementVariables.downwardAcceleration;
                //print("Accelerating to: " + movementVariables.downwardAcceleration);

                if (playerSpeed < movementVariables.maxSpeed + debugSpeedIncrease)
                    playerRigidbody.AddRelativeForce(Vector3.forward * (currAcceleration + debugSpeedIncrease), ForceMode.Acceleration);
                else
                    playerRigidbody.AddRelativeForce(Vector3.forward * (playerSpeed), ForceMode.Acceleration);
            }

            else
            {
                newAcceleration = movementVariables.upwardAcceleration;
                //print("Decelerating to: " + movementVariables.upwardAcceleration);

                if (playerSpeed < movementVariables.minSpeed + debugSpeedIncrease)
                    playerRigidbody.AddRelativeForce(Vector3.forward * (currAcceleration + debugSpeedIncrease), ForceMode.Acceleration);
                else
                    playerRigidbody.AddRelativeForce(Vector3.forward * (playerSpeed), ForceMode.Acceleration);
            }
        }
    }

    IEnumerator GamepadMovementCoroutine()
    {
        yield return new WaitForFixedUpdate();

        pitch = playerRigidbody.rotation.eulerAngles.x + Input.GetAxis("LVertical") * movementVariables.pitchSensitivity;
        yaw = playerRigidbody.rotation.eulerAngles.y + Input.GetAxis("LHorizontal") * movementVariables.yawSensitivity;

        //pitch rests at 0 degrees
        //when decending pitch travels in a positive direction (from 0 to 360)
        //when ascending pitch travels in a negative direction (from 360 to 0)

        //descending
        if (pitch < 180f)
        {
            if (pitch > movementVariables.maxDescendAngle)
                pitch = movementVariables.maxDescendAngle;
        }
        //ascending
        else
        {
            if (pitch < movementVariables.maxAscendAngle)
                pitch = movementVariables.maxAscendAngle;
        }

        ApplyForce();

        //since we don't want to make our player sick, make sure we never roll the camera
        playerRigidbody.MoveRotation(Quaternion.Euler(pitch, yaw, 0f));

        StartCoroutine(GamepadMovementCoroutine());
    }

    IEnumerator GyroMovementCoroutine()
    {
        yield return new WaitForFixedUpdate();

        pitch = (float)gyro.rollAngle * Mathf.Rad2Deg * gyroPitchPercentIncrease;
        pitch = Mathf.Lerp(gyroPrevPitch, pitch, gryoPitchInterpolation);
        gyroPrevPitch = pitch;
        
        yaw = playerRigidbody.rotation.eulerAngles.y + (float)gyro.pitchAngle * movementVariables.yawSensitivity;

        ApplyForce();

        //since we don't want to make our player sick, make sure we never roll the camera      
        playerRigidbody.MoveRotation(Quaternion.Euler(pitch, yaw, 0f));

        StartCoroutine(GyroMovementCoroutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //scale our impulse by our bounce amount if we aren't in the hub world
        if (SceneManager.GetActiveScene().buildIndex != 2)
            playerRigidbody.AddForce(collision.impulse * movementVariables.bounceModifier, ForceMode.Impulse);
    }

    void OnEnable()
    {
        EventManager.OnToggleMovement += SetPlayerMovementLock;
    }

    void OnDisable()
    {
        EventManager.OnToggleMovement -= SetPlayerMovementLock;
    }

}