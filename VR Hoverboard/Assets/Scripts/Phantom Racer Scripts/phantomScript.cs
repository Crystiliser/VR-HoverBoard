using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class phantomScript : MonoBehaviour
{
    ScoreManager.scoreStruct scoreInfo;
    int currentPos = 0;

	// Use this for initialization
	void Start ()
    {
        int level = SceneManager.GetActiveScene().buildIndex;

        switch (GameManager.instance.gameMode.currentMode)
        {
            case GameModes.Continuous:
                ScoreManager.continuousScores[] contScores = GameManager.instance.scoreScript.topContinuousScores;
                scoreInfo = contScores[0].levels[level];
                break;

            case GameModes.Cursed:
                ScoreManager.levelCurseScores[] levelScores = GameManager.instance.scoreScript.topCurseScores;

                scoreInfo = levelScores[level].curseScores[0];
                break;

            case GameModes.Free:
                break;

            case GameModes.GameModesSize:
                break;

            default:
                break;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (scoreInfo.positions != null)
        {
            gameObject.transform.position = scoreInfo.positions[currentPos];
            gameObject.transform.rotation = scoreInfo.rotations[currentPos];
            currentPos++;
            if (currentPos >= scoreInfo.positions.Length)
            {
                currentPos = 0;
            }
        }
	}
}
