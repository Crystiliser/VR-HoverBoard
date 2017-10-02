using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTabs : SelectedObject
{
    [SerializeField]
    private MenuTab menuTab = null;

    public override void selectSuccessFunction()
    {
        if (null != menuTab)
            if (null != GetComponentInParent<MainMenu>().OnSwitchTabs)
                GetComponentInParent<MainMenu>().OnSwitchTabs(menuTab);
    }
}