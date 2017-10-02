using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : SelectedObject
{
    [SerializeField] int sceneIndex = 0;

    override public void selectSuccessFunction()
    {
        //changes the current state of the game
        EventManager.OnTriggerTransition(sceneIndex);
    }
}