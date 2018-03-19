using UnityEngine;
using UnityEngine.PostProcessing;
using static KeyInputManager.VR;
public enum GameState { HubWorld, GamePlay, SceneTransition };
public enum GameMode { Continuous, Cursed, Free, Race, GameModesSize };
public enum GameDifficulty { Easy, Normal, Hard, GameDifficultiesSize };
public class GameManager : MonoBehaviour
{
    public static GameState gameState = GameState.HubWorld;
    public static GameMode gameMode = GameMode.Continuous;
    public static GameDifficulty gameDifficulty = GameDifficulty.Normal;
    public static int AI_Number = 0;
    public static int MaxLap = 1;
    public static int lastPortalBuildIndex = -1;
    public static GameObject player = null;
    private static GameManager instance = null;
    [SerializeField] private GameObject playerPrefab = null;
    private void Awake()
    {
        if (null == instance)
            instance = this;
        if (this == instance)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(player = Instantiate(playerPrefab));
            if (VRPresent)
                player.GetComponentInChildren<PostProcessingBehaviour>().profile.vignette.enabled = true;
            GameSettings.GetEnum("GameDifficulty", ref gameDifficulty);
            GameSettings.GetEnum("GameMode", ref gameMode);
#if DEBUGGER
            GameSettings.GetBool("DebugSpeed", ref BoardManager.debugSpeedEnabled);
#endif
            GetComponent<BoardManager>().SetupBoardManager();
            LevelManager.SetupLevelManager();
            ScoreManager.SetupScoreManager();
            KeyInputManager.SetupKeyInputManager();
        }
        else
            Destroy(gameObject);
    }
    private void OnEnable()
    {
        GameSettings.GetBool("RingPath", ref LevelManager.RingPathIsOn);
        GameSettings.GetBool("Respawn", ref ScoreManager.respawnEnabled);
        GameSettings.GetBool("MirrorMode", ref LevelManager.mirrorMode);
        GameSettings.GetBool("ReverseMode", ref LevelManager.reverseMode);
        GameSettings.GetInt("NumAI", ref AI_Number);
        GameSettings.GetInt("NumLaps", ref MaxLap);
        GameSettings.GetEnum("Weather", ref WeatherScript.currentWeather);
        GameSettings.GetEnum("TimeOfDay", ref DayNightScript.currentTimeOfDay);
    }
    private void OnDisable()
    {
        GameSettings.SetBool("RingPath", LevelManager.RingPathIsOn);
        GameSettings.SetBool("Respawn", ScoreManager.respawnEnabled);
#if DEBUGGER
        GameSettings.SetBool("DebugSpeed", BoardManager.debugSpeedEnabled);
#endif
        GameSettings.SetBool("MirrorMode", LevelManager.mirrorMode);
        GameSettings.SetBool("ReverseMode", LevelManager.reverseMode);
        GameSettings.SetInt("NumAI", AI_Number);
        GameSettings.SetInt("NumLaps", MaxLap);
        GameSettings.SetEnum("Weather", WeatherScript.currentWeather);
        GameSettings.SetEnum("TimeOfDay", DayNightScript.currentTimeOfDay);
        GameSettings.SetEnum("GameDifficulty", gameDifficulty);
        GameSettings.SetEnum("GameMode", gameMode);
    }
    private void OnDestroy() => GameSettings.Save();
    private static bool deleteScores = false;
    public static void DeleteScoresOnExit() => deleteScores = true;
    public static bool DoNotSave => deleteScores;
    private void OnApplicationQuit()
    {
        if (deleteScores)
            try { System.IO.File.Delete(Application.persistentDataPath + "/scores.gd"); }
            catch (System.Exception e) { Debug.LogWarning($"Failed to delete scores file: ({e.Message})"); }
    }
}