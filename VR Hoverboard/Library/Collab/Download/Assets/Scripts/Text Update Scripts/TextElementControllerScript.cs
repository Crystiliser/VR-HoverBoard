using UnityEngine;

public class TextElementControllerScript : MonoBehaviour
{
    private GameObject fpsText;
    private GameObject timerText;
    private GameObject scoreText;
    private GameObject arrow;
    private GameObject ringCountText;
    private GameObject speedText;
    private GameObject speedBar;
    private GameObject altimeterText;
    private GameObject debugGUI;

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

    public void setTimer(bool isOn) { hudElementsControl.timerBool = isOn; PlayerPrefs.SetInt("HudTimerBool", isOn ? 1 : 0); }
    public void setScore(bool isOn) { hudElementsControl.scoreBool = isOn; PlayerPrefs.SetInt("HudScoreBool", isOn ? 1 : 0); }
    public void setFPS(bool isOn) { hudElementsControl.fpsBool = isOn; PlayerPrefs.SetInt("HudFpsBool", isOn ? 1 : 0); }
    public void setArrow(bool isOn) { hudElementsControl.arrowBool = isOn; PlayerPrefs.SetInt("HudArrowBool", isOn ? 1 : 0); }
    public void setRingCount(bool isOn) { hudElementsControl.ringCountBool = isOn; PlayerPrefs.SetInt("HudRingCountBool", isOn ? 1 : 0); }
    public void setSpeed(bool isOn) { hudElementsControl.speedBool = isOn; PlayerPrefs.SetInt("HudSpeedBool", isOn ? 1 : 0); }
    public void setSpeedBar(bool isOn) { hudElementsControl.speedBarBool = isOn; PlayerPrefs.SetInt("HudSpeedBarBool", isOn ? 1 : 0); }
    public void setAltimeter(bool isOn) { hudElementsControl.altimeterBool = isOn; PlayerPrefs.SetInt("HudAltimeterBool", isOn ? 1 : 0); }
    public void setDebugGUI(bool isOn) { hudElementsControl.debugGUIBool = isOn; PlayerPrefs.SetInt("HudDebugGUIBool", isOn ? 1 : 0); }
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
            fpsText.SetActive(false);
        if (timerText.activeSelf)
            timerText.SetActive(false);
        if (scoreText.activeSelf)
            scoreText.SetActive(false);
        if (arrow.activeSelf)
            arrow.SetActive(false);
        if (ringCountText.activeSelf)
            ringCountText.SetActive(false);
        if (speedText.activeSelf)
            speedText.SetActive(false);
        if (speedBar.activeSelf)
            speedBar.SetActive(false);
        if (altimeterText.activeSelf)
            altimeterText.SetActive(false);
        if (debugGUI.activeSelf)
            debugGUI.SetActive(false);
    }


    void setHud(bool isOn)
    {
        if (isOn)
            gameStart();
        else
            menuStart();
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

        GetPlayerPrefs();

        EventManager.OnToggleHud += setHud;
    }

    private void OnDisable()
    {
        EventManager.OnToggleHud -= setHud;
    }
    private void GetPlayerPrefs()
    {
        hudElementsControl.timerBool = (0 != PlayerPrefs.GetInt("HudTimerBool", 1));
        hudElementsControl.scoreBool = (0 != PlayerPrefs.GetInt("HudScoreBool", 1));
        hudElementsControl.fpsBool = (0 != PlayerPrefs.GetInt("HudFpsBool", 1));
        hudElementsControl.arrowBool = (0 != PlayerPrefs.GetInt("HudArrowBool", 1));
        hudElementsControl.ringCountBool = (0 != PlayerPrefs.GetInt("HudRingCountBool", 1));
        hudElementsControl.speedBool = (0 != PlayerPrefs.GetInt("HudSpeedBool", 1));
        hudElementsControl.speedBarBool = (0 != PlayerPrefs.GetInt("HudSpeedBarBool", 1));
        hudElementsControl.altimeterBool = (0 != PlayerPrefs.GetInt("HudAltimeterBool", 1));
        hudElementsControl.debugGUIBool = (0 != PlayerPrefs.GetInt("HudDebugGUIBool", 1));
        hudElementsControl.overAllBool = (0 != PlayerPrefs.GetInt("HudOverAllBool", 1));
        PlayerPrefs.SetInt("HudTimerBool", hudElementsControl.timerBool ? 1 : 0);
        PlayerPrefs.SetInt("HudScoreBool", hudElementsControl.scoreBool ? 1 : 0);
        PlayerPrefs.SetInt("HudFpsBool", hudElementsControl.fpsBool ? 1 : 0);
        PlayerPrefs.SetInt("HudArrowBool", hudElementsControl.arrowBool ? 1 : 0);
        PlayerPrefs.SetInt("HudRingCountBool", hudElementsControl.ringCountBool ? 1 : 0);
        PlayerPrefs.SetInt("HudSpeedBool", hudElementsControl.speedBool ? 1 : 0);
        PlayerPrefs.SetInt("HudSpeedBarBool", hudElementsControl.speedBarBool ? 1 : 0);
        PlayerPrefs.SetInt("HudAltimeterBool", hudElementsControl.altimeterBool ? 1 : 0);
        PlayerPrefs.SetInt("HudDebugGUIBool", hudElementsControl.debugGUIBool ? 1 : 0);
        PlayerPrefs.SetInt("HudOverAllBool", hudElementsControl.overAllBool ? 1 : 0);
        PlayerPrefs.Save();
    }
}
