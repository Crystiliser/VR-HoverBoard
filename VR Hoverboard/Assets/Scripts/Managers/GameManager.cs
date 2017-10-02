using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

//our ManagerLoader prefab, will ensure that an instance of GameManager is loaded
public class GameManager : MonoBehaviour
{
    //store our game game state
    public ManagerClasses.GameState gameState = new ManagerClasses.GameState();

    //store our game mode
    public ManagerClasses.GameMode gameMode = new ManagerClasses.GameMode();

    //store our round timer
    public ManagerClasses.RoundTimer roundTimer;

    //last level and mode we were in
    [HideInInspector] public GameModes lastMode;
    [HideInInspector] public int lastPortalBuildIndex = -1;
    [HideInInspector] public int lastBuildIndex = -1;

    //set our player prefab through the inspector
    [SerializeField] GameObject playerPrefab;

    //variable for singleton, static makes this variable the same through all GameManager objects
    public static GameManager instance = null;

    //variable to store our player clone
    public static GameObject player = null;

    //store our managers
    [HideInInspector] public ScoreManager scoreScript;
    [HideInInspector] public LevelManager levelScript;
    [HideInInspector] public BoardManager boardScript;
    [HideInInspector] public KeyInputManager keyInputScript;

    void Awake()
    {
        //make sure we only have one instance of GameManager
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        //ensures that our game manager persists between scenes
        DontDestroyOnLoad(gameObject);

        //store our managers
        scoreScript = GetComponent<ScoreManager>();
        levelScript = GetComponent<LevelManager>();
        boardScript = GetComponent<BoardManager>();
        keyInputScript = GetComponent<KeyInputManager>();

        //setup our round timer
        roundTimer = new ManagerClasses.RoundTimer();

        //Instantiate our player, store the clone, then make sure it persists between scenes
        player = Instantiate(playerPrefab);
        DontDestroyOnLoad(player);

        //set the game to run in the background
        Application.runInBackground = true;

        InitGame();

        levelScript.RingPathIsOn = (0 != PlayerPrefs.GetInt("RingPath", levelScript.RingPathIsOn ? 1 : 0));
        scoreScript.respawnEnabled = (0 != PlayerPrefs.GetInt("Respawn", scoreScript.respawnEnabled ? 1 : 0));
        boardScript.debugSpeedEnabled = (0 != PlayerPrefs.GetInt("DebugSpeed", boardScript.debugSpeedEnabled ? 1 : 0));
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("RingPath", levelScript.RingPathIsOn ? 1 : 0);
        PlayerPrefs.SetInt("Respawn", scoreScript.respawnEnabled ? 1 : 0);
        PlayerPrefs.SetInt("DebugSpeed", boardScript.debugSpeedEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    //using this instead of Awake() in our scripts allows us to control the execution order
    void InitGame()
    {
        boardScript.SetupBoardManager(player);
        levelScript.SetupLevelManager(gameState, player, instance);
        scoreScript.SetupScoreManager();
        keyInputScript.setupKeyInputManager(gameState);
    }
}