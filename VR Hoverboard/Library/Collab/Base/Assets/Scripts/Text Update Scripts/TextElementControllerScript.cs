using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextElementControllerScript : MonoBehaviour {
    GameObject fpsText;
    GameObject timerText;
    GameObject scoreText;
    GameObject arrow;
    GameObject ringCountText;
    GameObject speedText;
    GameObject speedBar;
    GameObject altimeterText;
    GameObject debugGUI;

    public struct hudElementsBools
    {
        public bool timerBool;
        public bool scoreBool;
        public bool fpsBool;
        public bool arrowBool;
        public bool ringCountBool;
        public bool speedBool;
        public bool speedBarBool;
        public bool altimeterBool;
        public bool debugGUIBool;
        public bool overAllBool;
    }
    public hudElementsBools hudElementsControl;

    public void setTimer(bool isOn) { hudElementsControl.timerBool = isOn; }
    public void setScore(bool isOn) { hudElementsControl.scoreBool = isOn; }
    public void setFPS(bool isOn) { hudElementsControl.fpsBool = isOn; }
    public void setArrow(bool isOn){ hudElementsControl.arrowBool = isOn; }
    public void setRingCount(bool isOn) { hudElementsControl.ringCountBool = isOn; }
    public void setSpeed(bool isOn) { hudElementsControl.speedBool = isOn; }
    public void setSpeedBar(bool isOn) { hudElementsControl.speedBarBool = isOn; }
    public void setAltimeter(bool isOn) { hudElementsControl.altimeterBool = isOn; }
    public void setDebugGUI(bool isOn) { hudElementsControl.debugGUIBool = isOn; }
    public void setAll(bool isOn)
    {
        setTimer(isOn);
        setScore(isOn);
        setFPS(isOn);
        setArrow(isOn);
        setRingCount(isOn);
        setSpeed(isOn);
        setSpeedBar(isOn);
        setAltimeter(isOn);
        setDebugGUI(isOn);
        hudElementsControl.overAllBool = isOn;
    }

    //For level use
    public void gameStart()
    {
        timerText.SetActive(hudElementsControl.timerBool);
        scoreText.SetActive(hudElementsControl.scoreBool);
        fpsText.SetActive(hudElementsControl.fpsBool);
        arrow.SetActive(hudElementsControl.arrowBool);
        ringCountText.SetActive(hudElementsControl.ringCountBool);
        speedText.SetActive(hudElementsControl.speedBool);
        speedBar.SetActive(hudElementsControl.speedBarBool);
        altimeterText.SetActive(hudElementsControl.altimeterBool);
        debugGUI.SetActive(hudElementsControl.debugGUIBool);
        
    }
 
    //For menu's use
    public void menuStart()
    {
        if (fpsText.activeSelf)
        {
            fpsText.SetActive(false);
        }
        if(timerText.activeSelf)
        {
            timerText.SetActive(false);
        }
        if(scoreText.activeSelf)
        {
            scoreText.SetActive(false);
        }
        if (arrow.activeSelf)
        {
            arrow.SetActive(false);
        }
        if(ringCountText.activeSelf)
        {
            ringCountText.SetActive(false);
        }
        if (speedText.activeSelf)
        {
            speedText.SetActive(false);
        }
        if (speedBar.activeSelf)
        {
            speedBar.SetActive(false);
        }
        if (altimeterText.activeSelf)
        {
            altimeterText.SetActive(false);
        }
        if (debugGUI.activeSelf)
        {
            debugGUI.SetActive(false);
        }
    }
   

    void setHud(bool isOn)
    {
        if (isOn)
        {
            gameStart();
        }
        else
        {
            menuStart();
        }
    }

    private void OnEnable()
    {
        fpsText = GetComponentInChildren<FPSTextUpdateScript>().gameObject;
        timerText = GetComponentInChildren<TimerTextUpdateScript>().gameObject;
        scoreText = GetComponentInChildren<ScoreTextUpdateScript>().gameObject;
        arrow = GetComponentInChildren<arrowPointAtUpdater>().gameObject;
        ringCountText = GetComponentInChildren<RingCountTextUpdate>().gameObject;
        speedText = GetComponentInChildren<SpeedUpdate>().gameObject;
        speedBar = GetComponentInChildren<speedBarUpdater>().gameObject;
        altimeterText = GetComponentInChildren<altimeterTextUpdater>().gameObject;
        debugGUI = GameObject.Find("GUI");
     

        hudElementsControl.timerBool = true;
        hudElementsControl.scoreBool = true;
        hudElementsControl.fpsBool = true;
        hudElementsControl.arrowBool = true;
        hudElementsControl.ringCountBool = true;
        hudElementsControl.speedBool = true;
        hudElementsControl.speedBarBool = true;
        hudElementsControl.altimeterBool = true;
        hudElementsControl.debugGUIBool = true;
        hudElementsControl.overAllBool = true;

        EventManager.OnToggleHud += setHud;
    }

    private void OnDisable()
    {
        EventManager.OnToggleHud -= setHud;
    }
}
