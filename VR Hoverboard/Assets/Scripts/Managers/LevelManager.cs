using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public enum Level { Canyon, MultiEnvironment, BackyardRacetrack, ComputerChip, City, Venice, NumLevels }
    private static ScreenFade screenFade = null;
    private static PlayerMenuController menuController = null;
    public static int nextScene = 0;
    public static bool RingPathIsOn = true;
    private static Sprite[] stcLevelPreviews = null;
    public static bool mirrorMode = false;
    public static bool reverseMode = false;
    public static Level savedCurrentLevel = Level.Canyon;
    private static Transform[] stcSpawnPoints = null;
    public static Transform[] SpawnPoints => stcSpawnPoints;
    [SerializeField] private Transform[] spawnPoints = null;
    [SerializeField] private Sprite[] levelPreviews = null;
    public static Sprite GetLevelPreview(Level level) => stcLevelPreviews[(int)level];
    public static void SetupLevelManager()
    {
        menuController = GameManager.player.GetComponent<PlayerMenuController>();
        screenFade = GameManager.player.GetComponentInChildren<ScreenFade>();
    }
    private void Awake()
    {
        stcLevelPreviews = levelPreviews;
        stcSpawnPoints = spawnPoints;
        UnityEngine.Assertions.Assert.AreEqual(SceneManager.sceneCountInBuildSettings, spawnPoints.Length, $"GameManager.instance.levelScript.spawnPoints.Length should be {SceneManager.sceneCountInBuildSettings}.. Actual value: {spawnPoints.Length}");
        UnityEngine.Assertions.Assert.AreEqual(LevelCount, levelPreviews.Length, $"GameManager.instance.levelScript.levelPreviews.Length should be {LevelCount}.. Actual value: {levelPreviews.Length}");
    }
    private static void DoSceneTransition(int sceneIndex)
    {
        nextScene = sceneIndex;
        EventManager.OnTriggerSelectionLock(true);
        GameManager.player.GetComponentInChildren<effectController>().disableAllEffects();
        screenFade.StartTransitionFade();
    }
    private void UndoSceneTransitionLocks(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex >= LevelBuildOffset)
        {
            EventManager.OnSetHudOnOff(true);
            ApplyGamemodeChanges();
            GameManager.gameState = GameState.GamePlay;
        }
        else
        {
            if (GameManager.lastPortalBuildIndex != -1)
                menuController.ToggleMenuMovement(true);
            EventManager.OnSetHudOnOff(false);
            GameManager.gameState = GameState.HubWorld;
            ScoreManager.score = 0;
            ScoreManager.ringHitCount = 0;
            ScoreManager.firstPortal = true;
        }
        GameManager.player.transform.SetPositionAndRotation(spawnPoints[scene.buildIndex].position, spawnPoints[scene.buildIndex].rotation);
        EventManager.OnTriggerSelectionLock(false);
    }
    private static void ApplyGamemodeChanges()
    {
        if (GameMode.Free == GameManager.gameMode)
        {
            GameManager.lastPortalBuildIndex = -1;
            EventManager.OnCallSetRingPath(false);
        }
        else
            EventManager.OnCallSetRingPath(RingPathIsOn);
    }
    private void OnEnable()
    {
        EventManager.OnTransition += DoSceneTransition;
        SceneManager.sceneLoaded += UndoSceneTransitionLocks;
    }
    private void OnDisable()
    {
        EventManager.OnTransition -= DoSceneTransition;
        SceneManager.sceneLoaded -= UndoSceneTransitionLocks;
    }
    public const int HubWorldBuildIndex = 1;
    public const int LevelBuildOffset = HubWorldBuildIndex + 1;
    public const int LevelCount = (int)(Level.NumLevels);
    public const int MirroredOffset = LevelCount;
    public const int MirroredBuildOffset = LevelBuildOffset + MirroredOffset;
    public const int ReversedOffset = MirroredOffset + LevelCount;
    public const int ReversedBuildOffset = LevelBuildOffset + ReversedOffset;
    public const int MirroredReversedOffset = ReversedOffset + LevelCount;
    public const int MirroredReversedBuildOffset = LevelBuildOffset + MirroredReversedOffset;
    public static int GetLevelOffset => mirrorMode ? (reverseMode ? MirroredReversedOffset : MirroredOffset) : (reverseMode ? ReversedOffset : 0);
    public static int GetBuildOffset => LevelBuildOffset + GetLevelOffset;
}