using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class does not not need to be instantiated
public class EventManager : MonoBehaviour
{
    //delegates can be overwritten (less secure)
    public delegate void ToggleMovementLock(bool locked);
    //events can only be subscribed to or unsubscribed to (more secure)
    public static event ToggleMovementLock OnToggleMovement;

    static public void OnSetGameplayMovementLock(bool locked)
    {
        //if the event is subscribed to
        if (OnToggleMovement != null)
            OnToggleMovement(locked);
    }

    public delegate void Transition(int sceneIndex);
    public static event Transition OnTransition;

    static public void OnTriggerTransition(int sceneIndex)
    {
        if (OnTransition != null)
            OnTransition(sceneIndex);
    }

    public delegate void ToggleSelectionLock(bool locked);
    public static event ToggleSelectionLock OnSelectionLock;
    static public void OnTriggerSelectionLock(bool locked)
    {
        if (OnSelectionLock != null)
            OnSelectionLock(locked);
    }

    public delegate void ToggleHud(bool isOn);
    public static event ToggleHud OnToggleHud;

    static public void OnSetHudOnOff(bool isOn)
    {
        if (OnToggleHud != null)
            OnToggleHud(isOn);
    }

    //Needed to turn the arrow back on, and let it aim at consecutive rings
    public delegate void ToggleArrow(bool isOn);
    public static event ToggleArrow OnToggleArrow;

    static public void OnSetArrowOnOff(bool isOn)
    {
        if (OnToggleArrow != null)
            OnToggleArrow(isOn);
    }

    public delegate void updateOptionButtons();
    public static event updateOptionButtons OnUpdateButtons;

    static public void OnCallUpdateButtons()
    {
        if (OnUpdateButtons != null)
            OnUpdateButtons();
    }

    public delegate void setRingPath(bool isOn);
    public static event setRingPath OnSetRingPath;

    static public void OnCallSetRingPath(bool isOn)
    {
        if (OnSetRingPath != null)
            OnSetRingPath(isOn);
    }


    public delegate void updateBoardMenuEffects();
    public static event updateBoardMenuEffects OnUpdateBoardMenuEffects;

    static public void OnCallBoardMenuEffects()
    {
        if (OnUpdateBoardMenuEffects != null)
            OnUpdateBoardMenuEffects();
    }
}
