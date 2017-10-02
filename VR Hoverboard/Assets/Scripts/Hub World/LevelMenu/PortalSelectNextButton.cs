using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSelectNextButton : SelectedObject
{
    LevelMenu levelMenu;

    new private void Start()
    {
        base.Start();
        levelMenu = GetComponentInParent<LevelMenu>();
    }

    public override void selectSuccessFunction()
    {
        levelMenu.NextLevel();
    }
}
