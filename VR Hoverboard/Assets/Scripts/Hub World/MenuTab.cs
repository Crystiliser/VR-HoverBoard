﻿using UnityEngine;
public class MenuTab : MonoBehaviour
{
    private SelectedObject[] buttons;
    protected void Awake()
    {
        buttons = GetComponentsInChildren<SelectedObject>();
        if (this != GetComponentInParent<MainMenu>().mainTab)
            DisableButtons();
    }
    public void EnableButtons()
    {
        foreach (SelectedObject button in buttons)
            button.IsDisabled = false;
    }
    public void DisableButtons()
    {
        foreach (SelectedObject button in buttons)
            button.IsDisabled = true;
    }
}