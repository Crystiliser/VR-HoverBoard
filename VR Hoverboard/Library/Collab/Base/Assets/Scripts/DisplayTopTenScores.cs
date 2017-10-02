using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayTopTenScores : MonoBehaviour
{
    [SerializeField] TextMeshPro[] scoreDisplays = new TextMeshPro[10];
    int[] scores = new int[10];
    float[] times = new float[10];
    string[] names = new string[10];

    ScoreManager scoreScript;
    GameManager gameManager;

    int levelDisplayed = 3;

	void Start ()
    {
        gameManager = GameManager.instance;
        scoreScript = gameManager.scoreScript;

        switch (gameManager.gameMode.currentMode)
        {
            case GameModes.Continuous:
                for (int i = 0; i < scoreScript.topContinuousScores.Length; i++)
                {
                    int cumulativeScore = 0;
                    float totalTime = 0;
                    for (int j = 0; j < scoreScript.topContinuousScores[i].levels.Length; j++)
                    {
                        cumulativeScore += scoreScript.topContinuousScores[i].levels[j].score;
                        totalTime += scoreScript.topContinuousScores[i].levels[j].score;
                    }
                    scores[i] = cumulativeScore;
                    times[i] = totalTime;
                    names[i] = scoreScript.topContinuousScores[i].name;
                }

                break;

            case GameModes.Cursed:
                for (int i = 0; i < scoreScript.topCurseScores[levelDisplayed].curseScores.Length; i++)
                {
                    scores[i] = scoreScript.topCurseScores[levelDisplayed].curseScores[i].score;
                    times[i] = scoreScript.topCurseScores[levelDisplayed].curseScores[i].time;
                }
                
                break;

            case GameModes.Free:
                break;
            case GameModes.GameModesSize:
                break;
            default:
                break;
        }

        for (int i = 0; i < scoreDisplays.Length; i++)
        {
            scoreDisplays[i].SetText(names[i] + " | " + scores[i] + " | " + times[i].ToString("n2") + " ");
        }

    }
	
	void Update () {
		
	}
}
