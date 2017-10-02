using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFanController : MonoBehaviour
{
    MotorData motor;
    Rigidbody playerRB;
    Transform playerTransform;
    ManagerClasses.PlayerMovementVariables pmv;

    int motorCount = 0;
	float motorPercentage = 0f;
    float sampledVelocity = 0f;

	float invertedMaxSpeed = 0f;

    //called by our BoardManager
    public void SetupFanControllerScript()
    {
        playerRB = GameManager.player.GetComponent<Rigidbody>();
        playerTransform = GameManager.player.GetComponent<Transform>();
        pmv = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables;

        UpdateFanPercentage();

        StartCoroutine(DetectFanCoroutine());
    }

    //called by our BoardManager
    public void UpdateFanPercentage()
    {
		pmv = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables;
        invertedMaxSpeed = 1f / pmv.maxSpeed;
    }

    IEnumerator DetectFanCoroutine()
    {
        motor = new MotorData();

        //wait for the motor to attatch
        yield return new WaitForSeconds(0.1f);

        if (null != motor.device)
            motorCount = motor.device.motors.Count;

        if (motorCount > 0)
            StartCoroutine(FanCoroutine());
        else
            motor.Close();
    }

    IEnumerator FanCoroutine()
    {
        yield return new WaitForFixedUpdate();

        //get our velocity based on if we are going forward/backwards
        sampledVelocity = playerTransform.InverseTransformDirection(playerRB.velocity).z;
        
		//print ("Sampled Velocity: " + sampledVelocity);
		//print ("Min Val: " + pmv.minSpeed + " Max Val: " + pmv.maxSpeed);

		motorPercentage = sampledVelocity * 100f * invertedMaxSpeed;
		motorPercentage = Mathf.Clamp (motorPercentage, 15f, 100f);

		//print ("Motor Percentage: " + motorPercentage);
		//print ("Applying motor percentage.");
		for (int i = 0; i < motorCount; i++)
			motor.device.motors[i].Velocity = motorPercentage;
		
        StartCoroutine(FanCoroutine());
    }

    private void OnApplicationQuit()
    {
        if (motor != null)
        {
            StopAllCoroutines();
            motor.Close();
        }
    }

}
