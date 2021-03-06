﻿using UnityEngine;
using Xander.Debugging;
using Xander.NullConversion;
public class MainMenu : MonoBehaviour
{
    private const float BACK_LOCAL_Z_POS = 0.5f;
    private const float FRONT_LOCAL_Z_POS = -0.5f;
    private static readonly Vector3 TABS_BACK_LOCAL_POS = new Vector3(0.0f, 0.0f, BACK_LOCAL_Z_POS);
    private static readonly Vector3 TABS_FRONT_LOCAL_POS = new Vector3(0.0f, 0.0f, FRONT_LOCAL_Z_POS);
    [SerializeField] private SelectedObject backButton = null;
    public MenuTab mainTab = null;
    [SerializeField] private GameObject menuBox = null;
    public delegate void GoBackEvent();
    public event GoBackEvent OnBackButtonPressed, OnMenuExit;
    public delegate void SwitchTabsEvent(MenuTab menuTab);
    public event SwitchTabsEvent OnSwitchTabs;
    private Vector3 backButtonBackPos, backButtonFrontPos;
    private MenuTab currTab = null, prevTab = null;
    private float transitionTimer = 0.0f;
    public void InvokeOnBackButtonPressed() { OnBackButtonPressed?.Invoke(); }
    public void InvokeOnMenuExit() { OnMenuExit?.Invoke(); }
    public void InvokeOnSwitchTabs(MenuTab menuTab) { OnSwitchTabs?.Invoke(menuTab); }
    private void Start()
    {
        try { menuBox.SetActive(false); }
        catch { Debug.LogWarning("MenuBox missing" + this.Info(), this); }
        currTab = prevTab = mainTab;
        OnBackButtonPressed += GoBack;
        OnMenuExit += OnExit;
        OnSwitchTabs += SwitchTab;
        backButtonBackPos = backButtonFrontPos = backButton.ConvertNull()?.transform.localPosition ?? Vector3.zero;
        backButtonBackPos.z = BACK_LOCAL_Z_POS;
        backButtonFrontPos.z = FRONT_LOCAL_Z_POS;
        mainTab.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);
    }
    private enum TransitionState { SwitchingToTab, SwitchingToMain, OnMain, OnTab }
    private TransitionState currState = TransitionState.OnMain;
    private void GoBack()
    {
        if (TransitionState.OnTab == currState)
        {
            MenuTab t = currTab;
            currTab = prevTab;
            prevTab = t;
            transitionTimer = 0.0f;
            currState = mainTab == currTab ? TransitionState.SwitchingToMain : TransitionState.SwitchingToTab;
        }
    }
    private void OnExit()
    {
        GameManager.player.GetComponent<PlayerMenuController>().UnlockPlayerPosition();
        menuBox.ConvertNull()?.SetActive(false);
        if (TransitionState.SwitchingToTab == currState || TransitionState.OnTab == currState)
        {
            prevTab = currTab;
            currTab = mainTab;
            transitionTimer = 1.0f;
            currState = TransitionState.SwitchingToMain;
        }
    }
    private void SwitchTab(MenuTab menuTab)
    {
        if (TransitionState.OnMain != currState && TransitionState.OnTab != currState)
            return;
        prevTab = currTab;
        currTab = menuTab;
        transitionTimer = 0.0f;
        currState = TransitionState.SwitchingToTab;
    }
    private void OnDestroy()
    {
        OnBackButtonPressed -= GoBack;
        OnMenuExit -= OnExit;
        OnSwitchTabs -= SwitchTab;
    }
    private void Update()
    {
        transitionTimer += Time.deltaTime;
        switch (currState)
        {
            case TransitionState.SwitchingToTab:
                if (transitionTimer >= 1.0f)
                {
                    prevTab.gameObject.SetActive(false);
                    prevTab.transform.localPosition = TABS_BACK_LOCAL_POS;
                    currTab.transform.localPosition = TABS_FRONT_LOCAL_POS;
                    backButton.transform.localPosition = backButtonFrontPos;
                    currTab.EnableButtons();
                    backButton.IsDisabled = false;
                    currState = TransitionState.OnTab;
                }
                else
                {
                    prevTab.DisableButtons();
                    prevTab.transform.localPosition = Vector3.Lerp(TABS_FRONT_LOCAL_POS, TABS_BACK_LOCAL_POS, transitionTimer);
                    currTab.transform.localPosition = Vector3.Lerp(TABS_BACK_LOCAL_POS, TABS_FRONT_LOCAL_POS, transitionTimer);
                    if (mainTab == prevTab)
                        backButton.transform.localPosition = Vector3.Lerp(backButtonBackPos, backButtonFrontPos, transitionTimer);
                    currTab.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                }
                break;
            case TransitionState.SwitchingToMain:
                if (transitionTimer >= 1.0f)
                {
                    prevTab.gameObject.SetActive(false);
                    backButton.gameObject.SetActive(false);
                    prevTab.transform.localPosition = TABS_BACK_LOCAL_POS;
                    mainTab.transform.localPosition = TABS_FRONT_LOCAL_POS;
                    backButton.transform.localPosition = backButtonBackPos;
                    mainTab.EnableButtons();
                    currTab = prevTab = mainTab;
                    currState = TransitionState.OnMain;
                    mainTab.gameObject.SetActive(true);
                }
                else
                {
                    prevTab.DisableButtons();
                    backButton.IsDisabled = true;
                    prevTab.transform.localPosition = Vector3.Lerp(TABS_FRONT_LOCAL_POS, TABS_BACK_LOCAL_POS, transitionTimer);
                    mainTab.transform.localPosition = Vector3.Lerp(TABS_BACK_LOCAL_POS, TABS_FRONT_LOCAL_POS, transitionTimer);
                    backButton.transform.localPosition = Vector3.Lerp(backButtonFrontPos, backButtonBackPos, transitionTimer);
                    mainTab.gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
}