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

        public bool isLastScoreInput;
    }

    public struct levelCurseScores
    {
        public scoreStruct[] curseScores;
    }

    public struct continuousScores
    {
        //one score per level
        public scoreStruct[] levels;
        public bool isLastScoreInput;
        public string name;
    }

    public levelCurseScores[] topCurseScores;
    int currentAmoutFilled = 0;

    //top 10 scores for continuous mode
    public continuousScores[] topContinuousScores;
    int curFilledCont = -1;
    [HideInInspector] public bool firstPortal = true;

    void initScores()
    {
        topCurseScores = SaveLoader.loadCurseScores();
        if (topCurseScores == null)
        {
            topCurseScores = new levelCurseScores[SceneManager.sceneCountInBuildSettings];
            for (int i = 0; i < topCurseScores.Length; i++)
            {
                topCurseScores[i].curseScores = new scoreStruct[10];
            }
        }

        topContinuousScores = SaveLoader.loadContinuousScores();
        if (topContinuousScores == null)
        {
            topContinuousScores = new continuousScores[10];
            for (int i = 0; i < topContinuousScores.Length; i++)
            {
                topContinuousScores[i].levels = new scoreStruct[SceneManager.sceneCountInBuildSettings];
            }
        }

    }

    public float baseScorePerRing = 100;

    ManagerClasses.RoundTimer roundTimer;
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

    [HideInInspector] public bool respawnEnabled = true;

    //this will get called by our game manager
    public void SetupScoreManager()
    {
        gameState = GameManager.instance.gameState;
        spawnPoints = GameManager.instance.levelScript.spawnPoints;
        playerRespawnScript = GameManager.player.GetComponent<PlayerRespawn>();
        roundTimer = GameManager.instance.roundTimer;

        score = respawnCount = 0;
        maxRespawnCount = 3;
        initScores();
    }

    //called when you hit the last ring in the level, do all setting score stuff here
    public void levelEnd()
    {
        positionRecorder recorder;
        int level;
        switch (GameManager.instance.gameMode.currentMode)
        {
            case GameModes.Continuous:
                
                if (firstPortal)
                {
                    curFilledCont++;
                    firstPortal = false;
                }
                scoreStruct newLevelScore = new scoreStruct();
                newLevelScore.score = score;
                newLevelScore.time = roundTimer.TimeInLevel;
                newLevelScore.board = (int)GameManager.instance.boardScript.currentBoardSelection;
                recorder = GameManager.player.GetComponent<positionRecorder>();
                newLevelScore.positions = recorder.positions.ToArray();
                newLevelScore.rotations = recorder.rotations.ToArray();

                level = SceneManager.GetActiveScene().buildIndex;

                if (curFilledCont < 10)
                {
                    if (topContinuousScores[curFilledCont].levels[level].positions == null)
                    {
                        topContinuousScores[curFilledCont].levels[level] = newLevelScore;
                    }
                    else
                    {
                        topContinuousScores[curFilledCont].levels[level] = compareContinuousScores(level, topContinuousScores[curFilledCont].levels[level], newLevelScore);
                    }
                    topContinuousScores[curFilledCont].isLastScoreInput = true;
                }
                else
                {
                    topContinuousScores[9].levels[level] = compareContinuousScores(level, topContinuousScores[9].levels[level], newLevelScore);
                    topContinuousScores[9].isLastScoreInput = true;
                }

                sortContinuousScores(topContinuousScores, topContinuousScores.Length);
                break;

            case GameModes.Cursed:

                //setup new score object
                scoreStruct newCurseScore = new scoreStruct();
                newCurseScore.score = score;
                newCurseScore.time = roundTimer.TimeLeft;
                newCurseScore.board = (int)GameManager.instance.boardScript.currentBoardSelection;
                recorder = GameManager.player.GetComponent<positionRecorder>();
                newCurseScore.positions = recorder.positions.ToArray();
                newCurseScore.rotations = recorder.rotations.ToArray();
                newCurseScore.isLastScoreInput = true;


                level = SceneManager.GetActiveScene().buildIndex;


                if (currentAmoutFilled < 10)
                {
                    topCurseScores[level].curseScores[currentAmoutFilled] = newCurseScore;
                    currentAmoutFilled++;
                }
                else
                {
                    topCurseScores[level].curseScores[9] = newCurseScore;
                }

                sortCurseScores(topCurseScores[level].curseScores, currentAmoutFilled);
                break;

            case GameModes.Free:
                break;
            default:
                break;
        }

    }

    void sortCurseScores(scoreStruct[] scores, int arrayLength)
    {
        int curr = 1;
        scoreStruct storedScore;
        while (curr < arrayLength)
        {
            storedScore = scores[curr];

            int comparer = curr - 1;
            while (comparer >= 0)
            {
                if (scores[comparer].score < storedScore.score)
                {
                    scores[comparer + 1] = scores[comparer];
                    comparer--;
                }
                else if (scores[comparer].score == storedScore.score && scores[comparer].time > storedScore.time)
                {
                    scores[comparer + 1] = scores[comparer];
                    comparer--;
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

    void sortContinuousScores(continuousScores[] array, int length)
    {
        int i, j ;
        continuousScores key;
        for (i = 1; i < length; i++)
        {
            key = array[i];

            //cumulative numbers for this play of continuous mode
            int keyScore = 0;
            float keyTime = 0;
            for (int k = 0; k < array[i].levels.Length; k++)
            {
                keyScore += array[i].levels[k].score;
                keyTime += array[i].levels[k].score;
            }


            j = i - 1;
            while (j >= 0)
            {
                int cumulativeScore2 = 0;
                float totalTime2 = 0;
                for (int k = 0; k < array[j].levels.Length; k++)
                {
                    cumulativeScore2 += array[j].levels[k].score;
                    totalTime2 += array[j].levels[k].score;
                }
                
                //actual checking
                if (cumulativeScore2 < keyScore)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                else if (cumulativeScore2 == keyScore && totalTime2 < keyTime)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                else
                {
                    break;
                }
            }

            array[j + 1] = key;
        }
    }

    scoreStruct compareContinuousScores(int set, scoreStruct one, scoreStruct two)
    {
        int oneCumulativeScore = 0;
        float oneTotalTime = 0;
        for (int j = 0; j < topContinuousScores[set].levels.Length; j++)
        {
            oneCumulativeScore += topContinuousScores[set].levels[j].score;
            oneTotalTime += topContinuousScores[set].levels[j].score;
        }

        int twoCumulativeScore = 0;
        float twoTotalTime = 0;
        for (int j = 0; j < topContinuousScores[set].levels.Length; j++)
        {
            twoCumulativeScore += topContinuousScores[set].levels[j].score;
            twoTotalTime += topContinuousScores[set].levels[j].score;
        }

        if (oneCumulativeScore > twoCumulativeScore)
        {
            return one;
        }
        else if (oneCumulativeScore == twoCumulativeScore && oneTotalTime > twoTotalTime)
        {
            return one;
        }
        else
        {
            return two;
        }
    }

    //set the prevRingTransform to the spawn point whenever we load in a new scene, and restart our roundTimer
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        prevRingTransform = spawnPoints[SceneManager.GetActiveScene().buildIndex];
        roundTimer.TimeLeft = 5f;
        roundTimer.TimeInLevel = 0;
        prevRingBonusTime = 0f;
        respawnCount = 0;

        switch (GameManager.instance.gameMode.currentMode)
        {
            case GameModes.Continuous:
                respawnEnabled = false;
                break;
            case GameModes.Cursed:
                respawnEnabled = true;
                break;
            case GameModes.Free:
                respawnEnabled = false;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (gameState.currentState == GameStates.GamePlay)
        {
            //keep updating the timers 
            roundTimer.UpdateTimers();
            if (respawnEnabled && roundTimer.TimeLeft <= 0 && !playerRespawnScript.IsRespawning)
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
