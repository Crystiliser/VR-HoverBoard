using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    enum DisplayToUpdate { TopScores, GameMode, AICount, Difficulty, PortalSelect, displayCount };
    bool[] displayUpdateFlags;

    enum Levels { Canyon = 2, MultiEnvironment = 3, BackyardRacetrack = 4, levelCount };
    Levels currentLevel = Levels.Canyon;

    [SerializeField] BackBoardSinkEffect sinkEffect;
    [SerializeField] Transform[] backs;

    ManagerClasses.GameMode gameMode;

    [Header("Level Options")]
    [SerializeField]
    TextMeshPro gameModeTMP;

    [Header("Portal Select")]
    [SerializeField]
    WorldPortalProperties portal;
    [SerializeField] Image preview;
    [SerializeField] Sprite[] previewSprites;

    [Header("Top Scores")]
    [SerializeField]
    TextMeshPro[] highScoreTMPS;
    ScoreManager scoreScript;
    int[] scores = new int[10];
    float[] times = new float[10];
    string[] names = new string[10];

    void Start()
    {
        gameMode = GameManager.instance.gameMode;
        scoreScript = GameManager.instance.scoreScript;
        gameModeTMP.text = gameMode.currentMode.ToString();

        displayUpdateFlags = new bool[(int)DisplayToUpdate.displayCount];

        //set our preview to the last level we were in
        if (GameManager.instance.lastPortalBuildIndex > 1)
        {
            currentLevel = (Levels)GameManager.instance.lastPortalBuildIndex;
            portal.SceneIndex = (int)currentLevel;

            preview.sprite = previewSprites[GameManager.instance.lastPortalBuildIndex - 2];

        }
        else
        {
            UpdateScoreDisplay();
        }
    }

    public void NextLevel()
    {
        if (currentLevel + 1 == Levels.levelCount)
            currentLevel = Levels.Canyon;
        else
            ++currentLevel;

        portal.SceneIndex = (int)currentLevel;

        displayUpdateFlags[(int)DisplayToUpdate.TopScores] = true;
        displayUpdateFlags[(int)DisplayToUpdate.PortalSelect] = true;
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.TopScores]));
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.PortalSelect]));
    }

    public void PreviousLevel()
    {
        if (currentLevel - 1 < Levels.Canyon)
            currentLevel = Levels.levelCount - 1;
        else
            --currentLevel;

        portal.SceneIndex = (int)currentLevel;

        displayUpdateFlags[(int)DisplayToUpdate.TopScores] = true;
        displayUpdateFlags[(int)DisplayToUpdate.PortalSelect] = true;
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.TopScores]));
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.PortalSelect]));
    }

    public void NextGameMode()
    {
        gameMode.NextMode();

        displayUpdateFlags[(int)DisplayToUpdate.TopScores] = true;
        displayUpdateFlags[(int)DisplayToUpdate.GameMode] = true;
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.TopScores]));
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.GameMode]));
    }

    public void PreviousGameMode()
    {
        gameMode.PreviousMode();

        displayUpdateFlags[(int)DisplayToUpdate.TopScores] = true;
        displayUpdateFlags[(int)DisplayToUpdate.GameMode] = true;
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.TopScores]));
        StartCoroutine(sinkEffect.SinkEffectCoroutine(backs[(int)DisplayToUpdate.GameMode]));
    }

    public void UpdateScoreDisplay()
    {
        switch (gameMode.currentMode)
        {
            case GameModes.Continuous:
                for (int i = 0; i < scoreScript.topContinuousScores.Length; ++i)
                {
                    int cumulativeScore = 0;
                    float totalTime = 0;
                    for (int j = 0; j < scoreScript.topContinuousScores[i].levels.Length; ++j)
                    {
                        cumulativeScore += scoreScript.topContinuousScores[i].levels[j].score;
                        totalTime += scoreScript.topContinuousScores[i].levels[j].time;
                    }
                    scores[i] = cumulativeScore;
                    times[i] = totalTime;
                    names[i] = scoreScript.topContinuousScores[i].name;
                }
                break;
            case GameModes.Cursed:
                for (int i = 0; i < scoreScript.topCurseScores[(int)currentLevel].curseScores.Length; ++i)
                {
                    scores[i] = scoreScript.topCurseScores[(int)currentLevel].curseScores[i].score;
                    times[i] = scoreScript.topCurseScores[(int)currentLevel].curseScores[i].time;
                    names[i] = scoreScript.topCurseScores[(int)currentLevel].curseScores[i].name;
                }
                break;
            case GameModes.Free:
                for (int i = 0; i < 10; ++i)
                {
                    scores[i] = 0;
                    times[i] = 0;
                    names[i] = "NOBODY";
                }
                break;
            case GameModes.GameModesSize:
                break;
            default:
                break;
        }

        for (int i = 0; i < highScoreTMPS.Length; ++i)
        {
            highScoreTMPS[i].SetText(i + ": " + names[i] + " | " + scores[i] + " | " + times[i].ToString("n2") + " ");
        }

    }

    private void CheckUpdateFlags()
    {
        for (int i = 0; i < displayUpdateFlags.Length; i++)
        {
            if (displayUpdateFlags[i])
            {
                UpdateDisplay((DisplayToUpdate)i);
                displayUpdateFlags[i] = false;
            }
        }
    }

    private void UpdateDisplay(DisplayToUpdate display)
    {
        switch (display)
        {
            case DisplayToUpdate.TopScores:
                UpdateScoreDisplay();
                break;
            case DisplayToUpdate.GameMode:
                gameModeTMP.text = gameMode.currentMode.ToString();
                break;
            case DisplayToUpdate.AICount:
                break;
            case DisplayToUpdate.Difficulty:
                break;
            case DisplayToUpdate.PortalSelect:
                preview.sprite = previewSprites[(int)currentLevel - 2];
                break;
        }
    }

    private void OnEnable()
    {
        BackBoardSinkEffect.StartContentUpdate += CheckUpdateFlags;
    }

    private void OnDisable()
    {
        BackBoardSinkEffect.StartContentUpdate -= CheckUpdateFlags;
    }
}