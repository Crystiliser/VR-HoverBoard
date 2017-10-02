using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControllerSwitchScript : SelectedObject
{
    TextMeshPro controllerOnOffText;

    GameManager theManager;
    bool IsOn { get { return theManager.boardScript.gamepadEnabled; } set { theManager.boardScript.UpdateControlsType(value); } }

    new private void Start()
    {
        base.Start();
        theManager = GameManager.instance;
        StartCoroutine(DelayedTextUpdate());
    }

    IEnumerator DelayedTextUpdate()
    {
        //wait for the gyro to be detected, then update
        yield return new WaitForSeconds(0.11f);

        if (IsOn)
            controllerOnOffText.SetText("On");
        else
            controllerOnOffText.SetText("Off");
    }

    private void OnEnable()
    {
        controllerOnOffText = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    override public void selectSuccessFunction()
    {
        IsOn = !IsOn;
        if (IsOn)
        {
            controllerOnOffText.SetText("On");
        }
        else
        {
            controllerOnOffText.SetText("Off");
        }
    }
}
