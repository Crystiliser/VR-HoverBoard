using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudArrowOnOffScript : SelectedObject
{
    TextElementControllerScript textElementController;
    bool safeCheck = false;

    TextMeshPro onOffText;
    bool IsOn { get { return textElementController.hudElementsControl.arrowBool; } set { textElementController.setArrow(value); } }

    public void isOnUpdate()
    {
        if (IsOn)
        {
            onOffText.SetText("On");
        }
        else
        {
            onOffText.SetText("Off");
        }
    }

    private void OnEnable()
    {
        textElementController = GameManager.player.GetComponentInChildren<TextElementControllerScript>();
        if (textElementController != null)
        {
            safeCheck = true;
        }
        onOffText = gameObject.GetComponentsInChildren<TextMeshPro>()[0];
        EventManager.OnUpdateButtons += isOnUpdate;
        isOnUpdate();
    }

    private void OnDisable()
    {
        EventManager.OnUpdateButtons -= isOnUpdate;
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
        }
        else
        {
            Debug.Log("The button couldn't find the players text element to toggle");
        }
    }
}
