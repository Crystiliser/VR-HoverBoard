using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public struct scoreStruct
    {
        public Vector3[] positions;
        public Quaternion[] rotations;
        public int score;
        public float time;
        public int board;
        public string name;
    }

    public struct modeScores
    {
        public scoreStruct[] scores;
    }
    
    public struct levelScores
    {
        public modeScores[] modes;
    }
    
    public levelScores[] topScores;
    int currentAmoutFilled = 0;

    void initScores()
    {
        topScores = SaveLoader.load();
        if (topScores == null)
        {
            topScores = new levelScores[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < topScores.Length; i++)
            {
                topScores[i].modes = new modeScores[4];
                for (int j = 0; j < 4; j++)
                {   
                    topScores[i].modes[j].scores = new scoreStruct[10];
                }
            }
        }
    }


    public float baseScorePerRing = 100;
    public ManagerClasses.RoundTimer roundTimer = new ManagerClasses.RoundTimer();

    ManagerClasses.GameState gameState;
    PlayerRespawn playerRespawnScript;
    Transform[] spawnPoints;
    int respawnCount, maxRespawnCount;

    //used by our HUD and updated through RingScoreScript
    [HideInInspector] public int ringHitCount = 0;

    //values updated by our RingScoreScript
    [HideInInspector] public int score;
    [HideInInspector] public float prevRingBonusTime;
    [HideInInspector] public Transform prevRingTransform;

    public bool respawnEnabled = true;

    //this will get called by our game manager
    public void SetupScoreManager()
    {
        gameState = GameManager.instance.gameState;
        spawnPoints = GameManager.instance.levelScript.spawnPoints;
        playerRespawnScript = GameManager.player.GetComponent<PlayerRespawn>();

        score = respawnCount = 0;
        maxRespawnCount = 3;
        initScores();
    }

    //called when you hit the last ring in the level, do all setting score stuff here
    public void levelEnd()
    {
        scoreStruct newScore = new scoreStruct();
        newScore.score = score;
        newScore.board = (int)GameManager.instance.boardScript.currentBoardSelection;
        positionRecorder recorder = GameManager.player.GetComponent<positionRecorder>();
        newScore.positions = recorder.positions.ToArray();
        newScore.rotations = recorder.rotations.ToArray();
        newScore.time = roundTimer.TimeInLevel;
        int mode = (int)GameManager.instance.gameMode.currentMode;

        int level = SceneManager.GetActiveScene().buildIndex;

        if (currentAmoutFilled < 10)
        {
            topScores[level].modes[mode].scores[currentAmoutFilled] = newScore;
            currentAmoutFilled++;
        }
        else
        {
            topScores[level].modes[mode].scores[9] = newScore;
        }
        sort(topScores[level].modes[mode].scores, currentAmoutFilled);
    }
    
    void sort(scoreStruct[] scores, int arrayLength)
    {
        int curr = 1;
        while (curr < arrayLength)
        {
            scoreStruct storedScore = scores[curr];

            int comparer = curr - 1;
            while (comparer >= 0)
            {
                if (scores[comparer].score < storedScore.score)
                {
                    scores[comparer + 1] = scores[comparer];
                    --comparer;
                }
                else if (scores[comparer].score == storedScore.score && scores[comparer].time < storedScore.time)
                {
                    scores[comparer + 1] = scores[comparer];
                    --comparer;
                }
                else
                {
                    break;
                }
            }

            scores[comparer + 1] = storedScore;
            ++curr;
        }
    }


    //set the prevRingTransform to the spawn point whenever we load in a new scene, and restart our roundTimer
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        prevRingTransform = spawnPoints[SceneManager.GetActiveScene().buildIndex];
        roundTimer.TimeLeft = 5f;
        prevRingBonusTime = 0f;
        respawnCount = 0;
    }

    private void Update()
    {
        if (gameState.currentState == GameStates.GamePlay)
        {
            //keep updating the timers 
            if (roundTimer.TimeLeft > 0f)
                roundTimer.UpdateTimers();
            else if (respawnEnabled && !playerRespawnScript.IsRespawning)
            {
                //if the player has reached the maxRespawnCount, then send him/her back to the hub world
                if (respawnCount < maxRespawnCount)
                    playerRespawnScript.RespawnPlayer(prevRingTransform, 5f + prevRingBonusTime);
                else
                    EventManager.OnTriggerTransition(1);

                ++respawnCount;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }
}
