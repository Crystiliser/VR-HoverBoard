using UnityEngine;
using TMPro;
public class TimerTextUpdateScript : MonoBehaviour
{
    private TextMeshProUGUI element = null;
    private bool textIsRed = false;
    private float timeToTurnTextRed = 2.0f;
    private Color originalTextColor;
    private string textToWrite = "TIMER BROKE";
    private void Start()
    {
        element = GetComponent<TextMeshProUGUI>();
        textIsRed = false;
        timeToTurnTextRed = 2.0f;
        originalTextColor = element.color;
    }
    private void Update()
    {
        switch (GameManager.gameMode)
        {
            case GameMode.Continuous:
                textToWrite = " " + RoundTimer.timeInLevel.ToString("n2") + " ";
                break;
            case GameMode.Cursed:
                if (!textIsRed && RoundTimer.timeLeft < timeToTurnTextRed)
                {
                    element.color = Color.red;
                    textIsRed = true;
                }
                else if (textIsRed && RoundTimer.timeLeft > timeToTurnTextRed)
                {
                    element.color = originalTextColor;
                    textIsRed = false;
                }
                textToWrite = " " + RoundTimer.timeLeft.ToString("n2") + " ";
                break;
            case GameMode.Free:
                textToWrite = " " + RoundTimer.timeInLevel.ToString("n2") + " ";
                break;
            case GameMode.Race:
                textToWrite = " " + RoundTimer.timeInLevel.ToString("n2") + " ";
                break;
        }
        element.SetText(textToWrite);
    }
}