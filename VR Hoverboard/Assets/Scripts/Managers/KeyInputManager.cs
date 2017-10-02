using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class KeyInputManager : MonoBehaviour
{
    ManagerClasses.GameState state;

    //variables for returning back to menu
    public float flippedTimer = 3f;
    public bool hubOnFlippedHMD = false;
    bool countingDown = false;
    float timeUpsideDown = 0f;
    float originalCameraContainerHeight;
    Quaternion flippedQuaternion;

    public void setupKeyInputManager(ManagerClasses.GameState s)
    {
        state = s;
        originalCameraContainerHeight = GameManager.player.GetComponentInChildren<CameraCounterRotate>().transform.localPosition.y;
        StartCoroutine(CalibrationCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state.currentState != GameStates.MainMenu)
            {
                EventManager.OnTriggerTransition(1);
            }
            else
            { 
                SaveLoader.save();
                Application.Quit();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("XBox Back"))
        {
            StartCoroutine(CalibrationCoroutine());
        }

        if (state.currentState != GameStates.MainMenu && Input.GetButtonDown("XBox Start"))
        {
            EventManager.OnTriggerTransition(1);
        }

        if (state.currentState != GameStates.MainMenu && hubOnFlippedHMD && VRDevice.isPresent)
        {
            flippedQuaternion = InputTracking.GetLocalRotation(VRNode.Head);

            //if we're upside down, start the countdown and reset our timer
            if (flippedQuaternion.eulerAngles.z > 150f && flippedQuaternion.eulerAngles.z < 210f && !countingDown)
            {
                countingDown = true;
                timeUpsideDown = 0f;
            }
            else if (countingDown)
            {
                //if we're still upside down
                if (flippedQuaternion.eulerAngles.z > 150f && flippedQuaternion.eulerAngles.z < 210f)
                    timeUpsideDown += Time.deltaTime;
                else
                    countingDown = false;

                //go back to main menu once we've been upside down long enough
                if (timeUpsideDown > flippedTimer)
                {
                    countingDown = false;
                    EventManager.OnTriggerTransition(1);
                }
            }
        }
    }

    public IEnumerator CalibrationCoroutine()
    {
        if (VRDevice.isPresent)
        {
            //wait for the end of the frame so we can catch positional data from the VR headset
            yield return new WaitForEndOfFrame();

            GameObject player = GameManager.player;

            Transform cameraContainer = player.GetComponentInChildren<CameraCounterRotate>().transform;

            Vector3 playerPosition = player.GetComponent<Transform>().position;
            Vector3 originalPosition = new Vector3(playerPosition.x, playerPosition.y + originalCameraContainerHeight, playerPosition.z);
            Quaternion playerRotation = player.GetComponent<Transform>().rotation;

            //set the cameraContainer back on top of the board, in case we are re-calibrating
            cameraContainer.SetPositionAndRotation(originalPosition, playerRotation);

            Vector3 headPosition = player.GetComponentInChildren<ScreenFade>().transform.localPosition;
            Vector3 headRotation = player.GetComponentInChildren<ScreenFade>().transform.eulerAngles;

            //rotate, then translate

            //rotate the camera so that it is rotated in the same direction as the board
            float yRotation = Mathf.DeltaAngle(headRotation.y, cameraContainer.eulerAngles.y);
            cameraContainer.Rotate(Vector3.up * yRotation);

            //headPosition acts as though the cameraContainer is the ground
            //so if headPosition.y = 1.4, then the camera will be sitting 1.4 meters above the cameraContainer
            //therefore, translate the cameraContainer in opposite directions of wherever the headPosition is
            cameraContainer.Translate(headPosition * -1f);
        }
    }

}
