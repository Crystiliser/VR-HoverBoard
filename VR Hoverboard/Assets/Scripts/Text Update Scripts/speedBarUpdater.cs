using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class speedBarUpdater : MonoBehaviour
{
    //must set these values
    public float currentValue = 0;
    public float maxValue = 0;
    public float minValue = 0;
    
    Image fillUpBar;

    private ManagerClasses.PlayerMovementVariables moveVars;
    private Rigidbody player;

    //so we dont do division as often as possible
    float prevValue;

    void Start()
    {
        fillUpBar = gameObject.GetComponentsInChildren<Image>()[1];
        moveVars = GameManager.player.GetComponent<PlayerGameplayController>().movementVariables;
        maxValue = moveVars.maxSpeed;
        minValue = moveVars.minSpeed;
        player = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        currentValue = player.velocity.magnitude;
        if (prevValue != currentValue)
        {
            fillUpBar.fillAmount = (currentValue - minValue) / (maxValue - minValue);
        }

        prevValue = currentValue;
    }
}
