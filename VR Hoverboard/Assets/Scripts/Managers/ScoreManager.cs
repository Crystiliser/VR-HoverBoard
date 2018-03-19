using UnityEngine;
using UnityEngine.SceneManagement;
public class ScoreManager : MonoBehaviour
{
    public struct ScoreData
    {
        public Vector3[] positions;
        public Quaternion[] rotations;
        public GameDifficulty difficulty;
        public string name;
        public float time;
        public int score, board;
        public bool isLastScoreInput;
    }
    public struct CursedScores
    {
        public ScoreData[] cursedScores;
        public int currentAmoutFilled;
    }
    public struct RaceScores
    {
        public ScoreData[] racescores;
        public int currentAmoutFilled;
    }
    public struct ContinuousScores
    {
        public ScoreData[] levels;
        public GameDifficulty difficulty;
        public string name;
        public bool isLastScoreInput;
    }
    public static CursedScores[] topCursedScores = null;
    public static ContinuousScores[] topContinuousScores = null;
    public static RaceScores[] topRaceScores = null;
    public static int curFilledCont = -1, ringHitCount = 0, score = 0;
    public static bool firstPortal = true, respawnEnabled = true;
    public static float baseScorePerRing = 100.0f;
    private static PlayerRespawn playerRespawnScript = null;
    private static int respawnCount = 0;
    private const int maxRespawnCount = 3;
    public static float prevRingBonusTime = 0.0f, score_multiplier = 1.0f;
    public static Transform prevRingTransform = null;
    private static void InitScores()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        topCursedScores = SaveLoader.LoadCurseScores();
        topRaceScores = SaveLoader.LoadRaceScores();
        topContinuousScores = SaveLoader.LoadContinuousScores();
        if (null == topCursedScores)
        {
            topCursedScores = new CursedScores[sceneCount];
            for (int i = 0; i < sceneCount; ++i)
            {
                topCursedScores[i].cursedScores = new ScoreData[10];
                topCursedScores[i].currentAmoutFilled = 0;
            }
        }
        if (null == topContinuousScores)
        {
            topContinuousScores = new ContinuousScores[10];
            for (int i = 0; i < 10; ++i)
                topContinuousScores[i].levels = new ScoreData[sceneCount];
        }
        if (null == topRaceScores)
        {

            topRaceScores = new RaceScores[sceneCount];
            for (int i = 0; i < sceneCount; ++i)
            {
                topRaceScores[i].racescores = new ScoreData[10];
                topRaceScores[i].currentAmoutFilled = 0;
            }
        }
    }
    public static void SetupScoreManager()
    {
        playerRespawnScript = GameManager.player.GetComponent<PlayerRespawn>();
        score = respawnCount = 0;
        InitScores();
    }
    public static void LevelEnd()
    {
        try { TryLevelEnd(); }
        catch { Debug.Log("scores corrupt.. scores file will be reset on exit"); GameManager.DeleteScoresOnExit(); }
    }
    private static void TryLevelEnd()
    {
        positionRecorder recorder = GameManager.player.GetComponent<positionRecorder>();
        int level = SceneManager.GetActiveScene().buildIndex;
        ScoreData newLevelScore = new ScoreData();
        switch (GameManager.gameMode)
        {
            case GameMode.Continuous:
                if (firstPortal)
                {
                    ++curFilledCont;
                    firstPortal = false;
                }
                newLevelScore.score = score;
                newLevelScore.time = RoundTimer.timeInLevel;
                newLevelScore.board = (int)BoardManager.currentBoardSelection;
                newLevelScore.positions = recorder.positions.ToArray();
                newLevelScore.rotations = recorder.rotations.ToArray();
                if (curFilledCont < 10)
                {
                    topContinuousScores[curFilledCont].difficulty = GameManager.gameDifficulty;
                    topContinuousScores[curFilledCont].levels[level] = newLevelScore;
                    topContinuousScores[curFilledCont].isLastScoreInput = true;
                }
                else
                {
                    topContinuousScores[9].difficulty = GameManager.gameDifficulty;
                    topContinuousScores[9].levels[level] = newLevelScore;
                    topContinuousScores[9].isLastScoreInput = true;
                }
                SortContinuousScores(topContinuousScores);
                break;
            case GameMode.Cursed:
                newLevelScore.score = score;
                newLevelScore.time = RoundTimer.timeLeft;
                newLevelScore.board = (int)BoardManager.currentBoardSelection;
                newLevelScore.positions = recorder.positions.ToArray();
                newLevelScore.rotations = recorder.rotations.ToArray();
                newLevelScore.isLastScoreInput = true;
                newLevelScore.difficulty = GameManager.gameDifficulty;
                if (topCursedScores[level].currentAmoutFilled < 10)
                {
                    topCursedScores[level].cursedScores[topCursedScores[level].currentAmoutFilled] = newLevelScore;
                    ++topCursedScores[level].currentAmoutFilled;
                }
                else
                    topCursedScores[level].cursedScores[9] = newLevelScore;
                SortCurseScores(topCursedScores[level].cursedScores, topCursedScores[level].currentAmoutFilled);
                break;
            case GameMode.Race:
                newLevelScore.score = score;
                newLevelScore.time = RoundTimer.timeLeft;
                newLevelScore.board = (int)BoardManager.currentBoardSelection;
                newLevelScore.positions = recorder.positions.ToArray();
                newLevelScore.rotations = recorder.rotations.ToArray();
                newLevelScore.isLastScoreInput = true;
                newLevelScore.difficulty = GameManager.gameDifficulty;
                if (topRaceScores[level].currentAmoutFilled < 10)
                {
                    topRaceScores[level].racescores[topRaceScores[level].currentAmoutFilled] = newLevelScore;
                    ++topRaceScores[level].currentAmoutFilled;
                }
                else
                    topRaceScores[level].racescores[9] = newLevelScore;
                SortRaceScores(topRaceScores[level].racescores, topRaceScores[level].currentAmoutFilled);
                break;
        }
    }
    private static void SortCurseScores(ScoreData[] scores, int arrayLength)
    {
        int curr = 1, comparer;
        ScoreData storedScore;
        while (curr < arrayLength)
        {
            storedScore = scores[curr];
            comparer = curr - 1;
            while (comparer >= 0)
            {
                if (scores[comparer].score < storedScore.score)
                {
                    scores[comparer + 1] = scores[comparer];
                    --comparer;
                }
                else if (scores[comparer].score == storedScore.score && scores[comparer].time > storedScore.time)
                {
                    scores[comparer + 1] = scores[comparer];
                    --comparer;
                }
                else
                    break;
            }
            scores[comparer + 1] = storedScore;
            ++curr;
        }
    }
    private static void SortContinuousScores(ContinuousScores[] array)
    {
        int i, j, k, keyScore, cumulativeScore;
        ContinuousScores key;
        for (i = 1; i < array.Length; ++i)
        {
            key = array[i];
            keyScore = 0;
            for (j = 0; j < array[i].levels.Length; ++j)
                keyScore += array[i].levels[j].score;
            j = i - 1;
            while (j >= 0)
            {
                cumulativeScore = 0;
                for (k = 0; k < array[j].levels.Length; ++k)
                    cumulativeScore += array[j].levels[k].score;
                if (cumulativeScore < keyScore)
                {
                    array[j + 1] = array[j];
                    --j;
                }
                else
                    break;
            }
            array[j + 1] = key;
        }
    }
    private static void SortRaceScores(ScoreData[] scores, int arrayLength)
    {
        int curr = 1, comparer;
        ScoreData storedScore;
        while (curr < arrayLength)
        {
            storedScore = scores[curr];
            comparer = curr - 1;
            while (comparer >= 0)
            {
                if (scores[comparer].score < storedScore.score)
                {
                    scores[comparer + 1] = scores[comparer];
                    --comparer;
                }
                else if (scores[comparer].score == storedScore.score && scores[comparer].time > storedScore.time)
                {
                    scores[comparer + 1] = scores[comparer];
                    --comparer;
                }
                else
                    break;
            }
            scores[comparer + 1] = storedScore;
            ++curr;
        }
    }
    private static void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        prevRingTransform = LevelManager.SpawnPoints[SceneManager.GetActiveScene().buildIndex];
        RoundTimer.timeInLevel = 0.0f;
        prevRingBonusTime = 0.0f;
        respawnCount = 0;
        respawnEnabled = GameMode.Cursed == GameManager.gameMode;
    }
    private void Update()
    {
        if (GameState.GamePlay == GameManager.gameState)
        {
            RoundTimer.UpdateTimers();
            if (respawnEnabled && RoundTimer.timeLeft <= 0.0 && !playerRespawnScript.IsRespawning)
            {
                if (respawnCount < maxRespawnCount)
                    playerRespawnScript.RespawnPlayer(prevRingTransform, 5.0f + prevRingBonusTime);
                else
                    EventManager.OnTriggerTransition(1);
                ++respawnCount;
            }
        }
    }
    private void OnEnable() => SceneManager.sceneLoaded += OnLevelLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnLevelLoaded;
    private void OnApplicationQuit() => SaveLoader.Save();
}