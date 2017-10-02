using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardRollEffect : MonoBehaviour
{
    Transform boardTransform;
    Rigidbody playerRB;

    int currScene;
    float zRotation;
    float prevYRotation;
    bool isMoving;
    float xRotation;
    float forwardSpeed;

    [System.Serializable]
    public class PitchRollEffectVariables
    {
        [Header("Roll")]
        public float rollIncreaseRate = 1.2f;
        public float rollDecreaseRate = 0.1f;
        public float maxRollDegree = 25f;

        [Header("Pitch")]
        public float pitchIncreaseRate = 1f;
        public float pitchDecreaseRate = 0.5f;
        public float maxPitchDegree = 20f;
    }

    [SerializeField]
    PitchRollEffectVariables variables;

    void LevelSelectionUnlocked(bool locked)
    {
        if (locked == false)
        {
            StopAllCoroutines();

            if (boardTransform == null)
                boardTransform = GetComponent<Transform>();

            if (playerRB == null)
                playerRB = GetComponentInParent<Rigidbody>();

            //reset our zRotation and prevYRotation once level manager has updated the player position
            zRotation = 0f;
            prevYRotation = boardTransform.eulerAngles.y;

            //reset our xRotation and prevXRotation as well
            xRotation = 0f;

            StartCoroutine(BoardRollCoroutine());
        }
    }

    void RollEffect()
    {
        //no reason to do any calculations if there was no difference between rotations
        if (prevYRotation != boardTransform.eulerAngles.y)
        {
            zRotation += Mathf.DeltaAngle(boardTransform.eulerAngles.y, prevYRotation) * variables.rollIncreaseRate;
            //print("Z ROT: " + zRotation);

            //clamp our rotation to our maxRollDegree
            zRotation = Mathf.Clamp(zRotation, -variables.maxRollDegree, variables.maxRollDegree);

            //don't forget to update our prevYRotation
            prevYRotation = boardTransform.eulerAngles.y;
        }

        //only update our zRotation if there is a rotation to update
        if (zRotation != 0f)
        {
            //lerp to 0 based off of our rollDecreaseRate
            zRotation = Mathf.Lerp(zRotation, 0f, variables.rollDecreaseRate);

            //set our rotation to 0 if we're within tolerance
            if (zRotation < 0.1f && zRotation > -0.1f)
                zRotation = 0f;

            boardTransform.rotation = Quaternion.Euler(boardTransform.eulerAngles.x, boardTransform.eulerAngles.y, zRotation);
        }
    }

    void PitchEffect()
    {      
        //only do our pitch effect if we are in start or main menu scenes
        if (currScene == 0 || currScene == 1)
        {
            forwardSpeed = boardTransform.InverseTransformDirection(playerRB.velocity).z;
            //print("FORWARD SPEED: " + forwardSpeed);

            if (!(forwardSpeed < 0.1f && forwardSpeed > -0.1f))
            {
                xRotation += forwardSpeed * variables.pitchIncreaseRate;
                xRotation = Mathf.Clamp(xRotation, -variables.maxPitchDegree, variables.maxPitchDegree);
            }

            if (xRotation != 0f)
            {
                xRotation = Mathf.Lerp(xRotation, 0f, variables.pitchDecreaseRate);

                if (xRotation < 0.1f && xRotation > -0.1f)
                    xRotation = 0f;

                boardTransform.rotation = Quaternion.Euler(xRotation, boardTransform.eulerAngles.y, boardTransform.eulerAngles.z);
            }
        }
    }

    IEnumerator BoardRollCoroutine()
    {
        yield return new WaitForFixedUpdate();

        RollEffect();
        PitchEffect();

        StartCoroutine(BoardRollCoroutine());
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        currScene = SceneManager.GetActiveScene().buildIndex;
    }

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
