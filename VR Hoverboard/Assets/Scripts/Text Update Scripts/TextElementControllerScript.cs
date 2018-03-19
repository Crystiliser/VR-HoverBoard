using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextElementControllerScript : MonoBehaviour
{
    private GameObject timerText;
    private GameObject bonusTimeText;
    private GameObject scoreText;
    private GameObject scoreMulti;
    [SerializeField] private GameObject[] arrow;
    private GameObject speedText;
    private GameObject speedBar;
    private GameObject bestlap_time;
    /* private GameObject checkpoint_time;
     private GameObject time_difference;*/
    private GameObject player_list;
    private GameObject current_lap_time;
    //private GameObject position_text;
    private GameObject lap_text;
    public GameObject Compass_Display;
    [SerializeField] private Color HUD_Color;
    public delegate void HudColorChangedEvent(Color newColor);
    public static event HudColorChangedEvent OnHudColorChanged;
    public Color HudColor { get { return HUD_Color; } set { GameSettings.SetColor("HudColor", HUD_Color = value); OnHudColorChanged?.Invoke(HUD_Color); } }
    public struct HudElementsBools
    {
        public bool timerBool;
        public bool scoreBool;
        public bool scoremultiBool;
        public bool arrowBool;
        public bool speedBool;
        public bool speedBarBool;
        public bool bestlapBool;
        public bool checkpoint_timeBool;
        public bool time_differenceBool;
        public bool player_listBool;
        public bool current_lap_timeBool;
        public bool positionBool;
        public bool lapBool;
        public bool reticleBool;
        public bool compassBool;
    }
    public HudElementsBools hudElementsControl;
    public void setTimer(bool isOn) => hudElementsControl.timerBool = GameSettings.SetBool("HudTimer", isOn);
    public void setScore(bool isOn) => hudElementsControl.scoreBool = GameSettings.SetBool("HudScore", isOn);
    public void setScoreMulti(bool isOn) => hudElementsControl.scoremultiBool = GameSettings.SetBool("HudScoremulti", isOn);
    public void setArrow(bool isOn) => hudElementsControl.arrowBool = GameSettings.SetBool("HudArrow", isOn);
    public void setSpeed(bool isOn) => hudElementsControl.speedBool = GameSettings.SetBool("HudSpeed", isOn);
    public void setSpeedBar(bool isOn) => hudElementsControl.speedBarBool = GameSettings.SetBool("HudSpeedBar", isOn);
    public void setBestLap(bool isOn) => hudElementsControl.bestlapBool = GameSettings.SetBool("HudBestLap", isOn);
    //public void setCheckpoint_time(bool isOn) => hudElementsControl.checkpoint_timeBool = GameSettings.SetBool("HudCheckpointTime", isOn);
    // public void setTimeDifference(bool isOn) => hudElementsControl.time_differenceBool = GameSettings.SetBool("HudTimeDifference", isOn);
    public void setPlayerList(bool isOn) => hudElementsControl.player_listBool = GameSettings.SetBool("HudPlayerList", isOn);
    public void setCurrentLapTime(bool isOn) => hudElementsControl.current_lap_timeBool = GameSettings.SetBool("HudCurrentLapTime", isOn);
    //public void setPositionText(bool isOn) => hudElementsControl.positionBool = GameSettings.SetBool("HudPosition", isOn);
    public void setLapText(bool isOn) => hudElementsControl.lapBool = GameSettings.SetBool("HudLap", isOn);
    public void setReticle(bool isOn) => hudElementsControl.reticleBool = GameSettings.SetBool("HudReticle", isOn);
    public void setCompass(bool isOn) => hudElementsControl.compassBool = GameSettings.SetBool("HudCompass", isOn);
    public void setAll(bool isOn)
    {
        setTimer(isOn);
        setScore(isOn);
        setScoreMulti(isOn);
        setArrow(isOn);
        setSpeed(isOn);
        setBestLap(isOn);
        // setCheckpoint_time(isOn);
        //setTimeDifference(isOn);
        setPlayerList(isOn);
        setCurrentLapTime(isOn);
        //setPositionText(isOn);
        setLapText(isOn);
        setSpeedBar(isOn);
        setReticle(isOn);
        setCompass(isOn);
    }
    private void GameStart()
    {
        GameMode currentMode = GameManager.gameMode;

        timerText.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        bonusTimeText.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        scoreText.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        scoreMulti.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        bestlap_time.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        //checkpoint_time.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        //time_difference.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        player_list.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        current_lap_time.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        //position_text.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        lap_text.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        speedText.GetComponent<TextMeshProUGUI>().color = HUD_Color;
        speedBar.GetComponent<Image>().color = HUD_Color;
        Compass_Display.GetComponentInChildren<TextMeshPro>(true).color = HUD_Color;
        Compass_Display.GetComponentsInParent<Image>(true)[0].color = HUD_Color;

        timerText.SetActive(hudElementsControl.timerBool);
        bonusTimeText.SetActive(hudElementsControl.timerBool && GameMode.Cursed == currentMode);
        scoreText.SetActive(hudElementsControl.scoreBool);
        scoreMulti.SetActive(hudElementsControl.scoremultiBool);
        for (int i = 0; i < arrow.Length; ++i)
            arrow[i].SetActive(hudElementsControl.arrowBool);
        bestlap_time.SetActive(hudElementsControl.bestlapBool && GameMode.Race == currentMode);
        //checkpoint_time.SetActive(hudElementsControl.checkpoint_timeBool);
        //time_difference.SetActive(hudElementsControl.time_differenceBool);
        player_list.SetActive(hudElementsControl.player_listBool && GameMode.Race == currentMode);
        current_lap_time.SetActive(hudElementsControl.current_lap_timeBool && GameMode.Race == currentMode);
        //position_text.SetActive(hudElementsControl.positionBool && GameMode.Race == currentMode);
        lap_text.GetComponent<TextMeshProUGUI>().enabled = hudElementsControl.lapBool && (GameMode.Race == currentMode || GameMode.Cursed == currentMode);
        speedText.SetActive(hudElementsControl.speedBool);
        speedBar.SetActive(hudElementsControl.speedBarBool);
        Compass_Display.SetActive(hudElementsControl.compassBool);
    }
    private void MenuStart()
    {
        timerText.SetActive(false);
        bonusTimeText.SetActive(false);
        scoreText.SetActive(false);
        scoreMulti.SetActive(false);
        bestlap_time.SetActive(false);
        //checkpoint_time.SetActive(false);
        //time_difference.SetActive(false);
        player_list.SetActive(false);
        current_lap_time.SetActive(false);
        //position_text.SetActive(false);
        lap_text.SetActive(true);
        lap_text.GetComponent<TextMeshProUGUI>().enabled = false;
        for (int i = 0; i < arrow.Length; ++i)
            arrow[i].SetActive(false);
        speedText.SetActive(false);
        speedBar.SetActive(false);
        Compass_Display.SetActive(false);
    }
    private void SetHud(bool isOn)
    {
        if (isOn)
            GameStart();
        else
            MenuStart();
    }
    private void OnEnable()
    {
        timerText = GetComponentInChildren<TimerTextUpdateScript>().gameObject;
        bonusTimeText = GetComponentInChildren<bonusTimeTextUpdater>().gameObject;
        scoreText = GetComponentInChildren<ScoreTextUpdateScript>().gameObject;
        scoreMulti = GetComponentInChildren<Score_Multi_Script>().gameObject;
        bestlap_time = GetComponentInChildren<Best_Lap_Text_script>().gameObject;
        //checkpoint_time = GetComponentInChildren<Checkpoint_Time_Text_Script>().gameObject;
        //time_difference = GetComponentInChildren<Time_Gap_Text_Script>().gameObject;
        player_list = GetComponentInChildren<Player_List_Text_script>().gameObject;
        current_lap_time = GetComponentInChildren<Current_Lap_Time_Text>().gameObject;
        //position_text = GetComponentInChildren<Position_Text_script>().gameObject;
        lap_text = GetComponentInChildren<Lap_Text_script>().gameObject;
        speedText = GetComponentInChildren<SpeedUpdate>().gameObject;
        speedBar = GetComponentInChildren<speedBarUpdater>().gameObject;
        Compass_Display = GetComponentInChildren<Compass_rotate>(true).gameObject;
        GetPlayerPrefs();
        EventManager.OnToggleHud += SetHud;
    }
    private void OnDisable()
    {
        EventManager.OnToggleHud -= SetHud;
    }
    private void GetPlayerPrefs()
    {
        hudElementsControl.timerBool = GameSettings.GetBool("HudTimer", true);
        hudElementsControl.scoreBool = GameSettings.GetBool("HudScore", true);
        hudElementsControl.scoremultiBool = GameSettings.GetBool("HudScoremulti", true);
        hudElementsControl.arrowBool = GameSettings.GetBool("HudArrow", true);
        hudElementsControl.speedBool = GameSettings.GetBool("HudSpeed", true);
        hudElementsControl.speedBarBool = GameSettings.GetBool("HudSpeedBar", true);
        hudElementsControl.bestlapBool = GameSettings.GetBool("HudBestLap", true);
        // hudElementsControl.time_differenceBool = GameSettings.GetBool("HudTimeDifference", true);
        hudElementsControl.player_listBool = GameSettings.GetBool("HudPlayerList", true);
        hudElementsControl.current_lap_timeBool = GameSettings.GetBool("HudCurrentLapTime", true);
        //hudElementsControl.positionBool = GameSettings.GetBool("HudPosition", true);
        //hudElementsControl.checkpoint_timeBool = GameSettings.GetBool("HudCheckpointTime", true);
        hudElementsControl.lapBool = GameSettings.GetBool("HudLap", true);
        hudElementsControl.reticleBool = GameSettings.GetBool("HudReticle", true);
        hudElementsControl.compassBool = GameSettings.GetBool("HudCompass", true);
        GameSettings.GetColor("HudColor", ref HUD_Color);
        OnHudColorChanged?.Invoke(HUD_Color);
    }
}