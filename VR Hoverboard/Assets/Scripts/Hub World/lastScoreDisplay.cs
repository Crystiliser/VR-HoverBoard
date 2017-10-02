using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lastScoreDisplay : MonoBehaviour
{
    [SerializeField] TextMeshPro scoreDisplay;
    [SerializeField] TextMeshPro timeDisplay;


    ScoreManager scoreScript;
    GameManager gameManager;
    int lastScore = 0;
    float lastTime = 0;

    int lastPortalBuildIndex;

    int lastScoreLocation;

    private void Start()
    {
        gameManager = GameManager.instance;
        scoreScript = gameManager.scoreScript;
        findLastScore();
        displayLastScore();
    }

    void findLastScore()
    {
        switch (gameManager.gameMode.currentMode)
        {
            case GameModes.Continuous:

                for (int i = 0; i < scoreScript.topContinuousScores.Length; i++)
                {
                    if (scoreScript.topContinuousScores[i].isLastScoreInput)
                    {
                        lastScoreLocation = i;
                        //scoreScript.topContinuousScores[i].isLastScoreInput = false;
                        break;
                    }
                }
                
                lastPortalBuildIndex = gameManager.lastPortalBuildIndex;
                if (lastPortalBuildIndex > 1)
                {
                        for (int j = 0; j < scoreScript.topContinuousScores[lastScoreLocation].levels.Length; j++)
                        {
                            lastScore += scoreScript.topContinuousScores[lastScoreLocation].levels[j].score;
                            lastTime += scoreScript.topContinuousScores[lastScoreLocation].levels[j].time;
                        }
                }
                else
                {
                    lastScore = 0;
                    lastTime = 0;
                }


                
                break;

            case GameModes.Cursed:
                
                for (int i = 0; i < scoreScript.topCurseScores.Length; i++)
                {
                    int j = 0;
                    for (; j < scoreScript.topCurseScores[i].curseScores.Length; j++)
                    {
                        if (scoreScript.topCurseScores[i].curseScores[j].isLastScoreInput)
                        {
                            lastScoreLocation = j;
                            //scoreScript.topCurseScores[i].curseScores[j].isLastScoreInput = false;
                            break;
                        }
                    }
                    if(j < scoreScript.topCurseScores[i].curseScores.Length - 1)
                    {
                        break;
                    }
                }

                lastPortalBuildIndex = gameManager.lastPortalBuildIndex;
                if (lastPortalBuildIndex > 1)
                {
                    lastScore = scoreScript.topCurseScores[lastPortalBuildIndex].curseScores[lastScoreLocation].score;
                    lastTime = scoreScript.topCurseScores[lastPortalBuildIndex].curseScores[lastScoreLocation].time;
                }
                else
                {
                    lastScore = 0;
                    lastTime = 0;
                }



                break;

            case GameModes.Free:
                Debug.Log("Free Mode shouldnt display anything");
                break;

            case GameModes.GameModesSize:
                break;

            default:
                break;
        }
    }

    void displayLastScore()
    {
        scoreDisplay.SetText("Score: " + lastScore);
        timeDisplay.SetText("Time: " + lastTime.ToString("n2"));
    }
}
