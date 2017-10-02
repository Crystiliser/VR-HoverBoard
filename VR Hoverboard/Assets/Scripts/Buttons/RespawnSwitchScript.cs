using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RespawnSwitchScript : SelectedObject
{
    TextMeshPro respawnOnOffText;

    GameManager theManager;
    bool IsOn { get { return theManager.scoreScript.respawnEnabled; } set { theManager.scoreScript.respawnEnabled = value; } }

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
            respawnOnOffText.SetText("On");
        }
        else
        {
            respawnOnOffText.SetText("Off");
        }
    }

    private void OnEnable()
    {
        respawnOnOffText = gameObject.GetComponentInChildren<TextMeshPro>();
    }

    override public void selectSuccessFunction()
    {
        IsOn = !IsOn;
        if (IsOn)
        {
            respawnOnOffText.SetText("On");
        }
        else
        {
            respawnOnOffText.SetText("Off");
        }
    }
}
