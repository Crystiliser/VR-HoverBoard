using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudOverallOnOffButton : SelectedObject
{
    TextElementControllerScript textElementController;
    bool safeCheck = false;
    

    TextMeshPro onOffText;
    bool IsOn { get { return textElementController.hudElementsControl.overAllBool; } set { textElementController.setAll(value); } }
    new private void Start ()
    {
        base.Start();
        textElementController = GameManager.player.GetComponentInChildren<TextElementControllerScript>();
        if (textElementController != null)
        {
            safeCheck = true;
        }
        onOffText = gameObject.GetComponentsInChildren<TextMeshPro>()[0];
        if (IsOn)
        {
            onOffText.SetText("On");
        }
        else
        {
            onOffText.SetText("Off");
        }
        EventManager.OnCallUpdateButtons();
    }

    override public void selectSuccessFunction()
    {
        if (safeCheck)
        {
            IsOn = !IsOn;
            if (IsOn)
            {
                onOffText.SetText("On");
            }
            else
            {
                onOffText.SetText("Off");
            }
            EventManager.OnCallUpdateButtons();
        }
        else
        {
            Debug.Log("The button couldn't find the players text element controller to toggle");
        }
    }
}
