using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class closeScoreSplashScreen : SelectedObject
{
    [SerializeField] TextMeshPro lOne = null;
    [SerializeField] TextMeshPro lTwo = null;
    [SerializeField] TextMeshPro lThree = null;


    public GameObject panel;


    ScoreManager scoreScript;
    GameManager gameManager;
    int lastPortalBuildIndex;

    int lastScoreLocation;

    [SerializeField] LevelMenu lMenu;

    new private void Start()
    {
        base.Start();
        gameManager = GameManager.instance;
        scoreScript = gameManager.scoreScript;
        if (gameManager.lastPortalBuildIndex < 2)
        {
            gameObject.SetActive(false);
            panel.SetActive(false);
            GameManager.player.GetComponent<PlayerMenuController>().ToggleMenuMovement(false);
        }
    }

    override public void selectSuccessFunction()
    {
        GameManager.player.GetComponent<PlayerMenuController>().ToggleMenuMovement(false);
        gameObject.SetActive(false);
        panel.SetActive(false);

        switch (gameManager.gameMode.currentMode)
        {
            case GameModes.Continuous:
                for (int i = 0; i < scoreScript.topContinuousScores.Length; i++)
                {
                    if (scoreScript.topContinuousScores[i].isLastScoreInput)
                    {
                        lastScoreLocation = i;
                        scoreScript.topContinuousScores[i].isLastScoreInput = false;
                        break;
                    }
                }
                lastPortalBuildIndex = gameManager.lastPortalBuildIndex;
                if (lastPortalBuildIndex > 1)
                {
                    scoreScript.topContinuousScores[lastScoreLocation].name 
                        = lOne.GetParsedText() + lTwo.GetParsedText() + lThree.GetParsedText();
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
                            scoreScript.topCurseScores[i].curseScores[j].isLastScoreInput = false;
                            break;
                        }
                    }
                    if (j < scoreScript.topCurseScores[i].curseScores.Length - 1)
                    {
                        break;
                    }
                }
                
                lastPortalBuildIndex = gameManager.lastPortalBuildIndex;
                if (lastPortalBuildIndex > 1)
                {
                    scoreScript.topCurseScores[lastPortalBuildIndex].curseScores[lastScoreLocation].name
                        = lOne.GetParsedText() + lTwo.GetParsedText() + lThree.GetParsedText();
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
        lMenu.UpdateScoreDisplay();
    }
}
