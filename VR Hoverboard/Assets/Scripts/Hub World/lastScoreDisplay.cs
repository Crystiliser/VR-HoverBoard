using UnityEngine;
using TMPro;
public class lastScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreDisplay = null, timeDisplay = null;
    private void Start()
    {
        int lastScore = 0, lastScoreLocation = 0, i = 0, j = 0;
        float lastTime = 0.0f;
        switch (GameManager.gameMode)
        {
            case GameMode.Continuous:
                for (i = 0; i < ScoreManager.topContinuousScores.Length; ++i)
                {
                    if (ScoreManager.topContinuousScores[i].isLastScoreInput)
                    {
                        lastScoreLocation = i;
                        break;
                    }
                }
                if (GameManager.lastPortalBuildIndex >= LevelManager.LevelBuildOffset)
                {
                    for (i = 0; i < ScoreManager.topContinuousScores[lastScoreLocation].levels.Length; ++i)
                    {
                        lastScore += ScoreManager.topContinuousScores[lastScoreLocation].levels[i].score;
                        lastTime += ScoreManager.topContinuousScores[lastScoreLocation].levels[i].time;
                    }
                }
                else
                {
                    lastScore = 0;
                    lastTime = 0.0f;
                }
                break;
            case GameMode.Cursed:
                for (i = 0; i < ScoreManager.topCursedScores.Length; ++i)
                {
                    for (j = 0; j < ScoreManager.topCursedScores[i].cursedScores.Length; ++j)
                    {
                        if (ScoreManager.topCursedScores[i].cursedScores[j].isLastScoreInput)
                        {
                            lastScoreLocation = j;
                            break;
                        }
                    }
                    if (j < ScoreManager.topCursedScores[i].cursedScores.Length - 1)
                        break;
                }
                if (GameManager.lastPortalBuildIndex >= LevelManager.LevelBuildOffset)
                {
                    lastScore = ScoreManager.topCursedScores[GameManager.lastPortalBuildIndex].cursedScores[lastScoreLocation].score;
                    lastTime = ScoreManager.topCursedScores[GameManager.lastPortalBuildIndex].cursedScores[lastScoreLocation].time;
                }
                else
                {
                    lastScore = 0;
                    lastTime = 0.0f;
                }
                break;
            case GameMode.Free:
                break;
            case GameMode.Race:
                for (i = 0; i < ScoreManager.topRaceScores.Length; ++i)
                {
                    for (j = 0; j < ScoreManager.topRaceScores[i].racescores.Length; ++j)
                    {
                        if (ScoreManager.topRaceScores[i].racescores[j].isLastScoreInput)
                        {
                            lastScoreLocation = j;
                            break;
                        }
                    }
                    if (j < ScoreManager.topRaceScores[i].racescores.Length - 1)
                        break;
                }
                if (GameManager.lastPortalBuildIndex >= LevelManager.LevelBuildOffset)
                {
                    lastScore = ScoreManager.topRaceScores[GameManager.lastPortalBuildIndex].racescores[lastScoreLocation].score;
                    lastTime = ScoreManager.topRaceScores[GameManager.lastPortalBuildIndex].racescores[lastScoreLocation].time;
                }
                else
                {
                    lastScore = 0;
                    lastTime = 0.0f;
                }
                break;
        }
        scoreDisplay.SetText("Score: " + lastScore);
        timeDisplay.SetText("Time: " + lastTime.ToString("n2"));
        Destroy(this);
    }
}