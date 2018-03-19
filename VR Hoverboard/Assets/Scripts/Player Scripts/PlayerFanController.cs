using System.Collections;
using UnityEngine;
public class PlayerFanController : MonoBehaviour
{
    private MotorData motor = null;
    private Rigidbody playerRB = null;
    private PlayerMovementVariables pmv = null;
    private int motorCount = 0;
    private float motorPercentage = 0.0f, invertedMaxSpeed100x = 0.0f;
    public void SetupFanControllerScript()
    {
        playerRB = GameManager.player.GetComponent<Rigidbody>();
        pmv = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables;
        UpdateFanPercentage();
        StartCoroutine(DetectFanCoroutine());
    }
    public void UpdateFanPercentage()
    {
        pmv = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables;
        invertedMaxSpeed100x = 100.0f / pmv.maxSpeed;
    }
    private IEnumerator DetectFanCoroutine()
    {
        motor = new MotorData();
        yield return new WaitForSeconds(MotorData.WaitForAttach);
        motorCount = motor.MotorDevice?.motors.Count ?? motorCount;
        if (motorCount > 0)
            StartCoroutine(FanCoroutine());
        else
            motor.Close();
    }
    private IEnumerator FanCoroutine()
    {
        yield return new WaitForFixedUpdate();
        motorPercentage = Mathf.Clamp(GameManager.player.transform.InverseTransformDirection(playerRB.velocity).z * invertedMaxSpeed100x, 25.0f, 100.0f);
        for (int i = 0; i < motorCount; ++i)
            motor.MotorDevice.motors[i].Velocity = motorPercentage;
        StartCoroutine(FanCoroutine());
    }
    private void OnApplicationQuit()
    {
        if (null != motor)
        {
            StopAllCoroutines();
            motor.Close();
        }
    }
}