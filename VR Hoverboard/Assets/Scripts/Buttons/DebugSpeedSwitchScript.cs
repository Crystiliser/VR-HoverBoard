using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugSpeedSwitchScript : SelectedObject
{
    TextMeshPro debugSpeedOnOffText;

    GameManager theManager;
    bool IsOn { get { return theManager.boardScript.debugSpeedEnabled; } set { theManager.boardScript.UpdateDebugSpeedControls(value); } }

    new private void Start()
    {
        base.Start();
        theManager = GameManager.instance;
        controllerIsOnUpdate();
    }

    public void controllerIsOnUpdate()
    {
        if (IsOn)
        {
            debugSpeedOnOffText.SetText("On");
        }
        else
        {
            debugSpeedOnOffText.SetText("Off");
        }
    }

    private void OnEnable()
    {
        debugSpeedOnOffText = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    override public void selectSuccessFunction()
    {
        IsOn = !IsOn;
        controllerIsOnUpdate();
    }
}
