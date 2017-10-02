using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerTextUpdateScript : MonoBehaviour
{
    ManagerClasses.RoundTimer roundTimer;

    TextMeshProUGUI element;
    bool textIsRed;
    float timeToTurnTextRed;

    Color originalTextColor;
    GameManager gameManager;

	void Start ()
    {
        gameManager = GameManager.instance;
        roundTimer = gameManager.roundTimer;
        element = gameObject.GetComponent<TextMeshProUGUI>();

        textIsRed = false;
        timeToTurnTextRed = 2f;
        originalTextColor = element.color;
    }
	
	void Update ()
    {
        string textToWrite = "TIMER BROKE";
        switch (gameManager.gameMode.currentMode)
        {
            case GameModes.Continuous:

                textToWrite = " " + roundTimer.TimeInLevel.ToString("n2") + " ";

                break;

            case GameModes.Cursed:

                if (!textIsRed && roundTimer.TimeLeft < timeToTurnTextRed)
                {
                    element.color = new Color(1f, 0f, 0f, 1f);
                    textIsRed = true;
                }
                else if (textIsRed && roundTimer.TimeLeft > timeToTurnTextRed)
                {
                    element.color = originalTextColor;
                    textIsRed = false;
                }

                textToWrite = " " + roundTimer.TimeLeft.ToString("n2") + " ";

                break;

            case GameModes.Free:

                textToWrite = " " + roundTimer.TimeInLevel.ToString("n2") + " ";

                break;

            default:
                break;
        }

        element.SetText(textToWrite);
	}
}
