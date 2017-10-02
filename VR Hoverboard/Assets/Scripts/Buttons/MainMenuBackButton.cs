using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackButton : SelectedObject
{
    public override void selectSuccessFunction()
    {
        if (null != GetComponentInParent<MainMenu>().OnBackButtonPressed)
            GetComponentInParent<MainMenu>().OnBackButtonPressed();
    }
}